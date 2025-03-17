using UnityEngine;

public class CubeCollisionSound : MonoBehaviour
{
    // 引用 AudioSource 组件，用于播放声音
    private AudioSource audioSource;
    // 标记声音是否已经播放过
    private bool hasPlayedSound = false;
    // 第一个指定的 Cube
    public GameObject cube1;
    // 第二个指定的 Cube
    public GameObject cube2;
    // 音频剪辑
    public AudioClip collisionSoundClip;

    void Start()
    {
        // 获取当前 Cube 上的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        // 如果没有 AudioSource 组件，则添加一个
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 设置音频剪辑
        if (collisionSoundClip != null)
        {
            audioSource.clip = collisionSoundClip;
        }
    }

    // 碰撞开始时调用此方法
    private void OnCollisionEnter(Collision collision)
    {
        // 检查 cube1 和 cube2 是否为空
        if (cube1 == null || cube2 == null)
        {
            Debug.LogError("cube1 or cube2 is not assigned!");
            return;
        }

        // 检查是否是指定的两个 Cube 相互碰撞
        bool isSpecialCollision = (gameObject == cube1 && collision.gameObject == cube2) ||
                                  (gameObject == cube2 && collision.gameObject == cube1);

        // 如果是指定的两个 Cube 相互碰撞，且声音还未播放过，并且 AudioSource 和音频剪辑都存在
        if (isSpecialCollision && !hasPlayedSound && audioSource != null && audioSource.clip != null)
        {
            // 播放声音
            audioSource.Play();
            // 标记声音已播放
            hasPlayedSound = true;
        }
    }
}