using static GameConstants;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 세션을 관리하는 스크립트
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 게임 페이즈
    /// </summary>
    private Phase phase;
    public virtual Phase Phase => phase;

    /// <summary>
    /// 로컬 플레이어 오브젝트
    /// </summary>
    private GameObject localPlayer;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject keyContainerHud;
    [SerializeField]
    private PolygonCollider2D itemSpawnAllowArea;
    [SerializeField]
    private PolygonCollider2D itemSpawnForbiddenArea;

    [SerializeField]
    private TMP_Text timerText;

    [SerializeField]
    private TMP_Text actionBar;
    private float actionBarDuration;

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 마스터 클라이언트의 퇴장으로 게임 진행이 불가능하므로 퇴장
        MainMenu.disconnectByMasterClient = true;
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 서버와 연결이 끊긴 경우 씬 전환
        this.phase = null;
        SceneManager.LoadScene("Main");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[GameManager] 플레이어 입장: {newPlayer.ActorNumber}");
        this.phase?.OnJoin(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[GameManager] 플레이어 퇴장: {otherPlayer.ActorNumber}");
        this.phase?.OnLeft(otherPlayer);
    }

    /// <summary>
    /// 게임 나가기 버튼
    /// </summary>
    public void OnClickBack(GameObject exitButton)
    {
        exitButton.SetActive(false);
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// 페이즈를 변경합니다.
    /// </summary>
    /// <param name="newPhase">새 페이즈</param>
    /// <exception cref="System.Exception">동일한 페이즈인 경우 예외가 던져집니다</exception>
    public void ChangePhase(Phase newPhase)
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
        Vector3 spawnPosition = new(SpawnLocation.X, SpawnLocation.Y, this.playerPrefab.transform.position.z);
        GameObject obj = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPosition, Quaternion.identity);
        if (obj.GetPhotonView().IsMine)
        {
            Camera.main.GetComponent<CameraFollow>().target = obj.transform;
            this.localPlayer = obj;
        }
    }

    /// <summary>
    /// 아이템을 스폰합니다.
    /// </summary>
    /// <param name="prefabName">프리팹 이름</param>
    /// <returns>스폰된 아이템 오브젝트</returns>
    public GameObject SpawnItem(string prefabName)
    {
        /* 아이템 스폰 가능 위치 탐색 */
        Vector2 spawnPoint;
        int attempts = 0;

        do
        {
            Bounds bounds = this.itemSpawnAllowArea.bounds;
            spawnPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            attempts++;
        } while (
            // 결정된 위치가 스폰 허용 범위 안에 들어있고 비허용 범위 안에 없는 경우
            (!this.itemSpawnAllowArea.OverlapPoint(spawnPoint) ||
                this.itemSpawnForbiddenArea.OverlapPoint(spawnPoint))
            // 시도 횟수가 30회를 초과하면 아이템 스폰을 포기 (...)
            && attempts < 30
        );

        /* 아이템 스폰 시도 횟수를 넘은 경우 처리하지 않음 */
        if (attempts >= 30)
            return null;

        /* 아이템 스폰 */
        Vector3 spawnPointV3 = new(spawnPoint.x, spawnPoint.y, -1);
        return PhotonNetwork.Instantiate(prefabName, spawnPointV3, Quaternion.identity);
    }

    void FixedUpdate()
    {
        /* 서버 사이드 로직 */
        if (this.phase != null)
            this.phase.Tick();

        /* 클라이언트 사이드 로직 */

        // 액션바 처리
        if (this.actionBarDuration > 0)
            this.actionBarDuration -= Time.fixedDeltaTime;
        else
        {
            this.actionBarDuration = 0f;
            this.actionBar.text = "";
        }
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
        this.actionBarDuration = 2f;
        this.actionBar.text = message;
    }

    [PunRPC]
    void UpdateKeyCount(int count) =>
        this.keyContainerHud.GetComponent<KeyCountHud>().UpdateKeyCount(count);

    [PunRPC]
    void TeleportTo(float x, float y)
    {
        if (this.localPlayer == null)
            return;

        Vector3 prev = this.localPlayer.transform.position;
        this.localPlayer.transform.position = new(x, y, prev.z);
    }

    [PunRPC]
    void EnterSpectatorMode()
    {
        // 디스폰 처리
        PhotonNetwork.Destroy(this.localPlayer);
        this.localPlayer = null;

        // 랜덤 플레이어 선정 후 관전 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
            return;

        GameObject random = players[Random.Range(0, players.Length)];
        Camera.main.GetComponent<CameraFollow>().target = random.transform;
    }

    [PunRPC]
    void ExitSpectatorMode()
    {
        // 관전 상태인 경우 플레이어 재생성
        if (this.localPlayer == null)
            this.SpawnPlayer();
    }

    [PunRPC]
    void GameEnded(int raw)
    {
        GameEndState state = (GameEndState)raw;
        float duration = GameConstants.GameResultDuration - 1;

        if (state == GameEndState.NotEnoughPlayers)
            NoticeAlert.Create("인원 부족으로 인해 게임이 종료되었습니다.", duration);
        else if (state == GameEndState.HidersWin)
            NoticeAlert.Create("학생들이 학교를 탈출하여 승리하였습니다!", duration);
        else if (state == GameEndState.SeekersWinTimeout)
            NoticeAlert.Create("학생들이 학교를 탈출하지 못하였습니다!", duration);
        else if (state == GameEndState.SeekersWinCaught)
            NoticeAlert.Create("선생님이 모든 학생을 붙잡아 승리하였습니다!", duration);
    }

#endregion
}
