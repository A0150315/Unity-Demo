using UnityEngine;

public class BlockSwing : MonoBehaviour
{
    public float swingSpeed = 2f; // �ڶ��ٶ�
    public float swingRange = 2f; // �ڶ���Χ
    private float initialX; // �����ʼ�� X ����

    void Start()
    {
        // ��¼�����ʼ�� X ����
        initialX = transform.position.x;
    }

    void Update()
    {
        // ���㷽���µ� X ���꣬ʹ�����Һ���ʵ�����Ұڶ�Ч��
        float newX = initialX + Mathf.Sin(Time.time * swingSpeed) * swingRange;
        // ���·����λ��
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}