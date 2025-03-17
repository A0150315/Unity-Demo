using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    public Camera mainCamera;
    private Vector3 cameraVelocity = Vector3.zero;
    public float smoothTime = 0.3f;
    public float cameraMoveDownDistance = 3f;
    private float initialCameraHeight;
    private bool isAdjustingCamera = false; // 是否正在调整相机
    
    void Awake()
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
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        initialCameraHeight = mainCamera.transform.position.y;
    }
    
    public void UpdateCameraPosition()
    {
        if (isAdjustingCamera || mainCamera == null) return;
        
        HomeBlockSpawner spawner = FindObjectOfType<HomeBlockSpawner>();
        if (spawner == null || spawner.GetAllBlocks().Count == 0) return;
        
        // 只有当方块成功放置时，才调整相机位置
        List<GameObject> allBlocks = spawner.GetAllBlocks();
        if (allBlocks.Count > 0)
        {
            StartCoroutine(AdjustCameraPosition(allBlocks));
        }
    }
    
    private IEnumerator AdjustCameraPosition(List<GameObject> blocks)
    {
        isAdjustingCamera = true;
        
        // 等待一小段时间，确保方块已经稳定
        yield return new WaitForSeconds(0.2f);
        
        // 获取最高方块的顶部位置
        float highestPoint = 0f;
        foreach (GameObject block in blocks)
        {
            if (block != null)
            {
                Collider collider = block.GetComponent<Collider>();
                if (collider != null)
                {
                    float topY = collider.bounds.max.y;
                    if (topY > highestPoint)
                    {
                        highestPoint = topY;
                    }
                }
            }
        }
        
        // 设置目标位置
        Vector3 targetPosition = mainCamera.transform.position;
        targetPosition.y = highestPoint + initialCameraHeight;
        
        // 平滑移动相机
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        
        while (elapsedTime < smoothTime)
        {
            mainCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsedTime / smoothTime
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = targetPosition;
        isAdjustingCamera = false;
    }
    
    public void MoveDownCamera()
    {
        if (mainCamera != null)
        {
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y -= cameraMoveDownDistance;
            StartCoroutine(MoveCameraToPosition(targetPosition));
        }
    }
    
    private IEnumerator MoveCameraToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = mainCamera.transform.position;

        while (elapsedTime < smoothTime)
        {
            mainCamera.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }
    
    public void ResetCamera()
    {
        // 重置摄像机位置到初始状态
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 10, -10);
            mainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
        }
    }
} 