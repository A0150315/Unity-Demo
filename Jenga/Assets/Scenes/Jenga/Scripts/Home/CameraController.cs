using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    public Camera mainCamera;
    private Vector3 cameraVelocity = Vector3.zero;
    public float smoothTime = 0.3f;
    public float cameraMoveDownDistance = 3f;
    private float initialCameraHeight;
    
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
    
    public void FollowBlock(float stackHeight)
    {
        if (mainCamera != null)
        {
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y = initialCameraHeight + stackHeight;
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref cameraVelocity,
                smoothTime
            );
        }
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