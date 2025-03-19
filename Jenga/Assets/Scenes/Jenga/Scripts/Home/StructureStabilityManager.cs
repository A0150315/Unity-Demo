using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StructureStabilityManager : MonoBehaviour
{
    public static StructureStabilityManager Instance { get; private set; }

    [Header("摇晃设置")]
    [SerializeField] private int topBlocksToShake = 3;        // 顶部要摇晃的方块数量
    [SerializeField] private float maxShakeAngle = 3f;        // 最大摇晃角度
    [SerializeField] private float shakeSpeed = 1.5f;         // 摇晃速度
    [SerializeField] private float heightThreshold = 5f;      // 开始摇晃的高度阈值
    [SerializeField] private float transitionDuration = 0.5f; // 过渡动画持续时间

    private HomeBlockSpawner blockSpawner;
    private List<GameObject> shakingBlocks = new List<GameObject>();
    private GameObject pivotBlock;  // 作为旋转中心的方块
    private Vector3 pivotOriginalPosition; // 旋转中心的原始位置
    private List<Vector3> relativePositions = new List<Vector3>(); // 其他方块相对于旋转中心的位置

    // 平滑过渡变量
    private List<GameObject> previousShakingBlocks = new List<GameObject>();
    private Vector3 previousPivotPosition;
    private List<Vector3> previousRelativePositions = new List<Vector3>();
    private float transitionProgress = 1.0f; // 1.0表示过渡完成

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        blockSpawner = FindObjectOfType<HomeBlockSpawner>();
        InvokeRepeating("UpdateShakingBlocks", 0.5f, 0.3f); // 每1秒更新一次摇晃的方块，延长间隔减少抖动
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameStarted) return;

        // 更新过渡进度
        if (transitionProgress < 1.0f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            if (transitionProgress > 1.0f)
            {
                transitionProgress = 1.0f;
            }
        }

        ApplyShakingEffect();
    }

    // 比较两个方块列表是否相同
    private bool AreBlockListsEqual(List<GameObject> list1, List<GameObject> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
        {
            // 如果任何一个对象不在另一个列表中，则列表不相同
            if (!list2.Contains(list1[i]))
                return false;
        }

        return true;
    }

    // 更新需要摇晃的顶部方块
    private void UpdateShakingBlocks()
    {
        if (blockSpawner == null
        || GameManager.Instance == null
        || !GameManager.Instance.isGameStarted
        || blockSpawner.GetIsBlockFalling()
        ) return;

        // 获取所有已放置的方块
        List<GameObject> allBlocks = blockSpawner.GetAllBlocks();
        if (allBlocks.Count == 0) return;

        // 在开始更新之前，保存当前的摇晃状态用于平滑过渡
        previousShakingBlocks = new List<GameObject>(shakingBlocks);
        if (pivotBlock != null)
        {
            previousPivotPosition = pivotOriginalPosition;
            previousRelativePositions = new List<Vector3>(relativePositions);
        }

        // 恢复所有方块的原始位置
        ResetAllBlocks();

        // 清除之前的摇晃状态
        shakingBlocks.Clear();
        relativePositions.Clear();
        pivotBlock = null;

        // 检查塔是否高于阈值
        float towerHeight = blockSpawner.GetStackHeight();
        if (towerHeight < heightThreshold) return;

        // 根据高度排序方块（Y轴位置最高的在前）
        var sortedBlocks = allBlocks
            .Where(b => b != null && b.GetComponent<HomeBlockCollisionHandler>() != null && b.GetComponent<HomeBlockCollisionHandler>().IsLocked())
            .OrderByDescending(b => b.transform.position.y)
            .ToList();

        if (sortedBlocks.Count == 0) return;

        // 选择顶部的几个方块进行摇晃
        int blocksToShake = Mathf.Min(topBlocksToShake, sortedBlocks.Count);
        if (blocksToShake <= 0) return;

        // 设置旋转中心为最底部的晃动方块
        pivotBlock = sortedBlocks[blocksToShake - 1];
        pivotOriginalPosition = pivotBlock.transform.position;

        // 添加所有要晃动的方块
        for (int i = 0; i < blocksToShake; i++)
        {
            GameObject block = sortedBlocks[i];
            shakingBlocks.Add(block);

            // 记录每个方块相对于旋转中心的位置
            relativePositions.Add(block.transform.position - pivotOriginalPosition);
        }

        // 如果方块组合发生变化，启动过渡
        if (!AreBlockListsEqual(previousShakingBlocks, shakingBlocks) && previousShakingBlocks.Count > 0)
        {
            Debug.Log("方块组合发生变化，启动平滑过渡");
            transitionProgress = 0f;
        }
    }

    // 应用摇晃效果 - 以底部晃动方块为中心，整体摇晃，带平滑过渡
    private void ApplyShakingEffect()
    {
        if (shakingBlocks.Count == 0 && previousShakingBlocks.Count == 0)
            return;

        // 计算当前的摇晃角度 - 只在Z轴方向摇晃（左右摇晃）
        float zAngle = Mathf.Sin(Time.time * shakeSpeed) * maxShakeAngle;

        // 创建一个以pivotBlock位置为中心的旋转矩阵
        Quaternion rotation = Quaternion.Euler(0, 0, zAngle);

        // 正在进行过渡
        if (transitionProgress < 1.0f && previousShakingBlocks.Count > 0)
        {
            // 处理之前的方块组（使用渐变淡出效果）
            for (int i = 0; i < previousShakingBlocks.Count; i++)
            {
                GameObject block = previousShakingBlocks[i];
                if (block == null) continue;

                // 检查此方块是否也在当前摇晃列表中
                if (shakingBlocks.Contains(block)) continue;

                // 计算旧的旋转位置
                Vector3 oldRotatedPosition = previousPivotPosition + (rotation * previousRelativePositions[i]);

                // 恢复原始位置（线性插值）
                Vector3 originalPosition = previousPivotPosition + previousRelativePositions[i];
                block.transform.position = Vector3.Lerp(oldRotatedPosition, originalPosition, transitionProgress);

                // 恢复原始旋转（线性插值）
                block.transform.rotation = Quaternion.Slerp(rotation, Quaternion.identity, transitionProgress);
            }
        }

        // 处理当前的方块组
        if (shakingBlocks.Count > 0 && pivotBlock != null)
        {
            // 应用旋转到所有方块
            for (int i = 0; i < shakingBlocks.Count; i++)
            {
                GameObject block = shakingBlocks[i];
                if (block == null) continue;

                // 计算旋转后的位置（以pivotBlock为中心旋转）
                Vector3 rotatedPosition = pivotOriginalPosition + (rotation * relativePositions[i]);

                // 如果在过渡中，则进行插值
                if (transitionProgress < 1.0f && previousShakingBlocks.Contains(block))
                {
                    // 查找在旧列表中的索引
                    int oldIndex = previousShakingBlocks.IndexOf(block);
                    if (oldIndex >= 0 && oldIndex < previousRelativePositions.Count)
                    {
                        // 计算旧的旋转位置
                        Vector3 oldRotatedPosition = previousPivotPosition + (rotation * previousRelativePositions[oldIndex]);

                        // 在旧位置和新位置之间进行插值
                        block.transform.position = Vector3.Lerp(oldRotatedPosition, rotatedPosition, transitionProgress);
                    }
                    else
                    {
                        block.transform.position = rotatedPosition;
                    }
                }
                else
                {
                    // 设置方块的位置
                    block.transform.position = rotatedPosition;
                }

                // 应用相同的旋转到方块自身
                block.transform.rotation = rotation;
            }
        }
    }

    // 重置所有方块到原始状态
    private void ResetAllBlocks()
    {
        if (pivotBlock == null) return;

        for (int i = 0; i < shakingBlocks.Count; i++)
        {
            GameObject block = shakingBlocks[i];
            if (block == null) continue;

            // 恢复原始位置
            block.transform.position = pivotOriginalPosition + relativePositions[i];

            // 恢复原始旋转
            block.transform.rotation = Quaternion.identity;
        }
    }

    // 在游戏结束时调用
    public void StopAllShaking()
    {
        ResetAllBlocks();
        shakingBlocks.Clear();
        relativePositions.Clear();
        pivotBlock = null;

        previousShakingBlocks.Clear();
        previousRelativePositions.Clear();
        transitionProgress = 1.0f;
    }

    public void ForceUpdateShakingBlocks()
    {
        UpdateShakingBlocks();
    }
}