using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;    // �ִ� HP
    private int currentHP;   // ���� HP

    private MonsterSpawner spawner; // MonsterSpawner ����

    void Start()
    {
        // ���� HP�� �ִ� HP�� ����
        currentHP = maxHP;

        // MonsterSpawner ã��
        spawner = FindObjectOfType<MonsterSpawner>();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log($"Player HP: {currentHP}");

        if (currentHP <= 0)
        {
            // HP�� 0 ���� => ���� ����
            spawner.TriggerGameOver();
        }
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
    }
}
