using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    private bool is_rewinding = false;

    [SerializeField]
    private List<ObjectState> state_history = new List<ObjectState>();
    private Rigidbody2D rb;
    public float rewind_duration = 3f;

    [System.Serializable]
    private struct ObjectState {
        public Vector2 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public bool is_active;

        public ObjectState(Vector2 position, Quaternion rotation, Vector2 velocity, bool is_active) {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.is_active = is_active;
        }
    }

    // Start is called before the first frame update
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Debug.Log($"FixedUpdate �����: {gameObject.name}, is_rewinding: {is_rewinding}");
        if (is_rewinding) {
            Debug.Log("in Rewind");
            Rewind();
        }
        else
            RecordState();
    }

    public void RecordInitialState(Vector2 position, Quaternion rotation) {
        state_history.Insert(0, new ObjectState(position, rotation, Vector3.zero, true));
    }

    public void StartRewind() {
        Debug.Log("In StartRewind");
        is_rewinding = true;
        rb.isKinematic = true; // ���� ��Ȱ��ȭ
    }

    public void StopRewind() {
        is_rewinding = false;
        rb.isKinematic = true;

        // �����ε� ���� �� ������ ���� ����
        if (state_history.Count > 0) {
            ObjectState lastState = state_history[0];
            rb.velocity = lastState.velocity;
        }

    }

    private void RecordState() {
        if (state_history.Count > Mathf.Round(rewind_duration / Time.fixedDeltaTime)) {
            ObjectState oldest_state = state_history[state_history.Count - 1];

            // ����Ʈ�� ���� ������  ���°� ��Ȱ��ȭ ���¶�� ������Ʈ�� ����
            if(!oldest_state.is_active) {
                Destroy(gameObject);
                return; // ���� ���� ���� ����
            }

            state_history.RemoveAt(state_history.Count - 1);
        }
            
        state_history.Insert(0, new ObjectState(transform.position, transform.rotation, rb.velocity, gameObject.activeSelf));
    }

    public void Kill() {
        gameObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ 
    }

    private void Rewind() { 
        if (0 < state_history.Count) {
            ObjectState state = state_history[0]; // �ֱ� ���� ��������

            // ���� ���¿� �ֱ� ���¸� ���ϸ� �α� ���
            Debug.Log($"[Rewind] �ֱ� ����: Position={state.position}, Rotation={state.rotation.eulerAngles}, " +
                      $"Velocity={state.velocity}, IsActive={state.is_active}");
            Debug.Log($"[Rewind] ���� ����: Position={transform.position}, Rotation={transform.rotation.eulerAngles}, " +
                      $"Velocity={(rb != null ? rb.velocity : Vector2.zero)}, IsActive={gameObject.activeSelf}");


            transform.position = state.position;
            transform.rotation = state.rotation;

            // ���� ������Ʈ ����
            if (!state.is_active)
                gameObject.SetActive(true);
 
            state_history.RemoveAt(0); // �ֱ� ���� ����
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
