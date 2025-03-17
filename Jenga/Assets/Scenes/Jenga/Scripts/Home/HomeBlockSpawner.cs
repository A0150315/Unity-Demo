using UnityEngine;
using System.Collections.Generic;

public class HomeBlockSpawner : MonoBehaviour
{
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
                CameraController.Instance.FollowBlock(GetStackHeight());
                CheckBlockFailure();
            }
        }
    }
    
    void SpawnBlock()
    {
        if (BlockAlignmentManager.Instance.IsAligning) return;

        if (currentBlock != null)
        {
            Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                isBlockFalling = true;
            }
        }

        int randomIndex = Random.Range(0, blockPrefabs.Length);
        currentBlock = Instantiate(blockPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        allBlocks.Add(currentBlock);
         currentBlock.GetComponent<Collider>().enabled = true;
         currentBlock.GetComponent<Collider>().gameObject.AddComponent<HomeBlockCollisionHandler>();

        GameManager.Instance.IncreaseScore();

        BlockAlignmentManager.Instance.CheckForAutomaticAlignment(currentBlock);
    }
    
    void CheckBlockFailure()
    {
        if (currentBlock.transform.position.y < groundYThreshold)
        {
            GameManager.Instance.AddFallenBlock(currentBlock);
            GameManager.Instance.GameOver();
        }
    }
    
    public float GetStackHeight()
    {
        if (allBlocks.Count > 0)
        {
            // 估算堆栈高度
            return allBlocks[0].transform.position.y + 
                   (allBlocks.Count - 1) * allBlocks[0].transform.localScale.y;
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