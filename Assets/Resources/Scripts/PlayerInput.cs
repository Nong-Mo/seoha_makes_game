using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerController player_controller;
    public GameObject bullet_prefab;
    public Transform fire_point; 

    // Start is called before the first frame update
    // �� ó�� �� �� Start ȣ��
    private void Start()
    {
        player_controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    // ������ �� �� �� ȣ��Ǵ� ����
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
            Fire();

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        Vector3 move_vector = (Vector3.up * v) + (Vector3.right * h);
        player_controller.MoveVector3 = move_vector;
    }

    private void Fire()
    {
        if (null != bullet_prefab && null != fire_point)
        {
            Instantiate(bullet_prefab, fire_point.position, fire_point.rotation);
        }
    }
}
