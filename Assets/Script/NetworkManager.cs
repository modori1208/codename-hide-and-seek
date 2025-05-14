using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // �÷��̾� ������ ����

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // ������ �����ϱ�
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom(); // ���� �� ���� �õ�
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
        // ���忡 �����ϸ� �÷��̾� ��ȯ
    }
}
