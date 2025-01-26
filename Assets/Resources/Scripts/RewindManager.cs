using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public List<RewindableObject> rewindable_objects = new List<RewindableObject>();
    private List<(RewindableObject obj, float time_killed)> killed_objects = new List<(RewindableObject, float)>();
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
        if(Input.GetKeyUp(KeyCode.R))
            StopRewind();

        float current_time = Time.time;

        // killed_objects ����� �ڿ������� Ȯ���ذ��� �Ҹ� ��� �Ǵ�.
        for(int i = killed_objects.Count - 1; 0 <= i; i--) {
            var (obj, time_killed) = killed_objects[i];
            if(current_time - time_killed >= obj.rewind_duration) {
                if (null != obj) {
                    RemoveRewindableObject(obj);
                    if(null != obj.gameObject)
                        Destroy(obj.gameObject);
                    killed_objects.RemoveAt(i);
                }
            }
        }
    }

    private void FixedUpdate() {
        // ������Ʈ�� ��Ȱ��ȭ �Ǹ� Rewind�Լ��� FixedUpdate�� ȣ���� �� �� ���� ���� �̿��Ͽ� 
        // Manager�� ���� ȣ�� 
        foreach (var obj in rewindable_objects) {
            if (null != obj) {
                obj.ManualFixedUpdate();
            }
        }
    }

    public void StartRewind() {
        is_rewinding = true;

        foreach (var obj in rewindable_objects) {
            if (null != obj) {
                Debug.Log(obj);
                obj.StartRewind();
            }
        }
    }

    public void StopRewind() { 
        is_rewinding = false;

        foreach (var obj in rewindable_objects) {
            if (null != obj) {
                obj.StopRewind();
            }
        }
    }

    public void AddRewindableObject(RewindableObject obj) {
        if(!rewindable_objects.Contains(obj)) {
            rewindable_objects.Add(obj);
        }
    }

    public void RemoveRewindableObject(RewindableObject obj) {
        if(rewindable_objects.Contains(obj)) {
            rewindable_objects.Remove(obj);
        }
    }

    public void RegisterKilledObject(RewindableObject obj) {
        killed_objects.Add((obj, Time.time));
    }
}
