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
        Vector3 spawnPosition = new(0, 0, this.playerPrefab.transform.position.z);
        GameObject obj = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPosition, Quaternion.identity);
        if (obj.GetComponent<PhotonView>().IsMine)
            Camera.main.GetComponent<CameraFollow>().target = obj.transform;
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

    /// <summary>
    /// 게임 나가기 버튼
    /// </summary>
    public void OnClickBack()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main");
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
    void UpdateKeyCount(int count) => this.keyContainerHud.GetComponent<KeyCountHud>().UpdateKeyCount(count);

    [PunRPC]
    void GameEnded(int raw)
    {
        GameEndState state = (GameEndState)raw;
        if (state == GameEndState.NotEnoughPlayers)
            NoticeAlert.Create("인원 부족으로 인해 게임이 종료되었습니다.");
        else if (state == GameEndState.HidersWin)
            NoticeAlert.Create("학생들이 이겼습니다!");
        else if (state == GameEndState.SeekersWin)
            NoticeAlert.Create("선생님이 이겼습니다!");
    }

#endregion
}
