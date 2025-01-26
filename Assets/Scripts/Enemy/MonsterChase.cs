using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterChase : MonoBehaviour
{
    [Header("Target & Movement Settings")]
    public Transform player;
    public float speed = 2f;
    public float flankingSpeed = 3f;
    public float rushSpeed = 4f;
    public float surroundRadius = 3f;
    public float minDistanceFromOthers = 1.5f;
    public float pathUpdateInterval = 0.5f;
    public float nodeReachDistance = 0.1f;

    public LayerMask monsterLayer;

    [Header("Combat Settings")]
    public float attackInterval = 2f;
    public int damage = 10;
    public GameObject attackEffect;

    [Header("Health Settings")]
    public int health = 100;

    private enum MovementState { Surround, Flank, Rush, Retreat }
    private MovementState currentState;
    private float stateTimer;
    private Vector2 direction;
    private Vector2 offsetPosition;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isPlayerInRange = false;

    private Animator animator;
    private TilemapPathfinder pathfinder;
    private List<Vector3> currentPath;
    private int currentPathIndex;
    private float pathUpdateTimer;

    public bool IsAttacking => isAttacking;
    public bool IsDead { get; private set; }

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip walkSound;
    private AudioSource audioSource;
    private float footstepTimer = 0f;         // 추가
    private float footstepInterval = 0.5f;    // 추가

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pathfinder = FindObjectOfType<TilemapPathfinder>();

        if (pathfinder == null)
        {
            Debug.LogError($"{gameObject.name}: TilemapPathfinder not found!");
            return;
        }

        if (player == null)
        {
            Debug.Log($"{gameObject.name}: Finding player by tag...");
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError($"{gameObject.name}: Player not found!");
                return;
            }
            Debug.Log($"{gameObject.name}: Player found: {player.name}");
        }

        currentPath = new List<Vector3>();
        currentPathIndex = 0;
        pathUpdateTimer = 0f;

        SwitchState();
        UpdatePath();
    }

    void Update()
    {
        if (health <= 0 && !IsDead)
        {
            IsDead = true;
            return;
        }

        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0)
        {
            UpdatePath();
            pathUpdateTimer = pathUpdateInterval;
        }
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking || IsDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer <= 0)
            SwitchState();

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isPlayerInRange && distanceToPlayer <= 1.5f) // 공격 거리 명확히 지정
        {
            rb.velocity = Vector2.zero;
            if (!isAttacking)
                StartCoroutine(Attack());
        }
        else
        {
            FollowPath();
        }
    }

    void UpdatePath()
    {
        if (pathfinder == null || player == null)
        {
            Debug.LogError($"{gameObject.name}: Cannot update path - dependencies missing");
            return;
        }

        Vector3 targetPos = GetTargetPosition();
        Debug.Log($"{gameObject.name}: Finding path to target: {targetPos}");

        List<Vector3> newPath = null;
        int maxAttempts = 5;

        while (maxAttempts > 0 && (newPath == null || newPath.Count == 0))
        {
            newPath = pathfinder.FindPath(transform.position, targetPos);
            maxAttempts--;

            if (newPath.Count == 0)
            {
                Vector2 randomOffset = Random.insideUnitCircle * surroundRadius;
                targetPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                Debug.Log($"{gameObject.name}: Retrying with new target: {targetPos}");
            }
        }

        if (newPath.Count > 0)
        {
            currentPath = newPath;
            currentPathIndex = 0;
            Debug.Log($"{gameObject.name}: New path set with {currentPath.Count} points");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Failed to find path after {5 - maxAttempts} attempts");
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetPos = player.position;

        switch (currentState)
        {
            case MovementState.Surround:
                targetPos += (Vector3)offsetPosition;
                break;
            case MovementState.Flank:
                Vector2 perp = Vector2.Perpendicular((player.position - transform.position).normalized);
                targetPos += (Vector3)(perp * surroundRadius * (Random.value > 0.5f ? 1 : -1));
                break;
            case MovementState.Retreat:
                Vector3 awayDir = transform.position - player.position;
                targetPos = transform.position + awayDir.normalized * surroundRadius;
                pathUpdateTimer = pathUpdateInterval * 2f;
                break;
        }

        return targetPos;
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            rb.velocity = Vector2.zero;
            UpdatePath();
            return;
        }

        Vector2 targetPosition = currentPath[currentPathIndex];
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (distanceToTarget < nodeReachDistance)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                UpdatePath();
                return;
            }
            targetPosition = currentPath[currentPathIndex];
        }

        direction = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 finalDirection = direction;

        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(
            transform.position,
            minDistanceFromOthers,
            monsterLayer
        );

        Vector2 avoidanceDirection = Vector2.zero;
        foreach (Collider2D monster in nearbyMonsters)
        {
            if (monster.gameObject != gameObject)
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)monster.transform.position;
                avoidanceDirection += diff.normalized;
            }
        }

        if (avoidanceDirection != Vector2.zero)
            finalDirection = (direction + avoidanceDirection.normalized).normalized;

        float currentSpeed = currentState switch
        {
            MovementState.Flank => flankingSpeed,
            MovementState.Rush => rushSpeed,
            MovementState.Retreat => speed * 0.7f,
            _ => speed
        };

        rb.velocity = finalDirection * currentSpeed;

        if (rb.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                audioSource.PlayOneShot(walkSound);
                footstepTimer = 0f;
            }
        }

        if (animator != null)
        {
            animator.SetFloat("X", finalDirection.x);
            animator.SetFloat("Y", finalDirection.y);
        }
    }

    void SwitchState()
    {
        currentState = (MovementState)Random.Range(0, 4);
        stateTimer = Random.Range(2f, 4f);

        if (currentState == MovementState.Surround)
        {
            float randomAngle = Random.Range(0f, 360f);
            offsetPosition = new Vector2(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            ) * surroundRadius;
        }

        UpdatePath();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        // 플레이어가 여전히 범위 안에 있는지 확인
        if (!isPlayerInRange || player == null)
        {
            isAttacking = false;
            if (animator != null)
                animator.SetTrigger("Idle");
            yield break;
        }

        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect, transform.position, Quaternion.identity);
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(effect, ps.main.duration);
            else
                Destroy(effect, 1f);
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && isPlayerInRange) // 한번 더 확인
            playerHealth.TakeDamage(damage);

        yield return new WaitForSeconds(attackInterval - 0.5f);
        isAttacking = false;
        rb.velocity = Vector2.zero;
        if (animator != null)
            animator.SetTrigger("Idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isPlayerInRange = false;
    }

    public float GetCurrentSpeed()
    {
        if (rb == null) return 0f;
        return rb.velocity.magnitude;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        health -= amount;
        if (health <= 0) health = 0;
    }
}