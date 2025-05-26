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

        // 모든 플레이어를 도망자로 설정
        foreach (Player player in PhotonNetwork.PlayerList)
            this.SetToHider(player);
    }

    public override void Terminate()
    {
        Debug.Log("[Phase - Game] 정리 요청");
    }

    public override void OnJoin(Player newPlayer)
    {
        // 중도 참여는 불가능하므로 세션에서 추방
        PhotonNetwork.CloseConnection(newPlayer);
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
    }

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
    }

    private int GetPlayerSizeByState(PlayerState state) => this._stateSizes.GetValueOrDefault(state, 0);

    private void SetToHider(Player player) => this.SetPlayerState(player, PlayerState.Hider);
    private void SetToSeeker(Player player) => this.SetPlayerState(player, PlayerState.Seeker);

    private PlayerState GetPlayerState(Player player) => this.GetPlayerState(player.ActorNumber);
    private PlayerState GetPlayerState(int actorNumber) => this._playerStates[actorNumber];

    public bool IsSeeker(Player player) => this.IsSeeker(player.ActorNumber);
    public bool IsSeeker(int actorNumber) => this.GetPlayerState(actorNumber) == PlayerState.Seeker;

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

    #endregion
}
