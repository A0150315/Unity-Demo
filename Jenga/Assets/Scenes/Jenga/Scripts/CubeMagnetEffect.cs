using UnityEngine;

public class CubeMagnetEffect : MonoBehaviour
{
    public float magnetRange = 2f; // 磁铁的有效范围
    public float magnetForce = 5f; // 磁铁的吸引力大小

    private Rigidbody rb;

    void Start()
    {
        // 获取方块的刚体组件
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 查找场景中所有带有刚体组件的方块
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag("Cube");

        foreach (GameObject otherCube in allCubes)
        {
            // 排除自身方块
            if (otherCube != gameObject)
            {
                // 计算两个方块之间的距离
                float distance = Vector3.Distance(transform.position, otherCube.transform.position);

                // 如果距离在磁铁有效范围内
                if (distance < magnetRange)
                {
                    // 计算吸引力的方向
                    Vector3 direction = (otherCube.transform.position - transform.position).normalized;

                    // 施加吸引力
                    rb.AddForce(direction * magnetForce * (1 - distance / magnetRange));
                }
            }
        }
    }
}