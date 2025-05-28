using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// 게임 진행 페이즈
/// </summary>
public class GamePhase : Phase
{

    private bool _selectedRole;
    private readonly Dictionary<int, PlayerState> _playerStates = new();
    private readonly Dictionary<PlayerState, int> _stateSizes = new();
    public readonly Dictionary<GameObject, double> spawnedItem = new();
    private int _collectedKeySize;

    public enum PlayerState
    {
        Seeker, // 술래
        Hider, // 숨은 도망자
        Escaper, // 탈출한 도망자
        Dead // 사망한 플레이어
    }

    public GamePhase(GameManager session) : base(session) { }

    public override void Initiailze()
    {
        Debug.Log("[Phase - Game] 초기화 요청");
        this.UpdateTimer(GameConstants.GameDuration);
        this.BroadcastActionBar("");
        this.SetRoomJoinable(false);

        // 모든 플레이어를 도망자로 설정
        foreach (Player player in PhotonNetwork.PlayerList)
            this.SetToHider(player);
    }

    public override void Terminate()
    {
        Debug.Log("[Phase - Game] 정리 요청");

        // 모든 플레이어의 스킨을 초기화
        foreach (Player player in PhotonNetwork.PlayerList)
            this.SendPlayerSpriteChange(player.ActorNumber, true);

        // 클라이언트에게 빈 열쇠 정보 전송
        this.KeyReset();

        // 세션에 있는 모든 아이템 제거
        List<GameObject> itemsToRemove = new();
        foreach (var pair in this.spawnedItem)
            PhotonNetwork.Destroy(pair.Key);
    }

    public override void OnLeft(Player otherPlayer)
    {
        Debug.Log($"[Phase - Game] 플레이어 퇴장: {otherPlayer.ActorNumber}");

        // 플레이어를 세션에서 제거 
        if (this._playerStates.TryGetValue(otherPlayer.ActorNumber, out var prevState))
        {
            this._stateSizes[prevState] = this._stateSizes[prevState] - 1;
            this._playerStates.Remove(otherPlayer.ActorNumber);
        }
    }

    public override void Tick()
    {
        // 게임 종료
        if (this.TimeRemaining <= 0f)
        {
            this.session.ChangePhase(new GameResultPhase(this.session));
            return;
        }

        // 게임 틱 로직
        this.UpdateTimer(this.TimeRemaining - Time.fixedDeltaTime);
        this.SelectRole();
        this.CheckPlayerSize();
        this.SpawnItems();
    }

#region 플레이어 상태 관리

    private void SetPlayerState(Player player, PlayerState state) => this.SetPlayerState(player.ActorNumber, state);

    /// <summary>
    /// 플레이어의 상태를 설정
    /// </summary>
    /// <param name="actorNumber">대상 플레이어의 아이디</param>
    /// <param name="state">플레이어 상태</param>
    public void SetPlayerState(int actorNumber, PlayerState state)
    {
        // 이전 역할의 플레이어 크기 조절
        if (this._playerStates.TryGetValue(actorNumber, out var prevState))
            this._stateSizes[prevState] = this._stateSizes[prevState] - 1;

        // 새 역할의 플레이어 크기 조절
        this._playerStates[actorNumber] = state;
        int currentSize = this._stateSizes.GetValueOrDefault(state, 0);
        this._stateSizes[state] = currentSize + 1;

        // 새 스프라이트 적용
        if (state == PlayerState.Hider)
            this.SendPlayerSpriteChange(actorNumber, true);
        else if (state == PlayerState.Seeker)
            this.SendPlayerSpriteChange(actorNumber, false);
    }

    private int GetPlayerSizeByState(PlayerState state) => this._stateSizes.GetValueOrDefault(state, 0);

    private void SetToHider(Player player) => this.SetPlayerState(player, PlayerState.Hider);
    private void SetToSeeker(Player player) => this.SetPlayerState(player, PlayerState.Seeker);

    private PlayerState GetPlayerState(Player player) => this.GetPlayerState(player.ActorNumber);
    private PlayerState GetPlayerState(int actorNumber) => this._playerStates[actorNumber];

    public bool IsHider(Player player) => this.IsHider(player.ActorNumber);
    public bool IsHider(int actorNumber) => this.GetPlayerState(actorNumber) == PlayerState.Hider;

    public bool IsSeeker(Player player) => this.IsSeeker(player.ActorNumber);
    public bool IsSeeker(int actorNumber) => this.GetPlayerState(actorNumber) == PlayerState.Seeker;

#endregion

#region 게임 틱 로직

    /// <summary>
    /// 게임 플레이어 확인
    /// </summary>
    private void CheckPlayerSize()
    {
        /* 아직 역할이 정해지지 않은 경우 */
        if (!this._selectedRole)
            return;

        // 플레이어 수가 부족한 경우
        int totalPlayer = PhotonNetwork.PlayerList.Length;
        if (totalPlayer < 2)
        {
            this.UpdateTimer(0);
            this.BroadcastGameEnd(GameEndState.NotEnoughPlayers);
            return;
        }

        int seekerSize = this.GetPlayerSizeByState(PlayerState.Seeker);
        int hiderSize = this.GetPlayerSizeByState(PlayerState.Hider);
        int escaperSize = this.GetPlayerSizeByState(PlayerState.Escaper);
        int deadPlayerSize = this.GetPlayerSizeByState(PlayerState.Dead);

        // 술래 수의 절반이 탈출한 경우 도망자 승리
        if (seekerSize / 2.0f <= escaperSize)
        {
            this.UpdateTimer(0);
            this.BroadcastGameEnd(GameEndState.HidersWin);
        }
        // 모든 도망자가 죽은 경우
        else if (deadPlayerSize + seekerSize == totalPlayer)
        {
            this.UpdateTimer(0);
            this.BroadcastGameEnd(GameEndState.SeekersWin);
        }
    }

    /// <summary>
    /// 역할 선택 시간 확인
    /// </summary>
    private void SelectRole()
    {
        float selectRemaining = Mathf.FloorToInt(this.TimeRemaining - GameConstants.TimeSelectRole);

        /* 아직 역할 선정까지 시간이 남았을 경우 */
        if (selectRemaining > 0)
        {
            this.BroadcastActionBar($"역할 선정까지 {selectRemaining}초 남았습니다.");
        }
        /* 역할 선정이 완료되지 않은 경우 */
        else if (!_selectedRole)
        {
            this._selectedRole = true;

            // 게임 인원의 절반을 술래로 설정
            Player[] currentPlayers = PhotonNetwork.PlayerList;
            int targetSize = Mathf.CeilToInt(currentPlayers.Length / 2f);

            List<Player> shuffled = currentPlayers.OrderBy(p => Random.value).ToList();
            List<Player> selected = shuffled.Take(targetSize).ToList();

            foreach (Player player in selected)
                this.SetToSeeker(player);

            // 이제 역할을 알려주자...
            foreach (Player player in currentPlayers)
            {
                string role = this.IsSeeker(player) ? "술래" : "도망자";
                this.SendActionBar(player, $"당신의 역할은 {role} 입니다!");
            }
        }
    }

    /// <summary>
    /// 아이템 스폰 로직
    /// </summary>
    private void SpawnItems()
    {
        /* 아직 역할이 정해지지 않은 경우 아이템을 생성하지 않음 */
        if (!this._selectedRole)
            return;

        /* 아이템 수명 주기 확인 후 제거 */
        List<GameObject> itemsToRemove = new();
        foreach (var pair in this.spawnedItem)
            if (PhotonNetwork.Time - pair.Value > GameConstants.ItemLifeTime)
                itemsToRemove.Add(pair.Key);

        foreach (GameObject item in itemsToRemove)
        {
            this.spawnedItem.Remove(item);
            PhotonNetwork.Destroy(item);
        }

        /* 키 아이템 스폰 */
        if (spawnedItem.Count < 10)
        {
            GameObject obj = this.session.SpawnItem("ItemKey");
            this.spawnedItem.Add(obj, PhotonNetwork.Time);
        }
    }

#endregion

#region 아이템 관리

    public void KeyCollect()
    {
        // 이미 탈출에 필요한 키를 모두 획득한 경우
        if (this.CanEscape())
            return;

        this._collectedKeySize++;
        if (this.CanEscape())
            this.BroadcastActionBar("이제 탈출구로 탈출할 수 있습니다!");

        this.BroadcastKeyCount();
    }

    public void KeyReset()
    {
        this._collectedKeySize = 0;
        this.BroadcastKeyCount();
    }

    public bool CanEscape()
    {
        return this._collectedKeySize >= GameConstants.KeySizeForEscape;
    }

    private void BroadcastKeyCount()
    {
        this.session.photonView.RPC("UpdateKeyCount", RpcTarget.All, this._collectedKeySize);
    }

#endregion
}
