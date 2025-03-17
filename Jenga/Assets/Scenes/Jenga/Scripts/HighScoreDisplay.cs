using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Text highScoreText; // ������ʾ��߷ֵ� UI Text ���

    void Start()
    {
        // �� PlayerPrefs �ж�ȡ��߷�
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        // ���� UI ��ʾ
        if (highScoreText != null)
        {
            highScoreText.text = "��߷�: " + highScore;
        }
    }
}