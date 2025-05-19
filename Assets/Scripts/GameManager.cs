using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 플레이어 프리팹
    /// </summary>
    public GameObject playerPrefab;

    /// <summary>
    /// (UI) 타이머 텍스트
    /// </summary>
    public TMP_Text timerText;

    public float gameDuration = 180f; // 3분
    private float timeRemaining;
    private bool gameEnded = false;

    void Start()
    {
        this.timeRemaining = this.gameDuration;

        AssignRoles(); // TODO Remove

        // PhotonLauncher#OnJoinedRoom() 호출 이후
        // 씬이 전환되면서 마스터 클라이언트에서만 InRoom 플래그가
        // 활성화되므로 이후 난입하는 플레이어에겐 실행되지 않음 
        if (PhotonNetwork.InRoom)
        {
            this.SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        // 난입하는 플레이어에 대한 로직 실행
        if (!PhotonNetwork.IsMasterClient)
        {
            this.SpawnPlayer();
        }
    }

    // 플레이어 스폰 처리
    private void SpawnPlayer()
    {
        GameObject obj = PhotonNetwork.Instantiate(this.playerPrefab.name, Vector3.zero, Quaternion.identity);
        if (obj.GetComponent<PhotonView>().IsMine)
        {
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

    [PunRPC]
    void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
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
