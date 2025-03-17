using UnityEngine;

public class DuplicateCubeOnSpawn : MonoBehaviour
{
    // ����ָ��������һ�������λ��
    public Transform duplicatePosition;

    // �����ɷ���ʱ���ô˷���
    public void SpawnCube(GameObject originalCube)
    {
        if (originalCube == null || duplicatePosition == null)
        {
            Debug.LogWarning("Original cube or duplicate position is not assigned.");
            return;
        }

        // ʵ����һ���µķ��飬ʹ��ԭʼ�����Ԥ����
        GameObject newCube = Instantiate(originalCube, duplicatePosition.position, originalCube.transform.rotation);

        // �����·����������ԭʼ������ͬ
        newCube.transform.localScale = originalCube.transform.localScale;
    }
}