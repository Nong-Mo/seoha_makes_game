using UnityEngine;
using TMPro; // TextMeshPro ���
using System.Collections;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;  // ���� ������
    public Transform[] spawnPoints;  // ���� ���� ��ġ��

    public Canvas waveCanvas;        // Wave �˸��� ĵ����
    public TextMeshProUGUI waveText; // Wave �ؽ�Ʈ
    public TextMeshProUGUI timerText; // �� ī��Ʈ �ؽ�Ʈ
    public float alertDuration = 2f; // �˸� ǥ�� �ð�

    public int maxWave = 5;              // �� ���̺� ��
    public float gameDuration = 60f;    // ���� ��ü �ð� (1��)
    public float spawnIntervalDecrease = 0.5f; // ���̺� �� ���� ���� ����
    public int additionalMonstersPerWave = 1;  // ���̺긶�� �߰� ���� �� ����

    private float waveDuration;          // ���̺� �� ���� �ð�
    private float spawnInterval = 5f;    // �ʱ� ���� ����
    private int monstersPerSpawn = 1;    // �ʱ� ���� ���� ��
    private int currentWave = 0;         // ���� ���̺�
    private bool gameOver = false;       // ���� ���� ����
    private float elapsedTime = 0f;      // ���� ���� �ð�

    void Start()
    {
        if (waveCanvas != null)
        {
            waveCanvas.gameObject.SetActive(false); // �˸� ĵ���� ��Ȱ��ȭ
        }

        // ���̺� �� ���� �ð� ���
        waveDuration = gameDuration / maxWave;

        // Ÿ�̸� �ʱ�ȭ
        if (timerText != null)
        {
            timerText.text = "0��";
        }

        // ���� ����
        StartCoroutine(GameManager());
    }

    void Update()
    {
        if (!gameOver)
        {
            // �� ī��Ʈ ������Ʈ
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

            // ���̺� ���� �˸�
            StartCoroutine(ShowWaveAlert($"Wave {currentWave} ����!"));
            Debug.Log($"Wave {currentWave}: Spawning monsters!");

            // ���̺� ����
            yield return StartCoroutine(SpawnWave());

            // ���̺� ���̵� ����
            spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnIntervalDecrease);
            monstersPerSpawn += additionalMonstersPerWave;
        }

        if (!gameOver)
        {
            // �¸� ó��
            TriggerWinner();
        }
    }

    IEnumerator SpawnWave()
    {
        float timeRemaining = waveDuration;

        while (timeRemaining > 0 && !gameOver)
        {
            // ���� ����
            for (int i = 0; i < monstersPerSpawn; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
            }

            // ���
            yield return new WaitForSeconds(spawnInterval);

            timeRemaining -= spawnInterval;
        }
    }

    IEnumerator ShowWaveAlert(string message)
    {
        // �˸��� ĵ���� Ȱ��ȭ �� �ؽ�Ʈ ����
        waveCanvas.gameObject.SetActive(true);
        waveText.text = message;

        // �˸� ����
        yield return new WaitForSeconds(alertDuration);

        // �˸��� ĵ���� ��Ȱ��ȭ
        waveCanvas.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (!gameOver)
        {
            // �й� ó��
            StartCoroutine(EndGame("Game Over!"));
            Debug.Log("Game Over!");
            gameOver = true;
        }
    }

    public void TriggerWinner()
    {
        if (!gameOver)
        {
            // �¸� ó��
            StartCoroutine(EndGame("You Win!"));
            Debug.Log("You Win!");
            gameOver = true;
        }
    }

    IEnumerator EndGame(string resultMessage)
    {
        // ���� ����
        Time.timeScale = 0;

        // �� ī��Ʈ ����
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // ��� �޽��� ǥ��
        StartCoroutine(ShowWaveAlert(resultMessage));

        // �� �� ��� (UI ǥ��)
        yield return new WaitForSecondsRealtime(3f);

        // ���� �ð� ����
        PlayerPrefs.SetFloat("SurvivalTime", elapsedTime);

        // �� ��ȯ
        SceneManager.LoadScene("GameOverScene");
    }
}
