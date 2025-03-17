using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Ҫ�����Ŀ�꣨���飩
    public float smoothSpeed = 0.125f; // ��������ƽ���ٶ�
    public Vector3 offset; // �����Ŀ��֮���ƫ����

    void LateUpdate()
    {
        if (target != null)
        {
            // �������Ӧ���ƶ�����Ŀ��λ��
            Vector3 desiredPosition = target.position + offset;
            // ʹ��ƽ���������ƶ������Ŀ��λ��
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // �����ʼ�տ���Ŀ��
            transform.LookAt(target);
        }
    }
}