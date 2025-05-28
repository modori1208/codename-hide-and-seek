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
        // 게임 프로토콜 버전 설정
        PhotonNetwork.GameVersion = appVersion;

        // 씬을 자동으로 동기화 (방장이 씬을 바꾸면 모두 따라감)
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartGameConnection()
    {
        NoticeAlert.Create("서버에 연결 중입니다...", 10f);

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("[PhotonLauncher] 이미 서버에 연결됨, 방에 입장 시도");
            this.OnConnectedToMaster();
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
        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions(), Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"[PhotonLauncher] 접속에 실패함: {returnCode} {message}");

        // [ 게임 상태이므로 참여가 불가능한 경우 ]
        // 32764 상태는 존재하지 않는 방을 뜻하는데
        // 이 게임에서 세션은 하나 뿐이므로 이 상태 코드를 사용
        if (returnCode == 32764)
        {
            // 중도 참여 불가능 알림창 띄우기
            NoticeAlert.Create("게임 중이므로 잠시 후에 다시 접속해주십시오.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PhotonLauncher] 방 입장 완료! 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");

        // AutomaticallySyncScene 옵션 때문에 모든 플레이어가 동일한 씬을 가지게 되므로
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            PhotonNetwork.LoadLevel("Game");
    }
}
