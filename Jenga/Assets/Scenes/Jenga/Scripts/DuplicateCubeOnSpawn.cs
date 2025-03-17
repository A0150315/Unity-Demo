using UnityEngine;

public class DuplicateCubeOnSpawn : MonoBehaviour
{
    // 用于指定生成另一个方块的位置
    public Transform duplicatePosition;

    // 当生成方块时调用此方法
    public void SpawnCube(GameObject originalCube)
    {
        if (originalCube == null || duplicatePosition == null)
        {
            Debug.LogWarning("Original cube or duplicate position is not assigned.");
            return;
        }

        // 实例化一个新的方块，使用原始方块的预制体
        GameObject newCube = Instantiate(originalCube, duplicatePosition.position, originalCube.transform.rotation);

        // 设置新方块的缩放与原始方块相同
        newCube.transform.localScale = originalCube.transform.localScale;
    }
}