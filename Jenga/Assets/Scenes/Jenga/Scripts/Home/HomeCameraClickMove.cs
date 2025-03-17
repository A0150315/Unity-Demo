using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HomeCameraClickMove : MonoBehaviour
{
    // 定义镜头每次移动的偏移量，可在 Inspector 面板中设置
    public Vector3 moveOffset;
    // 引用主摄像机
    public Camera mainCamera;
    // 用于触发 4 秒倒计时和开始移动的 UI 按钮引用
    public Button startButton;
    // 两个用于恢复镜头位置的 UI 按钮的引用
    public Button button1;
    public Button button2;
    // 标记两个按钮是否都被点击
    private bool isButton1Clicked = false;
    private bool isButton2Clicked = false;
    // 可配置的目标恢复位置
    public Vector3 targetRestorePosition;
    // 控制镜头是否可以移动的标志位
    private bool canCameraMove = false;
    // 弹出的 UI 引用
    public GameObject popupUI;
    // 新增的另一个弹出 UI 引用
    public GameObject newPopupUI;
    // 用于恢复镜头移动功能的第一个 UI 按钮引用
    public Button restoreMoveButton1;
    // 新增：用于恢复镜头移动功能的第二个 UI 按钮引用
    public Button restoreMoveButton2;
    // 移动速度，用于平滑移动
    public float moveSpeed = 5f;
    // 用于 SmoothDamp 的速度变量
    private Vector3 velocity = Vector3.zero;
    // 平滑时间，控制缓入缓出效果
    public float smoothTime = 0.3f;
    // 用于显示倒计时的 UI 文本组件
    public Text countdownText;
    // 标记是否正在倒计时
    private bool isCountingDown = false;

    void Start()
    {
        // 如果没有手动指定摄像机，默认使用主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 为恢复镜头位置的按钮添加点击事件监听
        if (button1 != null)
        {
            button1.onClick.AddListener(OnButton1Clicked);
        }
        if (button2 != null)
        {
            button2.onClick.AddListener(OnButton2Clicked);
        }

        // 为恢复移动功能的按钮添加点击事件监听
        if (restoreMoveButton1 != null)
        {
            restoreMoveButton1.onClick.AddListener(OnRestoreMoveButtonClicked);
        }
        if (restoreMoveButton2 != null)
        {
            restoreMoveButton2.onClick.AddListener(OnRestoreMoveButtonClicked);
        }

        // 初始隐藏倒计时文本
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 检查两个 UI 是否弹出，若弹出则镜头不能移动
        if ((popupUI != null && popupUI.activeSelf) || (newPopupUI != null && newPopupUI.activeSelf) || isCountingDown)
        {
            canCameraMove = false;
        }

        // 检查鼠标左键是否被按下且镜头可以移动
        if (Input.GetMouseButtonDown(0) && canCameraMove)
        {
            // 启动协程进行平滑移动
            StartCoroutine(SmoothMove(mainCamera.transform.position + moveOffset));
        }
    }

    // 按钮 1 点击事件处理方法
    private void OnButton1Clicked()
    {
        if (!isCountingDown)
        {
            isButton1Clicked = true;
            CheckBothButtonsClicked();
        }
    }

    // 按钮 2 点击事件处理方法
    private void OnButton2Clicked()
    {
        if (!isCountingDown)
        {
            isButton2Clicked = true;
            CheckBothButtonsClicked();
        }
    }

    // 检查两个按钮是否都被点击
    private void CheckBothButtonsClicked()
    {
        if (isButton1Clicked && isButton2Clicked)
        {
            // 恢复镜头位置到目标位置
            StartCoroutine(SmoothMove(targetRestorePosition));
            // 重置按钮点击状态
            isButton1Clicked = false;
            isButton2Clicked = false;
        }
    }

    // 平滑移动摄像机的协程
    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        canCameraMove = false; // 移动过程中禁止再次移动
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.001f)
        {
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref velocity,
                smoothTime,
                moveSpeed
            );
            yield return null;
        }
        mainCamera.transform.position = targetPosition;
        canCameraMove = true; // 移动完成后允许再次移动
    }

    // 恢复移动功能按钮的点击事件处理方法
    private void OnRestoreMoveButtonClicked()
    {
        if (!isCountingDown)
        {
            canCameraMove = true;
            if (popupUI != null)
            {
                popupUI.SetActive(false);
            }
            if (newPopupUI != null)
            {
                newPopupUI.SetActive(false);
            }
        }
    }
}