using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class EndGameScript : MonoBehaviour
{
    private Scene scene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting scene load");
        StartCoroutine(LoadCreditsAfterDelay(2f));
    }

    IEnumerator LoadCreditsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ������ ��������� ����� - �� ����� �������� ����� �������
        SceneManager.LoadScene(2); // ������ ����� � �������

        Debug.Log("Scene loaded");
    }
}
