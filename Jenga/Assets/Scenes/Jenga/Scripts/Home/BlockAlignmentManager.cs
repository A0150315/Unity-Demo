using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockAlignmentManager : MonoBehaviour
{
    public static BlockAlignmentManager Instance { get; private set; }

    [Header("对齐设置")]
    private bool isAligning = false;
    private bool isAlignmentActive = false;
    private float alignmentStartTime;
    private float alignmentDuration = 3f;

    [Header("吸附设置")]
    private bool isAdsorbingActive = false;
    private float adsorbStartTime;

    [Header("自动吸附设置")]
    private bool isAutoAdsorbing = false;
    private float autoAdsorbStartTime;
    public Text autoAdsorbCountdownText;

    [Header("UI元素")]
    public Button alignBlocksButton;
    public Button secondAlignBlocksButton;
    public Button adsorbAlignButton;
    public Button autoAdsorbButton;

    public bool IsAligning { get { return isAligning; } }

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
        if (alignBlocksButton != null)
        {
            alignBlocksButton.onClick.AddListener(ActivateAlignment);
        }
        if (secondAlignBlocksButton != null)
        {
            secondAlignBlocksButton.onClick.AddListener(RemoveThreeBlocks);
        }
        if (adsorbAlignButton != null)
        {
            adsorbAlignButton.onClick.AddListener(ActivateAdsorbAlignment);
        }
        if (autoAdsorbButton != null)
        {
            autoAdsorbButton.onClick.AddListener(ActivateAutoAdsorb);
        }
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (isAlignmentActive && Time.time - alignmentStartTime > alignmentDuration)
        {
            isAlignmentActive = false;
        }

        if (isAdsorbingActive && Time.time - adsorbStartTime > alignmentDuration)
        {
            isAdsorbingActive = false;
        }

        if (isAutoAdsorbing)
        {
            float remainingTime = alignmentDuration - (Time.time - autoAdsorbStartTime);
            if (remainingTime > 0)
            {
                if (autoAdsorbCountdownText != null)
                {
                    autoAdsorbCountdownText.text = Mathf.CeilToInt(remainingTime).ToString();
                }
            }
            else
            {
                isAutoAdsorbing = false;
                if (autoAdsorbCountdownText != null)
                {
                    autoAdsorbCountdownText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void CheckForAutomaticAlignment(GameObject newBlock)
    {
        if (isAlignmentActive)
        {
            StartCoroutine(AlignBlocks());
        }

        if (isAdsorbingActive)
        {
            StartCoroutine(AdsorbAlignBlocks());
        }

        if (isAutoAdsorbing)
        {
            StartCoroutine(AutoAdsorbBlocks());
        }
    }

    public void ActivateAlignment()
    {
        isAlignmentActive = true;
        alignmentStartTime = Time.time;
        StartCoroutine(AlignBlocks());
    }

    private IEnumerator AlignBlocks()
    {
        isAligning = true;
        yield return new WaitForEndOfFrame();

        var allBlocks = FindObjectOfType<HomeBlockSpawner>().GetAllBlocks();
        if (allBlocks.Count == 0)
        {
            isAligning = false;
            yield break;
        }

        GameObject firstBlock = allBlocks[0];
        Vector3 basePosition = firstBlock.transform.position;
        float blockHeight = firstBlock.transform.localScale.y;

        for (int i = 0; i < allBlocks.Count; i++)
        {
            GameObject block = allBlocks[i];
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            block.transform.position = new Vector3(
                basePosition.x,
                basePosition.y + i * blockHeight,
                basePosition.z
            );
            block.transform.rotation = Quaternion.identity;
        }

        isAligning = false;
    }

    private void ActivateAdsorbAlignment()
    {
        isAdsorbingActive = true;
        adsorbStartTime = Time.time;
        StartCoroutine(AdsorbAlignBlocks());
    }

    private IEnumerator AdsorbAlignBlocks()
    {
        isAligning = true;
        yield return new WaitForEndOfFrame();

        var allBlocks = FindObjectOfType<HomeBlockSpawner>().GetAllBlocks();
        if (allBlocks.Count == 0)
        {
            isAligning = false;
            yield break;
        }

        GameObject topBlock = null;
        if (allBlocks.Count > 0)
        {
            topBlock = allBlocks[allBlocks.Count - 1];
        }

        for (int i = allBlocks.Count - 1; i >= 0; i--)
        {
            GameObject block = allBlocks[i];
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            if (topBlock != null)
            {
                Vector3 targetPos = topBlock.transform.position + Vector3.up * block.transform.localScale.y * (allBlocks.Count - 1 - i);
                block.transform.position = targetPos;
                block.transform.rotation = Quaternion.identity;
            }
        }

        isAligning = false;
    }

    private void ActivateAutoAdsorb()
    {
        isAutoAdsorbing = true;
        autoAdsorbStartTime = Time.time;
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(true);
            autoAdsorbCountdownText.text = "3";
        }
    }

    private IEnumerator AutoAdsorbBlocks()
    {
        isAligning = true;
        yield return new WaitForEndOfFrame();

        var allBlocks = FindObjectOfType<HomeBlockSpawner>().GetAllBlocks();
        if (allBlocks.Count < 2)
        {
            isAligning = false;
            yield break;
        }

        GameObject lastBlock = allBlocks[allBlocks.Count - 1];
        GameObject secondLastBlock = allBlocks[allBlocks.Count - 2];

        Rigidbody rb = lastBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Vector3 targetPos = secondLastBlock.transform.position + Vector3.up * lastBlock.transform.localScale.y;
        lastBlock.transform.position = targetPos;
        lastBlock.transform.rotation = Quaternion.identity;

        isAligning = false;
    }

    private void RemoveThreeBlocks()
    {
        var HomeBlockSpawner = FindObjectOfType<HomeBlockSpawner>();
        HomeBlockSpawner.RemoveLastBlocks(3);

        // 移动摄像机
        CameraController.Instance.MoveDownCamera();
    }

    public void ResetAlignmentState()
    {
        isAligning = false;
        isAlignmentActive = false;
        isAdsorbingActive = false;
        isAutoAdsorbing = false;
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }
}