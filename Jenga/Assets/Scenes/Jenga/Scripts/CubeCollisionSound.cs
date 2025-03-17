using UnityEngine;

public class CubeCollisionSound : MonoBehaviour
{
    // ���� AudioSource ��������ڲ�������
    private AudioSource audioSource;
    // ��������Ƿ��Ѿ����Ź�
    private bool hasPlayedSound = false;
    // ��һ��ָ���� Cube
    public GameObject cube1;
    // �ڶ���ָ���� Cube
    public GameObject cube2;
    // ��Ƶ����
    public AudioClip collisionSoundClip;

    void Start()
    {
        // ��ȡ��ǰ Cube �ϵ� AudioSource ���
        audioSource = GetComponent<AudioSource>();
        // ���û�� AudioSource ����������һ��
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // ������Ƶ����
        if (collisionSoundClip != null)
        {
            audioSource.clip = collisionSoundClip;
        }
    }

    // ��ײ��ʼʱ���ô˷���
    private void OnCollisionEnter(Collision collision)
    {
        // ��� cube1 �� cube2 �Ƿ�Ϊ��
        if (cube1 == null || cube2 == null)
        {
            Debug.LogError("cube1 or cube2 is not assigned!");
            return;
        }

        // ����Ƿ���ָ�������� Cube �໥��ײ
        bool isSpecialCollision = (gameObject == cube1 && collision.gameObject == cube2) ||
                                  (gameObject == cube2 && collision.gameObject == cube1);

        // �����ָ�������� Cube �໥��ײ����������δ���Ź������� AudioSource ����Ƶ����������
        if (isSpecialCollision && !hasPlayedSound && audioSource != null && audioSource.clip != null)
        {
            // ��������
            audioSource.Play();
            // ��������Ѳ���
            hasPlayedSound = true;
        }
    }
}