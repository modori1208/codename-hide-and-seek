using System.ComponentModel;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임 세션을 관리하는 스크립트
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 게임 페이즈
    /// </summary>
    private Phase phase;

    /// <summary>
    /// 플레이어 프리팹
    /// </summary>
    public GameObject playerPrefab;

    /// <summary>
    /// (UI) 타이머 텍스트
    /// </summary>
    public TMP_Text timerText;

    /// <summary>
    /// (UI) 액션바
    /// </summary>
    public TMP_Text actionBar;

    void Start()
    {
        this.ChangePhase(new WaitPhase(this));

        // PhotonLauncher#OnJoinedRoom() 호출 이후
        // 씬이 전환되면서 마스터 클라이언트에서만 InRoom 플래그가
        // 활성화되므로 이후 난입하는 플레이어에겐 실행되지 않음 
        if (PhotonNetwork.InRoom)
            this.SpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        // 난입하는 플레이어에 대한 로직 실행
        if (!PhotonNetwork.IsMasterClient)
            this.SpawnPlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[GameManager] 플레이어 입장: {newPlayer.NickName}");
        this.phase?.OnJoin(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[GameManager] 플레이어 퇴장: {otherPlayer.NickName}");
        this.phase?.OnLeft(otherPlayer);
    }

    /// <summary>
    /// 페이즈를 변경합니다.
    /// </summary>
    /// <param name="newPhase">새 페이즈</param>
    /// <exception cref="System.Exception">동일한 페이즈인 경우 예외가 던져집니다</exception>
    void ChangePhase(Phase newPhase)
    {
        // 마스터 클라이언트가 아닌 경우 무시
        if (!PhotonNetwork.IsMasterClient)
            return;

        // 동일한 페이즈 변경 시도 검사
        if (this.phase != null && this.phase.GetType() == newPhase.GetType())
            throw new System.Exception("동일한 게임 페이즈로 변경할 수 없습니다.");

        // 이전 페이즈 정리 후 새 페이즈 초기화
        this.phase?.Terminate();
        newPhase.Initiailze();
        this.phase = newPhase;
    }

    /// <summary>
    /// 플레이어 스폰을 처리합니다.
    /// </summary>
    private void SpawnPlayer()
    {
        GameObject obj = PhotonNetwork.Instantiate(this.playerPrefab.name, Vector3.zero, Quaternion.identity);
        if (obj.GetComponent<PhotonView>().IsMine)
            Camera.main.GetComponent<CameraFollow>().target = obj.transform;
    }

    void FixedUpdate()
    {
        // 서버 사이드 로직
        if (this.phase != null)
            this.phase.Tick();

        // 클라이언트 사이드 로직
        // TODO 액션바
    }

#region RPC 처리

    [PunRPC]
    void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        this.timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    [PunRPC]
    void UpdateActionBar(string message)
    {
        this.actionBar.text = message;
    }

#endregion
}
