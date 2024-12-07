using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PentominoManager : MonoBehaviour
{
    [Header("PUZZLE PLACE")]
    [SerializeField] private GameObject puzzlePlace;

    [Header("PUZZLE ANSWER BLOCKS")]
    [SerializeField] private GameObject[] answerBlocks;

    [Header("PUZZLE SCENE BLOCKS")]
    [SerializeField] private GameObject[] sceneBlocks;

    [Header("UI ELEMENTS")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI playerPointText;
    private int playerPoint = 0;

    [Header("Chapter Completed")]
    [SerializeField] private GameObject completeScreen;

    [Header("VFX EFFECTS")]
    [SerializeField] private ParticleSystem ConfettiEffects;

    private Camera mainCamera;

    private GameObject activeBlock;
    private Vector3 startPosition;
    private float zValue = 12f;
    private bool isDragging = false;
    private Collider puzzlePlaceCollider;
    private Vector3 offset;

    void Start()
    {
        completeScreen.SetActive(false);

        resultText.text = "";
        playerPointText.text = "PUAN: " + PlayerPrefs.GetInt("PlayerPoint");

        mainCamera = Camera.main;
        puzzlePlaceCollider = puzzlePlace.GetComponent<Collider>();
        UpdateActiveBlock();

        answerBlocks[0].SetActive(true);

        if (PlayerPrefs.HasKey("PlayerPoint"))
        {
            playerPoint = PlayerPrefs.GetInt("PlayerPoint");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && activeBlock != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == activeBlock.transform)
            {
                isDragging = true;
                offset = activeBlock.transform.position - hit.point;
                activeBlock.transform.position = new Vector3(activeBlock.transform.position.x, activeBlock.transform.position.y, zValue);
            }
        }

        if (Input.GetMouseButton(0) && isDragging && activeBlock != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPosition = hit.point + offset;
                Vector3 clampedPosition = ClampToPuzzlePlaceBounds(targetPosition);
                activeBlock.transform.position = new Vector3(clampedPosition.x, clampedPosition.y, zValue);
                activeBlock.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            if (activeBlock != null)
            {
                ReturnToStartPosition();
                CheckForAnswer();
            }
        }
    }

    void UpdateActiveBlock()
    {
        foreach (GameObject block in answerBlocks)
        {
            if (block.activeSelf)
            {
                activeBlock = block;
                startPosition = activeBlock.transform.position;
                break;
            }
        }
    }

    void ReturnToStartPosition()
    {
        if (activeBlock != null)
        {
            activeBlock.transform.position = startPosition;
        }
    }

    Vector3 ClampToPuzzlePlaceBounds(Vector3 targetPosition)
    {
        Vector3 clampedPosition = targetPosition;

        Bounds puzzleBounds = puzzlePlaceCollider.bounds;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, puzzleBounds.min.x, puzzleBounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, puzzleBounds.min.y, puzzleBounds.max.y);

        clampedPosition.z = zValue;

        return clampedPosition;
    }

    void CheckForAnswer()
    {
        if (activeBlock == null) return;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            string targetTag = activeBlock.name + "_Target";
            if (hit.transform.CompareTag(targetTag))
            {
                int blockIndex = System.Array.IndexOf(answerBlocks, activeBlock);

                if (blockIndex >= 0 && blockIndex < sceneBlocks.Length)
                {
                    sceneBlocks[blockIndex].SetActive(false);
                    activeBlock.SetActive(false);

                    if (blockIndex + 1 < answerBlocks.Length)
                    {
                        answerBlocks[blockIndex + 1].SetActive(true);
                        UpdateActiveBlock();
                    }

                    resultText.text = "DOÐRU CEVAP!";
                    ConfettiEffects.Play();
                    playerPoint += 20;
                    PlayerPrefs.SetInt("PlayerPoint", playerPoint);
                    playerPointText.text = "PUAN: " + playerPoint.ToString();

                    if (AllBlocksCompleted())
                    {
                        CompletePuzzle();
                    }
                }
            }
            else
            {
                resultText.text = "YANLIÞ CEVAP!";
            }
        }
    }

    bool AllBlocksCompleted()
    {
        foreach (GameObject block in answerBlocks)
        {
            if (block.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    void CompletePuzzle()
    {
        resultText.text = "TÜM BLOKLAR TAMAMLANDI!";
        completeScreen.SetActive(true);
        ConfettiEffects.Play();
    }

    public void ChangeScene() 
    {
        SceneManager.LoadScene("Level_2");
    }

    public void Reset()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
