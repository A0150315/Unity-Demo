using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    public bool isGameStarted = false;
    public int score = 0;
    private float startDelay = 4f;
    private float startTime;
    
    [Header("UI元素")]
    public GameObject gameOverUI;
    public Button startGameButton;
    public Button continueButton;
    public GameObject collisionPopupUI;
    
    [Header("音效")]
    public AudioClip failureSound;
    private AudioSource audioSource;
    
    [Header("引用")]
    public HomeBlockSpawner blockSpawner;
    public UIController uiController;
    
    private List<GameObject> fallenBlocks = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeUI();
        isGameStarted = false;
    }
    
    private void InitializeUI()
    {
        gameOverUI.SetActive(false);
        collisionPopupUI.SetActive(false);
        startGameButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        startGameButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
    }
    
    void StartGame()
    {
        ResetGame();
        isGameStarted = true;
        startGameButton.gameObject.SetActive(false);
        startTime = Time.time;
    }
    
    public void ContinueGame()
    {
        gameOverUI.SetActive(false);
        collisionPopupUI.SetActive(false);
        continueButton.gameObject.SetActive(false);
        ClearFallenBlocks();
        isGameStarted = true;
        startTime = Time.time;
    }
    
    private void ResetGame()
    {
        score = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }
        ClearFallenBlocks();
    }
    
    public void IncreaseScore()
    {
        score++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Text gameOverText = gameOverUI.GetComponentInChildren<Text>();
        if (gameOverText != null)
        {
            gameOverText.text = "恭喜你！获得: " + score;
        }
        isGameStarted = false;
        startGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

        score = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }
    }
    
    public void OnBlockCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("FailCube"))
        {
            PlayFailureSound();
            GameOver();
        }
    }
    
    private void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.clip = failureSound;
            audioSource.Play();
        }
    }
    
    public void AddFallenBlock(GameObject block)
    {
        fallenBlocks.Add(block);
    }
    
    private void ClearFallenBlocks()
    {
        foreach (GameObject block in fallenBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        fallenBlocks.Clear();
    }
    
    public bool CanSpawnBlock()
    {
        return isGameStarted && Time.time - startTime >= startDelay;
    }
} 