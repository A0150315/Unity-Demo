using UnityEngine;
using System;

public class HomeBlockCollisionHandler : MonoBehaviour
{
    private GameManager gameManager;
    private bool isLocked = false;
    private float snapThreshold = 0.4f;        // 吸附阈值
    private Rigidbody rb;
    private Collider myCollider;
    private float placementTime; // 记录方块放置的时间

    void Start()
    {
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        placementTime = Time.time; // 记录生成时间
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocked) return;

        GameObject otherBlock = collision.gameObject;
        HomeBlockCollisionHandler otherHandler = otherBlock.GetComponent<HomeBlockCollisionHandler>();

        // 检查是否与另一个方块碰撞
        if (otherBlock.CompareTag("Cube") && !isLocked)
        {
            // 如果对方方块已经锁定，或者是较早放置的方块，则当前方块应该移动
            bool shouldMove = true;

            if (otherHandler != null)
            {
                // 如果对方方块未锁定，并且是较新放置的方块，那么对方应该移动，当前方块不移动
                if (!otherHandler.IsLocked() && otherHandler.GetPlacementTime() > placementTime)
                {
                    shouldMove = false;
                }
            }

            if (shouldMove)
            {
                // 计算相对位置
                float verticalDistance = Math.Abs(transform.position.x - otherBlock.transform.position.x);

                // 确保当前方块在上方才进行吸附
                if (transform.position.y > otherBlock.transform.position.y &&
                    verticalDistance < snapThreshold)
                {
                    StartCoroutine(AlignAndLockBlock(otherBlock));
                }
            }
        }

        if (gameManager != null)
        {
            gameManager.OnBlockCollision(collision);
        }
        else
        {
            // 如果GameManager未找到，尝试再次查找
            gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnBlockCollision(collision);
            }
        }
    }

    // 公开方法，用于其他方块检查此方块是否已锁定
    public bool IsLocked()
    {
        return isLocked;
    }

    // 获取方块放置时间
    public float GetPlacementTime()
    {
        return placementTime;
    }

    private System.Collections.IEnumerator AlignAndLockBlock(GameObject targetBlock)
    {
        isLocked = true;

        // 立即禁用物理和碰撞检测，防止抖动
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 禁用碰撞器，防止继续触发物理反应
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // 等待一帧确保物理系统更新
        yield return null;

        // 计算目标位置（在目标方块正上方）
        Vector3 targetPosition = targetBlock.transform.position + Vector3.up * (targetBlock.transform.localScale.y);
        Quaternion targetRotation = Quaternion.identity;

        // 直接设置位置，避免使用Lerp产生的中间状态
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        // 等待一帧
        yield return null;

        // 重新启用碰撞器，但保持物体为运动学状态
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }

        // 确保物理组件保持锁定状态
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // 通知游戏管理器方块已固定
        if (gameManager != null)
        {
            // gameManager.OnBlockPlaced(gameObject);
        }
    }
}