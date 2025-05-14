using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // 플레이어 프리팹 연결

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // 서버에 연결하기
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom(); // 랜덤 방 입장 시도
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(playerPrefab.name,
            new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f)),
            Quaternion.identity);
        // 입장에 성공하면 플레이어 소환
    }
}
