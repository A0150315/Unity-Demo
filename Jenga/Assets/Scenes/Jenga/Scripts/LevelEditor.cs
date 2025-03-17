using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public GameObject blockPrefab;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Add Block"))
        {
            Vector3 spawnPosition = new Vector3(0, 1f, 0);
            Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        }
    }
}