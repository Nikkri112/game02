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

    private CanvasGroup buttonCanvasGroup; // Добавляем ссылку на CanvasGroup для кнопок
    private AudioSource menuMusic;

    void Awake() // Изменено с Start на Awake, чтобы CanvasGroup был готов до Start
    {
        // Получаем или добавляем CanvasGroup к родительскому элементу кнопок
        // Предполагаем, что обе кнопки находятся под одним родительским GameObject (например, пустым GameObject или Canvas)
        // Если кнопки прямо на Canvas, можно создать пустой GameObject, поместить их туда и прикрепить CanvasGroup к нему.
        // Или прикрепить CanvasGroup непосредственно к Canvas.
        // Для простоты, прикрепим CanvasGroup к Canvas, если кнопки являются прямыми потомками Canvas.
        // Если кнопки лежат вложенно, например: Canvas -> Panel -> StartButton, то CanvasGroup нужно на Panel.

        // Самый простой способ:
        // Если кнопки StartButton и ExitButton являются дочерними объектами одного и того же родителя,
        // то можно добавить CanvasGroup к этому родителю.
        // Например, если они обе на Canvas:
        // buttonCanvasGroup = startButton.transform.parent.GetComponent<CanvasGroup>();
        // if (buttonCanvasGroup == null)
        // {
        //     buttonCanvasGroup = startButton.transform.parent.gameObject.AddComponent<CanvasGroup>();
        // }

        // Более надежный способ - добавить CanvasGroup к самому объекту, где висит MainMenuController,
        // и убедиться, что кнопки являются его потомками, или просто добавить CanvasGroup на Canvas.
        // Для данного примера, мы добавим CanvasGroup к Canvas.
        buttonCanvasGroup = GetComponent<CanvasGroup>(); // Пытаемся получить CanvasGroup с того же объекта
        if (buttonCanvasGroup == null)
        {
            // Если MainMenuController висит на Canvas, то CanvasGroup должен быть на Canvas
            buttonCanvasGroup = FindObjectOfType<Canvas>().GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup = FindObjectOfType<Canvas>().gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Делаем кнопки невидимыми и неинтерактивными изначально
        buttonCanvasGroup.alpha = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        // Изначально делаем картинку полностью прозрачной
        Color tempColor = backgroundImage.color;
        tempColor.a = 0f;
        backgroundImage.color = tempColor;

        menuMusic = GetComponent<AudioSource>();
        if (menuMusic == null)
        {
            Debug.LogError("AudioSource не найден на объекте MenuManager. Убедитесь, что он прикреплен.");
        }

        // Запускаем корутину для плавного появления фоновой картинки и кнопок
        StartCoroutine(FadeInMenu());

        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExitGame);
    }

    // Корутина для плавного появления всего меню (картинка + кнопки)
    IEnumerator FadeInMenu()
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // Одновременное появление картинки и кнопок
        while (elapsed < fadeDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;

            // Появление картинки
            Color tempColor = backgroundImage.color;
            tempColor.a = Mathf.Lerp(0f, 1f, t);
            backgroundImage.color = tempColor;

            // Появление кнопок (управление через CanvasGroup)
            buttonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Убеждаемся, что всё полностью появилось
        Color finalColor = backgroundImage.color;
        finalColor.a = 1f;
        backgroundImage.color = finalColor;

        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true; // Делаем кнопки интерактивными после появления
        buttonCanvasGroup.blocksRaycasts = true;
    }

    // Метод, вызываемый при нажатии на кнопку "Начать"
    void OnStartGame()
    {
        // Отключаем кнопки, чтобы избежать повторных нажатий и начать исчезновение
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;

        // Запускаем корутину для плавного исчезновения всего меню и загрузки сцены
        StartCoroutine(FadeOutAndLoadScene());
    }

    // Корутина для плавного исчезновения всего меню и загрузки новой сцены
    IEnumerator FadeOutAndLoadScene()
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // Одновременное исчезновение картинки и кнопок
        while (elapsed < fadeDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;

            // Исчезновение картинки
            Color tempColor = backgroundImage.color;
            tempColor.a = Mathf.Lerp(1f, 0f, t);
            backgroundImage.color = tempColor;

            // Исчезновение кнопок
            buttonCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // Убеждаемся, что всё полностью исчезло
        Color finalColor = backgroundImage.color;
        finalColor.a = 0f;
        backgroundImage.color = finalColor;

        buttonCanvasGroup.alpha = 0f;

        // Опционально: можно плавно затушить музыку
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
        Debug.Log("Выход из игры");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}