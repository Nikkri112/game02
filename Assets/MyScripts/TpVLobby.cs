using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TpVLobby : MonoBehaviour
{
    public GameObject LobbySpawnPoint;
    public GameObject NeLobbySpawnPoint;
    public TextMeshProUGUI ButtonText;
    public GameObject Player;
    private bool InLobby = false;
    public string VLobby = "� �����";
    public string NeVLobby = "�� � �����";
    public GameObject LobbInterface = null;
    public GameObject fightButton = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonDown()
    {
        //������ ����� ���������� ������������� �������� 1 ��� 2.
        if (!InLobby)
        {
            ButtonText.text = NeVLobby;
            Player.transform.position = (LobbySpawnPoint.transform.position);
            InLobby = true;
            LobbInterface.SetActive(true);
            fightButton.SetActive(true);
        }
        else 
        {
            ButtonText.text = VLobby;
            Player.transform.position = NeLobbySpawnPoint.transform.position;
            InLobby=false;
            LobbInterface.SetActive(false);
            fightButton.SetActive(false);
        }
    }
}
