using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    private bool is_rewinding = false;
    private List<ObjectState> state_history = new List<ObjectState>();
    private Rigidbody rb;
    public float rewind_duration = 3f;

    private struct ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public bool is_active;

        public ObjectState(Vector3 position, Quaternion rotation, Vector3 velocity, bool is_active)
        {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.is_active = is_active;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FinxedUpdate()
    {
        if (is_rewinding)
            Rewind();
        else
            RecordState();
    }

    public void RecordInitialState(Vector3 position, Quaternion rotation)
    {
        state_history.Insert(0, new ObjectState(position, rotation, Vector3.zero, true));
    }

    public void StartRewind()
    {
        is_rewinding = true;
        rb.isKinematic = true; // 물리 비활성화
    }

    public void StopRewind()
    {
        is_rewinding = false;
        rb.isKinematic = false;

        // 리와인드 종료 후 마지막 상태 복구
        if (state_history.Count > 0)
        {
            ObjectState lastState = state_history[0];
            rb.velocity = lastState.velocity;
        }

    }

    private void RecordState()
    {
        if (state_history.Count > Mathf.Round(rewind_duration / Time.fixedDeltaTime))
            state_history.RemoveAt(state_history.Count - 1);

        state_history.Insert(0, new ObjectState(transform.position, transform.rotation, rb.velocity, gameObject.activeSelf));
    }

    public void Kill()
    {
        
        gameObject.SetActive(false); // 오브젝트 비활성화 
    }

    private void Rewind()
    {
        if (0 < state_history.Count)
        {
            ObjectState state = state_history[0]; // 최근 상태 가져오기
            transform.position = state.position;
            transform.rotation = state.rotation;

            // 죽은 오브젝트 복구
            if (!state.is_active)
            {
                gameObject.SetActive(true);
            }

            state_history.RemoveAt(0); // 최근 상태 삭제
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
