using UnityEngine;
using UnityEngine;
using System.Collections;
using TMPro;

public class StartGame : MonoBehaviour
{
    [Header("Enemy Spawner")]
    public GameObject enemyPrefab;
    public float spawnRate = 5f;
    public float spawnRadius = 5f;
    public Transform spawnArea;
    [Header("Timer")]
    public bool countDown = true;
    public TextMeshProUGUI firstMinute;
    public TextMeshProUGUI secondMinute;
    public TextMeshProUGUI separator;
    public TextMeshProUGUI firstSecond;
    public TextMeshProUGUI secondSecond;

    private float timer;
    private float timeDuration = 3f * 60f;
    private float flashTimer;
    private float flashDuration = 1f;
    private GameObject player;
    void Update()
    {
        if (timer > 0)
            if (countDown)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    UpdateTimerDisplay(timer);
                }
                else
                {
                    Flash();
                }
            }
            else
            {
                if (timer < timeDuration)
                {
                    timer += Time.deltaTime;
                    UpdateTimerDisplay(timer);
                }
                else
                {
                    Flash();
                }
            }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == player.GetComponent<CharacterController>())
        {
            StartCoroutine(SpawnEnemy(spawnRate, enemyPrefab));
            StartTimer();
            Vector3 newPosition = transform.position;
            newPosition.y -= 10.0f;
            transform.position = newPosition;
        }
    }

    private IEnumerator SpawnEnemy(float interval, GameObject enemy)
    {
        float elapsedTime = 0f;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;

            // Increase difficulty over time by reducing the spawn interval
            float difficultyMultiplier = Mathf.Clamp01(elapsedTime / (10f * 60f)); // Difficulty increases over 10 minutes
            float adjustedInterval = Mathf.Lerp(interval, interval / 2f, difficultyMultiplier);

            Vector3 spawnPosition = spawnArea.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = spawnArea.position.y; // Ensure the enemy spawns at the same height as the spawn area
            Instantiate(enemy, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(adjustedInterval);
        }
    }

    private void StartTimer()
    {
        timer = timeDuration;
        firstMinute.enabled = true;
        secondMinute.enabled = true;
        separator.enabled = true;
        firstSecond.enabled = true;
        secondSecond.enabled = true;
    }

    private void ResetTimer()
    {
        timer = timeDuration;
    }

    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);
        firstMinute.text = currentTime[0].ToString();
        secondMinute.text = currentTime[1].ToString();
        firstSecond.text = currentTime[2].ToString();
        secondSecond.text = currentTime[3].ToString();
    }

    private void Flash()
    {
        if (countDown && timer != 0)
        {
            timer = 0;
            UpdateTimerDisplay(timer);
        }

        if (!countDown && timer != timeDuration)
        {
            timer = 0;
            UpdateTimerDisplay(timer);
        }

        if (flashTimer <= 0)
        {
            flashTimer = flashDuration;
        }
        else if (flashTimer >= flashDuration / 2)
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(false);
        }
        else
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(true);
        }
    }

    private void SetTextDisplay(bool enabled)
    {
        firstMinute.enabled = enabled;
        secondMinute.enabled = enabled;
        separator.enabled = enabled;
        firstSecond.enabled = enabled;
        secondSecond.enabled = enabled;
    }
}
