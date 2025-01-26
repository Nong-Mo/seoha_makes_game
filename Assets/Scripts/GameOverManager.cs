using UnityEngine;
using TMPro; // TextMeshPro ���
using UnityEngine.SceneManagement; // �� ��ȯ��

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI resultText; // ���� �ð� �ؽ�Ʈ
    public TextMeshProUGUI mentText;   // ��Ʈ �ؽ�Ʈ

    void Start()
    {
        // PlayerPrefs���� ���� �ð� ��������
        float survivalTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

        // ��� �ؽ�Ʈ ����
        resultText.text = $"{Mathf.FloorToInt(survivalTime)}�� ����";

        // ��Ʈ �ؽ�Ʈ ����
        if (survivalTime >= 60f)
        {
            mentText.text = "���Ƴ½��ϴ�!";
        }
        else
        {
            mentText.text = "���㰡 �Ʊ���.";
        }
    }

    // �絵�� ��ư Ŭ�� �� ȣ��
    public void RestartGame()
    {
        // ���� ���� �ε��ϱ� ���� �ð� ������ ����
        Time.timeScale = 1f;
        SceneManager.LoadScene("MonsterScene");
    }
}
