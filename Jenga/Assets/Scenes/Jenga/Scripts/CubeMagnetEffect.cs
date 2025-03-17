using UnityEngine;

public class CubeMagnetEffect : MonoBehaviour
{
    public float magnetRange = 2f; // ��������Ч��Χ
    public float magnetForce = 5f; // ��������������С

    private Rigidbody rb;

    void Start()
    {
        // ��ȡ����ĸ������
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ���ҳ��������д��и�������ķ���
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag("Cube");

        foreach (GameObject otherCube in allCubes)
        {
            // �ų�������
            if (otherCube != gameObject)
            {
                // ������������֮��ľ���
                float distance = Vector3.Distance(transform.position, otherCube.transform.position);

                // ��������ڴ�����Ч��Χ��
                if (distance < magnetRange)
                {
                    // �����������ķ���
                    Vector3 direction = (otherCube.transform.position - transform.position).normalized;

                    // ʩ��������
                    rb.AddForce(direction * magnetForce * (1 - distance / magnetRange));
                }
            }
        }
    }
}