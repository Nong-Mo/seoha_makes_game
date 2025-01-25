using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    private bool is_rewinding = false;
    private List<ObjectState> state_history = new List<ObjectState>();
    public float rewind_duration = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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
