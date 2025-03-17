using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraClickMove : MonoBehaviour
{
    // ���徵ͷÿ���ƶ���ƫ���������� Inspector ���������
    public Vector3 moveOffset;
    // �����������
    public Camera mainCamera;
    // ��¼������ĳ�ʼλ��
    private Vector3 initialCameraPosition;
    // ���ڴ��� 4 �뵹��ʱ�Ϳ�ʼ�ƶ��� UI ��ť����
    public Button startButton;
    // �������ڻָ���ͷλ�õ� UI ��ť������
    public Button button1;
    public Button button2;
    // ���������ť�Ƿ񶼱����
    private bool isButton1Clicked = false;
    private bool isButton2Clicked = false;
    // �����õ�Ŀ��ָ�λ��
    public Vector3 targetRestorePosition;
    // ���ƾ�ͷ�Ƿ�����ƶ��ı�־λ
    private bool canCameraMove = false;
    // ������ UI ����
    public GameObject popupUI;
    // ��������һ������ UI ����
    public GameObject newPopupUI;
    // ���ڻָ���ͷ�ƶ����ܵĵ�һ�� UI ��ť����
    public Button restoreMoveButton1;
    // ���������ڻָ���ͷ�ƶ����ܵĵڶ��� UI ��ť����
    public Button restoreMoveButton2;
    // �ƶ��ٶȣ�����ƽ���ƶ�
    public float moveSpeed = 5f;
    // ���� SmoothDamp ���ٶȱ���
    private Vector3 velocity = Vector3.zero;
    // ƽ��ʱ�䣬���ƻ��뻺��Ч��
    public float smoothTime = 0.3f;
    // ������ʾ����ʱ�� UI �ı����
    public Text countdownText;
    // ����Ƿ����ڵ���ʱ
    private bool isCountingDown = false;

    void Start()
    {
        // ���û���ֶ�ָ���������Ĭ��ʹ���������
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // ��¼������ĳ�ʼλ��
        initialCameraPosition = mainCamera.transform.position;

        // Ϊ��ʼ��ť��ӵ���¼�����
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartFourSecondCountdown);
        }

        // Ϊ�ָ���ͷλ�õİ�ť��ӵ���¼�����
        if (button1 != null)
        {
            button1.onClick.AddListener(OnButton1Clicked);
        }
        if (button2 != null)
        {
            button2.onClick.AddListener(OnButton2Clicked);
        }

        // Ϊ�ָ��ƶ����ܵİ�ť��ӵ���¼�����
        if (restoreMoveButton1 != null)
        {
            restoreMoveButton1.onClick.AddListener(OnRestoreMoveButtonClicked);
        }
        if (restoreMoveButton2 != null)
        {
            restoreMoveButton2.onClick.AddListener(OnRestoreMoveButtonClicked);
        }

        // ��ʼ���ص���ʱ�ı�
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ������� UI �Ƿ񵯳�����������ͷ�����ƶ�
        if ((popupUI != null && popupUI.activeSelf) || (newPopupUI != null && newPopupUI.activeSelf) || isCountingDown)
        {
            canCameraMove = false;
        }

        // ����������Ƿ񱻰����Ҿ�ͷ�����ƶ�
        if (Input.GetMouseButtonDown(0) && canCameraMove)
        {
            // ����Э�̽���ƽ���ƶ�
            StartCoroutine(SmoothMove(mainCamera.transform.position + moveOffset));
        }
    }

    // ��ʼ 4 �뵹��ʱ
    private void StartFourSecondCountdown()
    {
        if (!isCountingDown)
        {
            StartCoroutine(FourSecondCountdown());
        }
    }

    // 4 �뵹��ʱЭ��
    private IEnumerator FourSecondCountdown()
    {
        isCountingDown = true;
        canCameraMove = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
        {
            countdownText.text = "GO";
        }
        yield return new WaitForSeconds(1f);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        isCountingDown = false;
        canCameraMove = true;
    }

    // ��ť 1 ����¼�������
    private void OnButton1Clicked()
    {
        if (!isCountingDown)
        {
            isButton1Clicked = true;
            CheckBothButtonsClicked();
        }
    }

    // ��ť 2 ����¼�������
    private void OnButton2Clicked()
    {
        if (!isCountingDown)
        {
            isButton2Clicked = true;
            CheckBothButtonsClicked();
        }
    }

    // ���������ť�Ƿ񶼱����
    private void CheckBothButtonsClicked()
    {
        if (isButton1Clicked && isButton2Clicked)
        {
            // �ָ���ͷλ�õ�Ŀ��λ��
            StartCoroutine(SmoothMove(targetRestorePosition));
            // ���ð�ť���״̬
            isButton1Clicked = false;
            isButton2Clicked = false;
        }
    }

    // ƽ���ƶ��������Э��
    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        canCameraMove = false; // �ƶ������н�ֹ�ٴ��ƶ�
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
        canCameraMove = true; // �ƶ���ɺ������ٴ��ƶ�
    }

    // �ָ������λ�õķ���
    private void RestoreCameraPosition()
    {
        // �������λ������ΪĿ��ָ�λ��
        mainCamera.transform.position = targetRestorePosition;
    }

    // �ָ��ƶ����ܰ�ť�ĵ���¼�������
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