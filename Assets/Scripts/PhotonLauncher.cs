using Photon.Pun;
using UnityEngine;

/// <summary>
/// 멀티플레이를 위한 Photon PUN 런처
/// </summary>
public class PhotonLauncher : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 클라이언트 프로토콜 버전
    /// </summary>
    private readonly string appVersion = "1";

    void Awake()
    {
        // 씬을 자동으로 동기화 (방장이 씬을 바꾸면 모두 따라감)
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.GameVersion = appVersion;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("[PhotonLauncher] 이미 서버에 연결됨, 랜덤 방에 입장 시도");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("[PhotonLauncher] 서버에 연결되지 않음, 새 연결 시작");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PhotonLauncher] 마스터 서버에 연결됨. 방에 입장 시도 중...");
        PhotonNetwork.JoinOrCreateRoom("DefaultRoom", new Photon.Realtime.RoomOptions(), Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PhotonLauncher] 방 입장 완료! 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");

        // AutomaticallySyncScene 옵션 때문에 모든 플레이어가 동일한 씬을 가지게 되므로
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            PhotonNetwork.LoadLevel("Game");
    }
}
