using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Text highScoreText; // 用于显示最高分的 UI Text 组件

    void Start()
    {
        // 从 PlayerPrefs 中读取最高分
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        // 更新 UI 显示
        if (highScoreText != null)
        {
            highScoreText.text = "最高分: " + highScore;
        }
    }
}