using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public List<RewindableObject> rewindable_objects = new List<RewindableObject>();
    private bool is_rewinding = false;

    // Start is called before the first frame update
    private void Start() {
        
    }

    // Update is called once per frame
    private void Update() {   
        // RŰ�� �����ε� ����
        if(Input.GetKeyDown(KeyCode.R))
            StartRewind();


        // RŰ�� ���� �����ε� ����
        if(Input.GetKeyDown(KeyCode.R))
            StopRewind();
    }

    public void StartRewind() {
        is_rewinding = true;

        foreach(var obj in rewindable_objects)
            obj.StartRewind();
    }

    public void StopRewind() { 
        is_rewinding = false;

        foreach (var obj in rewindable_objects)
            obj.StopRewind();
    }
}
