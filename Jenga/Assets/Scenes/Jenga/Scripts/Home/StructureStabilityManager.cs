using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StructureStabilityManager : MonoBehaviour
{
    public static StructureStabilityManager Instance { get; private set; }

    [Header("摇晃设置")]
    [SerializeField] private int topBlocksToShake = 3;        // 顶部要摇晃的方块数量
    [SerializeField] private float maxShakeOffset = 0.15f;    // 最大摇晃位移
    [SerializeField] private float shakeSpeed = 1.5f;         // 摇晃速度
    [SerializeField] private float heightThreshold = 1f;      // 开始摇晃的高度阈值
    [SerializeField] private float transitionDuration = 0.5f; // 过渡动画持续时间

    private HomeBlockSpawner blockSpawner;
    private List<GameObject> shakingBlocks = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>(); // 方块的原始位置

    // 平滑过渡变量
    private List<GameObject> previousShakingBlocks = new List<GameObject>();
    private List<Vector3> previousOriginalPositions = new List<Vector3>();
    private float transitionProgress = 1.0f; // 1.0表示过渡完成

    // 用于防止第一次应用效果时的突然位移
    private bool isFirstApply = true;
    private float lastXOffset = 0f;
    private float updateTimer = 0f;
    private float updateInterval = 0.3f;

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
        // InvokeRepeating("UpdateShakingBlocks", 0.5f, 0.3f); // 减少更新间隔，提高响应性
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
            if (!list2.Contains(list1[i]))
                return false;
        }

        return true;
    }

    // 在新方块被固定时立即调用此方法
    public void ForceUpdateShakingBlocks()
    {
        UpdateShakingBlocks();
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
        previousOriginalPositions = new List<Vector3>(originalPositions);

        // 清除之前的摇晃状态，但不重置方块位置
        List<GameObject> oldShakingBlocks = new List<GameObject>(shakingBlocks);
        shakingBlocks.Clear();
        originalPositions.Clear();

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

        // 添加所有要晃动的方块
        for (int i = 0; i < blocksToShake; i++)
        {
            GameObject block = sortedBlocks[i];
            shakingBlocks.Add(block);

            // 记录每个方块的当前位置（包括可能的偏移）
            // 这样可以防止突然的位置跳变
            originalPositions.Add(block.transform.position);

            // 如果方块之前不在晃动列表中，修正其原始位置（去除当前可能的偏移）
            if (!oldShakingBlocks.Contains(block))
            {
                // 计算当前可能的偏移并去除
                float currentXOffset = Mathf.Sin(Time.time * shakeSpeed) * maxShakeOffset;
                Vector3 pos = originalPositions[i];
                originalPositions[i] = new Vector3(pos.x - currentXOffset, pos.y, pos.z);
            }
        }

        // 如果方块组合发生变化，启动过渡
        if (!AreBlockListsEqual(previousShakingBlocks, shakingBlocks) && previousShakingBlocks.Count > 0)
        {
            Debug.Log("方块组合发生变化，启动平滑过渡");
            transitionProgress = 0f;
        }


    }

    // 应用摇晃效果 - City Bloxx风格的水平平移
    private void ApplyShakingEffect()
    {
        if (shakingBlocks.Count == 0 && previousShakingBlocks.Count == 0)
            return;

        // 计算当前的水平偏移 - 只在X轴方向摇晃（左右摇晃）
        float xOffset = Mathf.Sin(Time.time * shakeSpeed) * maxShakeOffset;

        // 平滑偏移变化，减少抖动
        xOffset = Mathf.Lerp(lastXOffset, xOffset, 0.2f);
        lastXOffset = xOffset;

        // 正在进行过渡
        if (previousShakingBlocks.Count > 0)
        {
            if (transitionProgress < 1.0f)
            {
                // 处理之前的方块组（使用渐变淡出效果）
                for (int i = 0; i < previousShakingBlocks.Count; i++)
                {
                    GameObject block = previousShakingBlocks[i];
                    if (block == null || shakingBlocks.Contains(block)) continue;

                    // 渐渐减小晃动幅度
                    float fadingOffset = xOffset * (1.0f - transitionProgress);
                    Vector3 originalPos = previousOriginalPositions[i];
                    Vector3 targetPos = new Vector3(originalPos.x + fadingOffset, originalPos.y, originalPos.z);

                    // 设置位置
                    block.transform.position = targetPos;
                }
            }

        }

        // 处理当前的方块组
        if (shakingBlocks.Count > 0)
        {
            // 应用水平摇晃到所有方块
            for (int i = 0; i < shakingBlocks.Count; i++)
            {
                GameObject block = shakingBlocks[i];
                if (block == null) continue;

                // 如果在过渡中，则逐渐增加晃动幅度
                Debug.Log("transitionProgress: " + transitionProgress + " xOffset: " + xOffset);
                float currentOffset = transitionProgress < 1.0f ? xOffset * transitionProgress : xOffset;

                // 计算方块的新位置 (只改变X坐标，保持Y和Z不变)
                Vector3 originalPos = originalPositions[i];
                Vector3 targetPos = new Vector3(originalPos.x + currentOffset, originalPos.y, originalPos.z);

                // 设置方块的位置
                block.transform.position = targetPos;
            }
        }
    }

    // 重置所有方块到原始状态
    private void ResetAllBlocks()
    {
        for (int i = 0; i < shakingBlocks.Count; i++)
        {
            GameObject block = shakingBlocks[i];
            if (block == null) continue;

            // 恢复原始位置
            if (i < originalPositions.Count)
            {
                block.transform.position = originalPositions[i];
            }
        }
    }

    // 在游戏结束时调用
    public void StopAllShaking()
    {
        ResetAllBlocks();
        shakingBlocks.Clear();
        originalPositions.Clear();

        previousShakingBlocks.Clear();
        previousOriginalPositions.Clear();
        transitionProgress = 1.0f;
        isFirstApply = true;
    }
}