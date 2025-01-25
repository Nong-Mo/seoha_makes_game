using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float max_range = 15f;
    public GameObject explosion_effect;

    private Vector2 start_position;
    private RewindableObject rewindable;

    // Start is called before the first frame update
    private void Start()
    {
        start_position = transform.position;
        rewindable = GetComponent<RewindableObject>();

        // ���� �� �ʱ� ���� ���
        if(null != rewindable)
        {
            rewindable.RecordInitialState(transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // �Ѿ��� ������ �̵�
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // �����Ÿ� �ʰ� �� ����
        if(Vector2.Distance(start_position, transform.position) >= max_range)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹 �� ����
        Explode();
    }

    private void Explode()
    {
        // ���� ����Ʈ ����
        if(null != explosion_effect)
        {
            Instantiate(explosion_effect, transform.position, Quaternion.identity);
        }

        // ������Ʈ ���� ��� Kill() ȣ��
        if (null != rewindable)
            rewindable.Kill();
        else
            gameObject.SetActive(false);
    }
}
