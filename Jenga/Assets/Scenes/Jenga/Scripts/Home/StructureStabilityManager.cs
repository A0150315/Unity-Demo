using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StructureStabilityManager : MonoBehaviour
{
    public static StructureStabilityManager Instance { get; private set; }

    [Header("摇晃设置")]
    [SerializeField] private int topBlocksToShake = 3;        // 顶部要摇晃的方块数量
    [SerializeField] private float maxShakeAngle = 3f;      // 最大摇晃角度
    [SerializeField] private float shakeSpeed = 1.5f;         // 摇晃速度
    [SerializeField] private float heightThreshold = 5f;      // 开始摇晃的高度阈值

    private HomeBlockSpawner blockSpawner;
    private List<GameObject> shakingBlocks = new List<GameObject>();
    private GameObject pivotBlock;  // 作为旋转中心的方块
    private Vector3 pivotOriginalPosition; // 旋转中心的原始位置
    private List<Vector3> relativePositions = new List<Vector3>(); // 其他方块相对于旋转中心的位置

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
        InvokeRepeating("UpdateShakingBlocks", 1f, 0.5f); // 每0.5秒更新一次摇晃的方块
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameStarted) return;

        ApplyShakingEffect();
    }

    // 更新需要摇晃的顶部方块
    private void UpdateShakingBlocks()
    {
        if (blockSpawner == null || GameManager.Instance == null || !GameManager.Instance.isGameStarted) return;

        // 获取所有已放置的方块
        List<GameObject> allBlocks = blockSpawner.GetAllBlocks();
        if (allBlocks.Count == 0) return;

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
    }

    // 应用摇晃效果 - 以底部晃动方块为中心，整体摇晃
    private void ApplyShakingEffect()
    {
        if (shakingBlocks.Count == 0 || pivotBlock == null) return;

        // 计算当前的摇晃角度 - 只在Z轴方向摇晃（左右摇晃）
        float zAngle = Mathf.Sin(Time.time * shakeSpeed) * maxShakeAngle;
        
        // 创建一个以pivotBlock位置为中心的旋转矩阵
        Quaternion rotation = Quaternion.Euler(0, 0, zAngle);
        
        // 应用旋转到所有方块
        for (int i = 0; i < shakingBlocks.Count; i++)
        {
            GameObject block = shakingBlocks[i];
            if (block == null) continue;
            
            // 计算旋转后的位置（以pivotBlock为中心旋转）
            Vector3 rotatedPosition = pivotOriginalPosition + (rotation * relativePositions[i]);
            
            // 设置方块的位置
            block.transform.position = rotatedPosition;
            
            // 应用相同的旋转到方块自身
            block.transform.rotation = rotation;
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
    }
} 