using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject hiderPrefab;

    public float gameDuration = 180f; // 3분
    private float timeRemaining;

    public TextMeshProUGUI timerText;

    private bool gameEnded = false;

    void Start()
    {
        this.timeRemaining = this.gameDuration;

        AssignRoles();

        if (photonView.IsMine) // TODO Spawn Logic
        {
            GameObject obj = PhotonNetwork.Instantiate(hiderPrefab.name, Vector3.zero, Quaternion.identity);
            Camera.main.GetComponent<CameraFollow>().target = obj.transform;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && !gameEnded)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                EndGame("술래 승리! 시간 초과");
            }

            photonView.RPC("UpdateTimer", RpcTarget.All, timeRemaining);
        }
    }

    public override void OnJoinedRoom()
    {
        GameObject obj = PhotonNetwork.Instantiate(hiderPrefab.name, Vector3.zero, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().target = obj.transform;
    }

    [PunRPC]
    void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        //timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    void AssignRoles()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length < 2)
        {
            Debug.LogWarning("플레이어가 부족합니다.");
            return;
        }

        // TODO 흠...
    }

    void EndGame(string result)
    {
        this.gameEnded = true;
        photonView.RPC("ShowEndResult", RpcTarget.All, result);
    }

    [PunRPC]
    void ShowEndResult(string message)
    {
        Debug.Log("게임 종료: " + message);
        // TODO 결과 창 UI 띄우기, 다시 시작 등 처리
    }
}
