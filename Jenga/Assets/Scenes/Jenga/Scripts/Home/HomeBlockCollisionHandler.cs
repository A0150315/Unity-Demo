using UnityEngine;

public class HomeBlockCollisionHandler : MonoBehaviour
{
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
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
} 