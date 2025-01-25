using UnityEngine;

public class MonsterAnim : MonoBehaviour
{
    private Animator animator;
    private MonsterChase chase;

    void Start()
    {
        animator = GetComponent<Animator>();
        chase = GetComponent<MonsterChase>();
    }

    void Update()
    {
        if (chase == null || animator == null) return;

        Vector2 velocity = chase.GetComponent<Rigidbody2D>().velocity.normalized;
        animator.SetFloat("X", velocity.x);
        animator.SetFloat("Y", velocity.y);

        float currentSpeed = chase.GetCurrentSpeed();
        animator.SetFloat("Speed", currentSpeed);  // Speed 파라미터로 전환

        // 공격/죽음은 SetTrigger 유지
        if (chase.IsAttacking)
        {
        }
        if (chase.IsDead)
        {
            animator.SetTrigger("Die");
        }
    }
}
