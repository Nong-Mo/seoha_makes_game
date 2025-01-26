using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    private bool is_rewinding = false;

    [SerializeField]
    private List<ObjectState> state_history = new List<ObjectState>();
    private Rigidbody2D rb;
    public float rewind_duration = 2f;

    // 플레이어/총알 등 오브젝트에 따라 값을 달리 세팅
    public bool can_destroy_when_history_empty = true;

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

    public void ManualFixedUpdate() {
        Debug.Log($"FixedUpdate 실행됨: {gameObject.name}, is_rewinding: {is_rewinding}");
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
        rb.isKinematic = true; // 물리 비활성화
    }

    public void StopRewind() {
        is_rewinding = false;
        rb.isKinematic = true;

        // 리와인드 종료 후 마지막 상태 복구
        if (state_history.Count > 0) {
            ObjectState lastState = state_history[0];
            rb.velocity = lastState.velocity;
        }

    }

    private void RecordState() {
        if (state_history.Count > Mathf.Round(rewind_duration / Time.fixedDeltaTime)) {
            ObjectState oldest_state = state_history[state_history.Count - 1];

            state_history.RemoveAt(state_history.Count - 1);
        }
            
        state_history.Insert(0, new ObjectState(transform.position, transform.rotation, rb.velocity, gameObject.activeSelf));
    }

    public void Kill() {
        gameObject.SetActive(false); // 오브젝트 비활성화 

        // RewindManager에 등록
        var rewind_manager = FindObjectOfType<RewindManager>();
        if (null != rewind_manager) {
            rewind_manager.RegisterKilledObject(this);
        }
    }

    private void Rewind() { 
        if (0 < state_history.Count) {
            ObjectState state = state_history[0]; // 최근 상태 가져오기

            // 현재 상태와 최근 상태를 비교하며 로그 출력
            Debug.Log($"[Rewind] 최근 상태: Position={state.position}, Rotation={state.rotation.eulerAngles}, " +
                      $"Velocity={state.velocity}, IsActive={state.is_active}");
            Debug.Log($"[Rewind] 현재 상태: Position={transform.position}, Rotation={transform.rotation.eulerAngles}, " +
                      $"Velocity={(rb != null ? rb.velocity : Vector2.zero)}, IsActive={gameObject.activeSelf}");
         
            gameObject.SetActive(state.is_active);

            transform.position = state.position;
            transform.rotation = state.rotation;

            state_history.RemoveAt(0); // 최근 상태 삭제
        }
        else {
            if (can_destroy_when_history_empty) {
                Destroy(gameObject);
            }
        }
    }
}
