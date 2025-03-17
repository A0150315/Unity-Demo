using UnityEngine;

public class BlockSwing : MonoBehaviour
{
    public float swingSpeed = 2f; // 摆动速度
    public float swingRange = 2f; // 摆动范围
    private float initialX; // 方块初始的 X 坐标

    void Start()
    {
        // 记录方块初始的 X 坐标
        initialX = transform.position.x;
    }

    void Update()
    {
        // 计算方块新的 X 坐标，使用正弦函数实现左右摆动效果
        float newX = initialX + Mathf.Sin(Time.time * swingSpeed) * swingRange;
        // 更新方块的位置
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}