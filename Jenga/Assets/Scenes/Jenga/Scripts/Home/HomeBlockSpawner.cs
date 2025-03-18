using UnityEngine;
using System.Collections.Generic;

public class HomeBlockSpawner : MonoBehaviour
{
    private bool isJustSpawned = false;
    public GameObject[] blockPrefabs;
    private GameObject currentBlock;
    private bool isBlockFalling = false;
    public Transform spawnPoint;
    public float groundYThreshold = 0.1f;

    private List<GameObject> allBlocks = new List<GameObject>();

    void Update()
    {
        if (!GameManager.Instance.isGameStarted) return;

        if (GameManager.Instance.CanSpawnBlock())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnBlock();
            }

            if (currentBlock != null && isBlockFalling)
            {
                Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 如果速度大于阈值，说明方块开始下落了
                    if (rb.velocity.magnitude > 0.5f)
                    {
                        isJustSpawned = false;  // 清除刚生成标记
                    }
                    else if (!isJustSpawned && rb.velocity.magnitude < 0.1f)
                    {
                        isBlockFalling = false;
                        
                        CameraController.Instance.UpdateCameraPosition();
                        
                        currentBlock = null;
                    }
                }
            }
        }
    }

    void SpawnBlock()
    {
        if (BlockAlignmentManager.Instance.IsAligning) return;

        if (currentBlock != null && isBlockFalling)
        {
            return;
        }

        int randomIndex = Random.Range(0, blockPrefabs.Length);
        currentBlock = Instantiate(blockPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        currentBlock.tag = "Cube";  // 设置Tag
        allBlocks.Add(currentBlock);
        isBlockFalling = true;
        isJustSpawned = true;
        currentBlock.GetComponent<Collider>().enabled = true;
        currentBlock.GetComponent<Collider>().gameObject.AddComponent<HomeBlockCollisionHandler>();

        GameManager.Instance.IncreaseScore();

        BlockAlignmentManager.Instance.CheckForAutomaticAlignment(currentBlock);
    }

    public float GetStackHeight()
    {
        if (allBlocks.Count > 0)
        {
            return (allBlocks.Count - 1) * allBlocks[0].transform.localScale.y;
        }
        return 0f;
    }

    public List<GameObject> GetAllBlocks()
    {
        return allBlocks;
    }

    public void RemoveBlock(GameObject block)
    {
        if (allBlocks.Contains(block))
        {
            allBlocks.Remove(block);
        }
    }

    public void RemoveLastBlocks(int count)
    {
        int blocksToRemove = Mathf.Min(count, allBlocks.Count);
        for (int i = 0; i < blocksToRemove; i++)
        {
            GameObject blockToRemove = allBlocks[allBlocks.Count - 1];
            allBlocks.RemoveAt(allBlocks.Count - 1);
            Destroy(blockToRemove);
        }
    }
}