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

            // 리스트의 가장 오래된  상태가 비활성화 상태라면 오브젝트를 삭제
            if(!oldest_state.is_active) {
                Destroy(gameObject);
                return; // 이후 로직 실행 방지
            }

            state_history.RemoveAt(state_history.Count - 1);
        }
            
        state_history.Insert(0, new ObjectState(transform.position, transform.rotation, rb.velocity, gameObject.activeSelf));
    }

    public void Kill() {
        gameObject.SetActive(false); // 오브젝트 비활성화 
    }

    private void Rewind() { 
        if (0 < state_history.Count) {
            ObjectState state = state_history[0]; // 최근 상태 가져오기

            // 현재 상태와 최근 상태를 비교하며 로그 출력
            Debug.Log($"[Rewind] 최근 상태: Position={state.position}, Rotation={state.rotation.eulerAngles}, " +
                      $"Velocity={state.velocity}, IsActive={state.is_active}");
            Debug.Log($"[Rewind] 현재 상태: Position={transform.position}, Rotation={transform.rotation.eulerAngles}, " +
                      $"Velocity={(rb != null ? rb.velocity : Vector2.zero)}, IsActive={gameObject.activeSelf}");


            transform.position = state.position;
            transform.rotation = state.rotation;

            // 죽은 오브젝트 복구
            if (!state.is_active)
                gameObject.SetActive(true);
 
            state_history.RemoveAt(0); // 최근 상태 삭제
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
