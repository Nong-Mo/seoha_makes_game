using UnityEngine;
using TMPro; // TextMeshPro 사용
using System.Collections;
using UnityEngine.SceneManagement; // 씬 전환에 사용

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;  // 몬스터 프리팹
    public Transform[] spawnPoints;  // 몬스터 스폰 위치들

    public Canvas waveCanvas;        // Wave 알림용 캔버스
    public TextMeshProUGUI waveText; // Wave 텍스트
    public TextMeshProUGUI timerText; // 초 카운트 텍스트
    public float alertDuration = 2f; // 알림 표시 시간

    public int maxWave = 5;              // 총 웨이브 수
    public float gameDuration = 60f;    // 게임 전체 시간 (1분)
    public float spawnIntervalDecrease = 0.5f; // 웨이브 간 스폰 간격 감소
    public int additionalMonstersPerWave = 1;  // 웨이브마다 추가 스폰 수 증가

    private float waveDuration;          // 웨이브 당 지속 시간
    private float spawnInterval = 5f;    // 초기 스폰 간격
    private int monstersPerSpawn = 1;    // 초기 스폰 몬스터 수
    private int currentWave = 0;         // 현재 웨이브
    private bool gameOver = false;       // 게임 종료 상태
    private float elapsedTime = 0f;      // 게임 진행 시간

    void Start()
    {
        if (waveCanvas != null)
        {
            waveCanvas.gameObject.SetActive(false); // 알림 캔버스 비활성화
        }

        // 웨이브 당 지속 시간 계산
        waveDuration = gameDuration / maxWave;

        // 타이머 초기화
        if (timerText != null)
        {
            timerText.text = "0초";
        }

        // 게임 시작
        StartCoroutine(GameManager());
    }

    void Update()
    {
        if (!gameOver)
        {
            // 초 카운트 업데이트
            elapsedTime += Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"{Mathf.FloorToInt(elapsedTime)}s";
            }
        }
    }

    IEnumerator GameManager()
    {
        while (currentWave < maxWave && !gameOver)
        {
            currentWave++;

            // 웨이브 시작 알림
            StartCoroutine(ShowWaveAlert($"Wave {currentWave} 시작!"));
            Debug.Log($"Wave {currentWave}: Spawning monsters!");

            // 웨이브 진행
            yield return StartCoroutine(SpawnWave());

            // 웨이브 난이도 증가
            spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnIntervalDecrease);
            monstersPerSpawn += additionalMonstersPerWave;
        }

        if (!gameOver)
        {
            // 승리 처리
            TriggerWinner();
        }
    }

    IEnumerator SpawnWave()
    {
        float timeRemaining = waveDuration;

        while (timeRemaining > 0 && !gameOver)
        {
            // 몬스터 스폰
            for (int i = 0; i < monstersPerSpawn; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
            }

            // 대기
            yield return new WaitForSeconds(spawnInterval);

            timeRemaining -= spawnInterval;
        }
    }

    IEnumerator ShowWaveAlert(string message)
    {
        // 알림용 캔버스 활성화 및 텍스트 설정
        waveCanvas.gameObject.SetActive(true);
        waveText.text = message;

        // 알림 유지
        yield return new WaitForSeconds(alertDuration);

        // 알림용 캔버스 비활성화
        waveCanvas.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (!gameOver)
        {
            // 패배 처리
            StartCoroutine(EndGame("Game Over!"));
            Debug.Log("Game Over!");
            gameOver = true;
        }
    }

    public void TriggerWinner()
    {
        if (!gameOver)
        {
            // 승리 처리
            StartCoroutine(EndGame("You Win!"));
            Debug.Log("You Win!");
            gameOver = true;
        }
    }

    IEnumerator EndGame(string resultMessage)
    {
        // 게임 정지
        Time.timeScale = 0;

        // 초 카운트 멈춤
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // 결과 메시지 표시
        StartCoroutine(ShowWaveAlert(resultMessage));

        // 몇 초 대기 (UI 표시)
        yield return new WaitForSecondsRealtime(3f);

        // 생존 시간 저장
        PlayerPrefs.SetFloat("SurvivalTime", elapsedTime);

        // 씬 전환
        SceneManager.LoadScene("GameOverScene");
    }
}
