using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public Vector3 MoveVector2 = Vector2.zero;
    [SerializeField]
    public float MoveSpeed;

    // Start is called before the first frame update
    private void Start() {
        
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        transform.Translate(MoveVector2 * MoveSpeed * Time.fixedDeltaTime * Time.timeScale);
    }
}
