using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;    // 최대 HP
    private int currentHP;   // 현재 HP

    private MonsterSpawner spawner; // MonsterSpawner 연결

    void Start()
    {
        // 현재 HP를 최대 HP로 설정
        currentHP = maxHP;

        // MonsterSpawner 찾기
        spawner = FindObjectOfType<MonsterSpawner>();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log($"Player HP: {currentHP}");

        if (currentHP <= 0)
        {
            // HP가 0 이하 => 게임 오버
            spawner.TriggerGameOver();
        }
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
    }
}
