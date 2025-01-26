using UnityEngine;
using TMPro; // TextMeshPro 사용
using UnityEngine.SceneManagement; // 씬 전환용

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI resultText; // 생존 시간 텍스트
    public TextMeshProUGUI mentText;   // 멘트 텍스트

    void Start()
    {
        // PlayerPrefs에서 생존 시간 가져오기
        float survivalTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);

        // 결과 텍스트 설정
        resultText.text = $"{Mathf.FloorToInt(survivalTime)}초 생존";

        // 멘트 텍스트 설정
        if (survivalTime >= 60f)
        {
            mentText.text = "막아냈습니다!";
        }
        else
        {
            mentText.text = "폐허가 됐군요.";
        }
    }

    // 재도전 버튼 클릭 시 호출
    public void RestartGame()
    {
        // 씬을 새로 로드하기 전에 시간 스케일 리셋
        Time.timeScale = 1f;
        SceneManager.LoadScene("MonsterScene");
    }
}
