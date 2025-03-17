using UnityEngine;

public class CubeFollowCameraMovement : MonoBehaviour
{
    // �����������
    public Camera mainCamera;
    // ��¼������ĳ�ʼλ��
    private Vector3 initialCameraPosition;
    // ��¼ Cube �ĳ�ʼλ��
    private Vector3 initialCubePosition;
    // �ڶ��ٶ�
    public float swingSpeed = 2f;
    // �ڶ���Χ���Գ�ʼλ��Ϊ���ģ����Ұڶ��������룩
    public float swingRange = 1f;
    // ���ڼ�¼�ڶ���ʱ��
    private float swingTime = 0f;

    void Start()
    {
        // ���û���ֶ�ָ���������Ĭ��ʹ���������
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // ��¼������� Cube �ĳ�ʼλ��
        initialCameraPosition = mainCamera.transform.position;
        initialCubePosition = transform.position;
    }

    void Update()
    {
        // ����������ӳ�ʼλ�õ���ǰλ�õ�ƫ����
        Vector3 cameraOffset = mainCamera.transform.position - initialCameraPosition;

        // ����ڶ���ƫ����
        swingTime += Time.deltaTime;
        float swingOffset = Mathf.Sin(swingTime * swingSpeed) * swingRange;
        Vector3 swingVector = transform.right * swingOffset;

        // �����������ƫ�����Ͱڶ�ƫ�������� Cube ��λ��
        transform.position = initialCubePosition + cameraOffset + swingVector;
    }
}