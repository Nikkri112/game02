using UnityEngine;
using UnityEngine.UI; // Для работы с UI элементами
using TMPro; // Для работы с TextMeshPro
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    public RectTransform creditsTextRectTransform; // Ссылка на RectTransform вашего текстового полотна
    public float scrollSpeed = 50f; // Скорость прокрутки титров (пикселей в секунду)
    public float startOffset = -100f; // Начальное смещение текста вниз (чтобы он появлялся из-за экрана)
    public float endOffset = 100f; // Конечное смещение текста вверх (чтобы он полностью ушел за экран)

    public AudioSource creditsMusic; // Ссылка на AudioSource для музыки титров
    public float musicFadeDuration = 3f; // Длительность затухания музыки в конце

    private float initialTextYPosition; // Изначальная Y позиция текста
    private float targetTextYPosition;  // Целевая Y позиция текста

    void Start()
    {
        if (creditsTextRectTransform == null)
        {
            Debug.LogError("CreditsManager: Ссылка на RectTransform текстового полотна титров не установлена!", this);
            enabled = false;
            return;
        }

        if (creditsMusic == null)
        {
            Debug.LogWarning("CreditsManager: Ссылка на AudioSource для музыки титров не установлена. Музыка не будет играть/затухать.", this);
            // Продолжаем работу без музыки
        }
        else
        {
            // Убеждаемся, что музыка играет
            if (!creditsMusic.isPlaying)
            {
                creditsMusic.Play();
            }
        }

        // Вычисляем начальную и конечную Y позиции текста
        // Предполагаем, что pivot текста внизу (0)
        initialTextYPosition = creditsTextRectTransform.anchoredPosition.y - creditsTextRectTransform.sizeDelta.y + startOffset;
        targetTextYPosition = creditsTextRectTransform.sizeDelta.y + endOffset;

        // Устанавливаем текст в начальную позицию
        creditsTextRectTransform.anchoredPosition = new Vector2(creditsTextRectTransform.anchoredPosition.x, initialTextYPosition);

        // Запускаем корутину для прокрутки титров
        StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        // Пока текст не достиг целевой позиции
        while (creditsTextRectTransform.anchoredPosition.y < targetTextYPosition)
        {
            // Перемещаем текст вверх
            creditsTextRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }

        Debug.Log("Титры закончились. Начинаем затухание музыки и выход из игры.");

        // Затухание музыки
        if (creditsMusic != null && creditsMusic.isPlaying)
        {
            float startVolume = creditsMusic.volume;
            float timer = 0f;
            while (timer < musicFadeDuration)
            {
                creditsMusic.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            creditsMusic.Stop();
            creditsMusic.volume = startVolume; // Восстанавливаем громкость для следующего запуска, если понадобится
        }

        // Выход из игры
        Application.Quit();

        // Для тестирования в редакторе:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}