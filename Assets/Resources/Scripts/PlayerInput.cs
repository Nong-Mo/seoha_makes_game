using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerController player_controller;
    // Start is called before the first frame update
    // 맨 처음 한 번 Start 호출
    private void Start()
    {
        player_controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    // 프레임 당 한 번 호출되는 로직
    private void Update()
    {
        PCInput();
    }

    private void FixedUpdate()
    {
        
    }

    private void PCInput()
    {
        if (Input.GetButtonDown("Fire1"))
            Debug.Log("Fire1 Click");

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        Vector3 move_vector = (Vector3.up * v) + (Vector3.right * h);
        player_controller.MoveVector3 = move_vector;
    }
}
