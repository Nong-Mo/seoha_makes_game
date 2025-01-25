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

        // 생성 시 초기 상태 기록
        if(null != rewindable)
        {
            rewindable.RecordInitialState(transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // 총알을 앞으로 이동
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // 사정거리 초과 시 제거
        if(Vector2.Distance(start_position, transform.position) >= max_range)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 시 폭발
        Explode();
    }

    private void Explode()
    {
        // 폭발 이펙트 생성
        if(null != explosion_effect)
        {
            Instantiate(explosion_effect, transform.position, Quaternion.identity);
        }

        // 오브젝트 제거 대신 Kill() 호출
        if (null != rewindable)
            rewindable.Kill();
        else
            gameObject.SetActive(false);
    }
}
