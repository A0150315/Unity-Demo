using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理Home场景中的高分文本显示
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text scoreText;

    private int score = 0;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateHighScore();
        UpdateScoreText();
    }

    /// <summary>
    /// 更新高分显示
    /// </summary>
    public void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = highScore.ToString();
        }
    }

    /// <summary>
    /// 设置当前分数并更新显示
    /// </summary>
    public void SetScore(int newScore)
    {
        score = newScore;
        PlayerPrefs.SetInt("Score", score);
        UpdateScoreText();

        // 检查是否创造新高分
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            UpdateHighScore();
        }
    }

    /// <summary>
    /// 更新当前分数显示
    /// </summary>
    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    // 在应用暂停或退出时保存数据
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}