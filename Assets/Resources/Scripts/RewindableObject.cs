using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    private bool is_rewinding = false;
    private List<ObjectState> state_history = new List<ObjectState>();
    public float rewind_duration = 3f;

    private Rigidbody rb;

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

    public void StartRewind()
    {
        is_rewinding = true;
        rb.isKinematic = true; // 물리 작동 중지
    }

    public void StopRewind()
    {
        is_rewinding = false;
        rb.isKinematic = false;
    }

    private void RecordState()
    {
        if (state_history.Count > Mathf.Round(rewind_duration / Time.fixedDeltaTime))
            state_history.RemoveAt(0); // 오래된 상태 삭제

        state_history.Add(new ObjectState(transform.position, transform.rotation, rb.velocity));
    }

    private void Rewind()
    {
        if(state_history.Count > 0)
        {
            ObjectState last_state = state_history[state_history.Count - 1]; // 최근 상태 가져오기
            transform.position = last_state.position;
            transform.rotation = last_state.rotation;
            rb.velocity = last_state.velocity;
            state_history.RemoveAt(state_history.Count - 1); // 최근 상태 삭제
        }
        else
        {
            StopRewind();
        }
    }

    private struct ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        public ObjectState(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
        }
    }
}
