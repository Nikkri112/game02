using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image backgroundImage;
    public Button startButton;
    public Button exitButton;
    public string gameSceneName = "GameScene";

    [Range(0.5f, 5f)] public float fadeDuration = 2f;

    private CanvasGroup buttonCanvasGroup; // ��������� ������ �� CanvasGroup ��� ������
    private AudioSource menuMusic;

    void Awake() // �������� � Start �� Awake, ����� CanvasGroup ��� ����� �� Start
    {
        // �������� ��� ��������� CanvasGroup � ������������� �������� ������
        // ������������, ��� ��� ������ ��������� ��� ����� ������������ GameObject (��������, ������ GameObject ��� Canvas)
        // ���� ������ ����� �� Canvas, ����� ������� ������ GameObject, ��������� �� ���� � ���������� CanvasGroup � ����.
        // ��� ���������� CanvasGroup ��������������� � Canvas.
        // ��� ��������, ��������� CanvasGroup � Canvas, ���� ������ �������� ������� ��������� Canvas.
        // ���� ������ ����� ��������, ��������: Canvas -> Panel -> StartButton, �� CanvasGroup ����� �� Panel.

        // ����� ������� ������:
        // ���� ������ StartButton � ExitButton �������� ��������� ��������� ������ � ���� �� ��������,
        // �� ����� �������� CanvasGroup � ����� ��������.
        // ��������, ���� ��� ��� �� Canvas:
        // buttonCanvasGroup = startButton.transform.parent.GetComponent<CanvasGroup>();
        // if (buttonCanvasGroup == null)
        // {
        //     buttonCanvasGroup = startButton.transform.parent.gameObject.AddComponent<CanvasGroup>();
        // }

        // ����� �������� ������ - �������� CanvasGroup � ������ �������, ��� ����� MainMenuController,
        // � ���������, ��� ������ �������� ��� ���������, ��� ������ �������� CanvasGroup �� Canvas.
        // ��� ������� �������, �� ������� CanvasGroup � Canvas.
        buttonCanvasGroup = GetComponent<CanvasGroup>(); // �������� �������� CanvasGroup � ���� �� �������
        if (buttonCanvasGroup == null)
        {
            // ���� MainMenuController ����� �� Canvas, �� CanvasGroup ������ ���� �� Canvas
            buttonCanvasGroup = FindObjectOfType<Canvas>().GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup = FindObjectOfType<Canvas>().gameObject.AddComponent<CanvasGroup>();
            }
        }

        // ������ ������ ���������� � ���������������� ����������
        buttonCanvasGroup.alpha = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        // ���������� ������ �������� ��������� ����������
        Color tempColor = backgroundImage.color;
        tempColor.a = 0f;
        backgroundImage.color = tempColor;

        menuMusic = GetComponent<AudioSource>();
        if (menuMusic == null)
        {
            Debug.LogError("AudioSource �� ������ �� ������� MenuManager. ���������, ��� �� ����������.");
        }

        // ��������� �������� ��� �������� ��������� ������� �������� � ������
        StartCoroutine(FadeInMenu());

        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExitGame);
    }

    // �������� ��� �������� ��������� ����� ���� (�������� + ������)
    IEnumerator FadeInMenu()
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // ������������� ��������� �������� � ������
        while (elapsed < fadeDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;

            // ��������� ��������
            Color tempColor = backgroundImage.color;
            tempColor.a = Mathf.Lerp(0f, 1f, t);
            backgroundImage.color = tempColor;

            // ��������� ������ (���������� ����� CanvasGroup)
            buttonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // ����������, ��� �� ��������� ���������
        Color finalColor = backgroundImage.color;
        finalColor.a = 1f;
        backgroundImage.color = finalColor;

        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true; // ������ ������ �������������� ����� ���������
        buttonCanvasGroup.blocksRaycasts = true;
    }

    // �����, ���������� ��� ������� �� ������ "������"
    void OnStartGame()
    {
        // ��������� ������, ����� �������� ��������� ������� � ������ ������������
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;

        // ��������� �������� ��� �������� ������������ ����� ���� � �������� �����
        StartCoroutine(FadeOutAndLoadScene());
    }

    // �������� ��� �������� ������������ ����� ���� � �������� ����� �����
    IEnumerator FadeOutAndLoadScene()
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // ������������� ������������ �������� � ������
        while (elapsed < fadeDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;

            // ������������ ��������
            Color tempColor = backgroundImage.color;
            tempColor.a = Mathf.Lerp(1f, 0f, t);
            backgroundImage.color = tempColor;

            // ������������ ������
            buttonCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // ����������, ��� �� ��������� �������
        Color finalColor = backgroundImage.color;
        finalColor.a = 0f;
        backgroundImage.color = finalColor;

        buttonCanvasGroup.alpha = 0f;

        // �����������: ����� ������ �������� ������
        if (menuMusic != null)
        {
            float startVolume = menuMusic.volume;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                menuMusic.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            menuMusic.Stop();
        }

        SceneManager.LoadScene(gameSceneName);
    }

    void OnExitGame()
    {
        Debug.Log("����� �� ����");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}