using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [SerializeField] int orbsCollected = 0;
    [SerializeField] int orbsNeededForMemory = 5;

    public int OrbsCollected => orbsCollected;

    [Header("Speed Settings")]
    [SerializeField] float baseSpeed = 6f;
    [SerializeField] public float speed;
    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float maxSpeed = 25f;

    [Header("Game State")]
    [SerializeField] int score = 0;
    [SerializeField] bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;

    //[Header("Progress Settings")]
    public float levelLength = 500f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        speed = baseSpeed;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        StartCoroutine(ScoreAndDifficultyLoop());
    }

    IEnumerator ScoreAndDifficultyLoop()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(1f);
            score += Mathf.RoundToInt(speed);
            speed = Mathf.Min(maxSpeed, speed + acceleration);
        }
    }


    public void CollectOrb(int amount)
    {
        if (isGameOver) return;

        orbsCollected += amount;
        Debug.Log("Orb collected! Total: " + orbsCollected);

        if (orbsCollected >= orbsNeededForMemory)
        {
            UnlockMemory();
            orbsCollected = 0; // reset or keep? (Choose based on game design)
        }
    }

    void UnlockMemory()
    {
        Debug.Log("MEMORY UNLOCKED!");

        // TODO: play animation / open memory UI / start cutscene
        // Keep lightweight and non-blocking

        // Example minimal placeholder:
        // UIManager.Instance.ShowMemory();  <- if you add UI later
    }

    public void Fail(string reason = "")
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("Player failed: " + reason);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
