using UnityEngine;
using UnityEngine.UI; // ��� ������ � UI ����������
using TMPro; // ��� ������ � TextMeshPro
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    public RectTransform creditsTextRectTransform; // ������ �� RectTransform ������ ���������� �������
    public float scrollSpeed = 50f; // �������� ��������� ������ (�������� � �������)
    public float startOffset = -100f; // ��������� �������� ������ ���� (����� �� ��������� ��-�� ������)
    public float endOffset = 100f; // �������� �������� ������ ����� (����� �� ��������� ���� �� �����)

    public AudioSource creditsMusic; // ������ �� AudioSource ��� ������ ������
    public float musicFadeDuration = 3f; // ������������ ��������� ������ � �����

    private float initialTextYPosition; // ����������� Y ������� ������
    private float targetTextYPosition;  // ������� Y ������� ������

    void Start()
    {
        if (creditsTextRectTransform == null)
        {
            Debug.LogError("CreditsManager: ������ �� RectTransform ���������� ������� ������ �� �����������!", this);
            enabled = false;
            return;
        }

        if (creditsMusic == null)
        {
            Debug.LogWarning("CreditsManager: ������ �� AudioSource ��� ������ ������ �� �����������. ������ �� ����� ������/��������.", this);
            // ���������� ������ ��� ������
        }
        else
        {
            // ����������, ��� ������ ������
            if (!creditsMusic.isPlaying)
            {
                creditsMusic.Play();
            }
        }

        // ��������� ��������� � �������� Y ������� ������
        // ������������, ��� pivot ������ ����� (0)
        initialTextYPosition = creditsTextRectTransform.anchoredPosition.y - creditsTextRectTransform.sizeDelta.y + startOffset;
        targetTextYPosition = creditsTextRectTransform.sizeDelta.y + endOffset;

        // ������������� ����� � ��������� �������
        creditsTextRectTransform.anchoredPosition = new Vector2(creditsTextRectTransform.anchoredPosition.x, initialTextYPosition);

        // ��������� �������� ��� ��������� ������
        StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        // ���� ����� �� ������ ������� �������
        while (creditsTextRectTransform.anchoredPosition.y < targetTextYPosition)
        {
            // ���������� ����� �����
            creditsTextRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null; // ���� ��������� ����
        }

        Debug.Log("����� �����������. �������� ��������� ������ � ����� �� ����.");

        // ��������� ������
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
            creditsMusic.volume = startVolume; // ��������������� ��������� ��� ���������� �������, ���� �����������
        }

        // ����� �� ����
        Application.Quit();

        // ��� ������������ � ���������:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}