using UnityEngine;

public class CubeFollowCameraMovement : MonoBehaviour
{
    // 引用主摄像机
    public Camera mainCamera;
    // 记录摄像机的初始位置
    private Vector3 initialCameraPosition;
    // 记录 Cube 的初始位置
    private Vector3 initialCubePosition;
    // 摆动速度
    public float swingSpeed = 2f;
    // 摆动范围（以初始位置为中心，左右摆动的最大距离）
    public float swingRange = 1f;
    // 用于记录摆动的时间
    private float swingTime = 0f;

    void Start()
    {
        // 如果没有手动指定摄像机，默认使用主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 记录摄像机和 Cube 的初始位置
        initialCameraPosition = mainCamera.transform.position;
        initialCubePosition = transform.position;
    }

    void Update()
    {
        // 计算摄像机从初始位置到当前位置的偏移量
        Vector3 cameraOffset = mainCamera.transform.position - initialCameraPosition;

        // 计算摆动的偏移量
        swingTime += Time.deltaTime;
        float swingOffset = Mathf.Sin(swingTime * swingSpeed) * swingRange;
        Vector3 swingVector = transform.right * swingOffset;

        // 根据摄像机的偏移量和摆动偏移量更新 Cube 的位置
        transform.position = initialCubePosition + cameraOffset + swingVector;
    }
}