using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 要跟随的目标（方块）
    public float smoothSpeed = 0.125f; // 相机跟随的平滑速度
    public Vector3 offset; // 相机与目标之间的偏移量

    void LateUpdate()
    {
        if (target != null)
        {
            // 计算相机应该移动到的目标位置
            Vector3 desiredPosition = target.position + offset;
            // 使用平滑阻尼来移动相机到目标位置
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // 让相机始终看向目标
            transform.LookAt(target);
        }
    }
}