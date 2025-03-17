using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("游戏UI")]
    public GameObject gameOverUI;
    public GameObject collisionPopupUI;
    public Text scoreText;
    public Text gameOverScoreText;
    public Text countdownText;

    [Header("按钮")]
    public Button startGameButton;
    public Button continueButton;
    public Button alignBlocksButton;
    public Button removeBlocksButton;
    public Button adsorbAlignButton;
    public Button autoAdsorbButton;

    [Header("其他UI元素")]
    public Text autoAdsorbCountdownText;

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
        // 将UI元素引用传递给相关管理器
        if (BlockAlignmentManager.Instance != null)
        {
            BlockAlignmentManager.Instance.alignBlocksButton = alignBlocksButton;
            BlockAlignmentManager.Instance.secondAlignBlocksButton = removeBlocksButton;
            BlockAlignmentManager.Instance.adsorbAlignButton = adsorbAlignButton;
            BlockAlignmentManager.Instance.autoAdsorbButton = autoAdsorbButton;
            BlockAlignmentManager.Instance.autoAdsorbCountdownText = autoAdsorbCountdownText;
        }

        // 设置初始UI状态
        HideAllGameplayUI();

        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(true);
        }
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "得分: " + score.ToString();
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = "恭喜你！获得: " + finalScore;
            }

            if (startGameButton != null)
            {
                startGameButton.gameObject.SetActive(false);
            }

            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void ShowCollisionPopup()
    {
        if (collisionPopupUI != null)
        {
            collisionPopupUI.SetActive(true);
        }
    }

    public void HideCollisionPopup()
    {
        if (collisionPopupUI != null)
        {
            collisionPopupUI.SetActive(false);
        }
    }

    public void HideAllGameplayUI()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (collisionPopupUI != null)
        {
            collisionPopupUI.SetActive(false);
        }

        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    public void ShowAutoAdsorbCountdown(string countdownValue)
    {
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(true);
            autoAdsorbCountdownText.text = countdownValue;
        }
    }

    public void HideAutoAdsorbCountdown()
    {
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    public void OnGameStarted()
    {
        // 隐藏开始按钮
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(false);
        }

        // 初始显示倒计时
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // 其他UI元素的初始状态设置
    }

    // 更新倒计时显示
    public void UpdateCountdown(int seconds)
    {
        if (countdownText != null)
        {
            countdownText.text = seconds.ToString();
        }
    }

    // 倒计时结束时调用
    public void OnCountdownFinished()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // 可能需要显示其他游戏UI元素
        // 例如：scoreText.gameObject.SetActive(true);
    }
}