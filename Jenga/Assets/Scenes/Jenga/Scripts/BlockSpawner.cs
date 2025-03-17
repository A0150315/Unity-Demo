using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BlockSpawner : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    private GameObject currentBlock;
    private bool isBlockFalling = false;
    private bool isGameStarted = false;
    private bool isAligning = false;
    private bool isAlignmentActive = false;
    private float alignmentStartTime;
    private float alignmentDuration = 3f;
    public int score = 0;
    public Transform spawnPoint;
    public Camera mainCamera;
    private float stackHeight = 0f;
    private Vector3 cameraVelocity = Vector3.zero;
    public float smoothTime = 0.3f;
    public GameObject gameOverUI;
    public Button startGameButton;
    public Button continueButton;
    public GameObject failCube1;
    public GameObject failCube2;
    public GameObject collisionPopupUI;
    public AudioClip failureSound;
    private AudioSource audioSource;
    public float groundYThreshold = 0.1f;
    private List<GameObject> allBlocks = new List<GameObject>();
    private List<GameObject> fallenBlocks = new List<GameObject>();
    private float startDelay = 4f;
    private float startTime;
    public Button alignBlocksButton;
    public Button secondAlignBlocksButton; // �˰�ť����������������
    public Button adsorbAlignButton;
    private bool isAdsorbingActive = false;
    private float adsorbStartTime;
    public Button autoAdsorbButton;
    private bool isAutoAdsorbing = false;
    private float autoAdsorbStartTime;
    public Text autoAdsorbCountdownText;
    public float cameraMoveDownDistance = 3f; // ��ͷ�����ƶ��ľ���

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeUI();
        isGameStarted = false;

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
        if (!isGameStarted || isAligning) return;

        if (Time.time - startTime >= startDelay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnBlock();
            }

            if (currentBlock != null && isBlockFalling)
            {
                FollowBlockWithCamera();
                CheckBlockFailure();
            }
        }

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

    private void InitializeUI()
    {
        gameOverUI.SetActive(false);
        collisionPopupUI.SetActive(false);
        startGameButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        startGameButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
    }

    void StartGame()
    {
        ResetGame();
        isGameStarted = true;
        startGameButton.gameObject.SetActive(false);
        startTime = Time.time;
    }

    private void ResetGame()
    {
        score = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }
        ClearFallenBlocks();
        isAligning = false;
        isAlignmentActive = false;
        isAdsorbingActive = false;
        isAutoAdsorbing = false;
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    void SpawnBlock()
    {
        if (isAligning) return;

        if (currentBlock != null)
        {
            Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                isBlockFalling = true;
            }
        }

        int randomIndex = Random.Range(0, blockPrefabs.Length);
        currentBlock = Instantiate(blockPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        allBlocks.Add(currentBlock);
        currentBlock.GetComponent<Collider>().enabled = true;
        currentBlock.GetComponent<Collider>().gameObject.AddComponent<BlockCollisionHandler>().SetParent(this);

        score++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }

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

    public void OnBlockCollision(Collision collision)
    {
        if (collision.gameObject == failCube1 || collision.gameObject == failCube2)
        {
            PlayFailureSound();
            GameOver();
        }
    }

    void CheckBlockFailure()
    {
        if (currentBlock.transform.position.y < groundYThreshold)
        {
            PlayFailureSound();
            fallenBlocks.Add(currentBlock);
            GameOver();
        }
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
        Text gameOverText = gameOverUI.GetComponentInChildren<Text>();
        if (gameOverText != null)
        {
            gameOverText.text = "恭喜你！获得: " + score;
        }
        isGameStarted = false;
        startGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

        score = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScore(score);
        }
        isAligning = false;
        isAlignmentActive = false;
        isAdsorbingActive = false;
        isAutoAdsorbing = false;
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    void FollowBlockWithCamera()
    {
        if (mainCamera != null)
        {
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y = stackHeight + currentBlock.transform.localScale.y / 2 + 5f;
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref cameraVelocity,
                smoothTime
            );
        }
    }

    void ContinueGame()
    {
        gameOverUI.SetActive(false);
        collisionPopupUI.SetActive(false);
        continueButton.gameObject.SetActive(false);
        ClearFallenBlocks();
        isGameStarted = true;
        startTime = Time.time;
        isAligning = false;
        isAlignmentActive = false;
        isAdsorbingActive = false;
        isAutoAdsorbing = false;
        if (autoAdsorbCountdownText != null)
        {
            autoAdsorbCountdownText.gameObject.SetActive(false);
        }
    }

    private void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.clip = failureSound;
            audioSource.Play();
        }
    }

    private void ClearFallenBlocks()
    {
        foreach (GameObject block in fallenBlocks)
        {
            if (block != null)
            {
                allBlocks.Remove(block);
                Destroy(block);
            }
        }
        fallenBlocks.Clear();
    }

    private void ActivateAlignment()
    {
        isAlignmentActive = true;
        alignmentStartTime = Time.time;
        StartCoroutine(AlignBlocks());
    }

    private IEnumerator AlignBlocks()
    {
        isAligning = true;
        yield return new WaitForEndOfFrame();

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
        int blocksToRemove = Mathf.Min(3, allBlocks.Count);
        for (int i = 0; i < blocksToRemove; i++)
        {
            GameObject blockToRemove = allBlocks[allBlocks.Count - 1];
            allBlocks.RemoveAt(allBlocks.Count - 1);
            Destroy(blockToRemove);
        }

        // ��ͷ�����ƶ�
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
}

public class BlockCollisionHandler : MonoBehaviour
{
    private BlockSpawner parent;

    public void SetParent(BlockSpawner parent)
    {
        this.parent = parent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (parent != null)
        {
            parent.OnBlockCollision(collision);
        }
    }
}