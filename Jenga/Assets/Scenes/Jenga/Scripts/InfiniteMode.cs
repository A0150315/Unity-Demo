using UnityEngine;

public class InfiniteMode : MonoBehaviour
{
    public BlockSpawner blockSpawner;

    void Start()
    {
        blockSpawner = GetComponent<BlockSpawner>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // ��������ģʽ
            blockSpawner.GetComponent<BlockSpawner>().enabled = true;
        }
    }
}