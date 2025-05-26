using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 게임 페이즈
/// </summary>
public abstract class Phase
{

    protected readonly GameManager session;

    private float timeRemaining = 0f;
    public float TimeRemaining => timeRemaining;

    protected Phase(GameManager session)
    {
        this.session = session;
    }

    /// <summary>
    /// 페이즈 초기화를 시작할 때 호출됩니다.
    /// </summary>
    public virtual void Initiailze() { }

    /// <summary>
    /// 페이즈가 종료되어 정리할 때 호출됩니다.
    /// </summary>
    public virtual void Terminate() { }

    /// <summary>
    /// 플레이어가 세션에 입장하면 호출됩니다.
    /// </summary>
    /// <param name="newPlayer">입장 플레이어</param>
    public virtual void OnJoin(Player newPlayer) { }

    /// <summary>
    /// 플레이어가 세션에서 퇴장하면 호출됩니다.
    /// </summary>
    /// <param name="oldPlayer">퇴장 플레이어</param>
    public virtual void OnLeft(Player oldPlayer) { }

    /// <summary>
    /// 매 게임 시간(틱)마다 호출됩니다.
    /// </summary>
    public abstract void Tick();

    /// <summary>
    /// 방 접속 가능 여부를 설정합니다.
    /// </summary>
    /// <param name="joinable">접속 가능 여부</param>
    protected void SetRoomJoinable(bool joinable)
    {
        PhotonNetwork.CurrentRoom.IsOpen = joinable;
    }

    /// <summary>
    /// 게임 타이머를 업데이트합니다.
    /// </summary>
    /// <param name="time">게임 타이머</param>
    protected void UpdateTimer(float time)
    {
        this.timeRemaining = time <= 0 ? 0 : time;
        this.session.photonView.RPC("UpdateTimer", RpcTarget.All, this.timeRemaining);
    }

    /// <summary>
    /// 세션의 모든 플레이어에게 액션바를 전송합니다.
    /// </summary>
    /// <param name="message">액션바의 내용</param>
    protected void BroadcastActionBar(string message)
    {
        this.session.photonView.RPC("UpdateActionBar", RpcTarget.All, message);
    }

    /// <summary>
    /// 특정 플레이어에게 액션바를 전송합니다.
    /// </summary>
    /// <param name="player">대상 플레이어</param>
    /// <param name="message">액션바의 내용</param>
    protected void SendActionBar(Player player, string message)
    {
        this.session.photonView.RPC("UpdateActionBar", player, message);
    }

    /// <summary>
    /// 특정 플레이어의 스프라이트를 변경합니다.
    /// </summary>
    /// <param name="actorNumber">대상 플레이어의 아이디</param>
    /// <param name="isHider">도망자 여부</param>
    protected void SendPlayerSpriteChange(int actorNumber, bool isHider)
    {
        // 게임 오브젝트가 다르면 RPC를 전송할 수 없는 문제가 있어 우회
        object[] content = new object[] { actorNumber, isHider };
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameConstants.SkinChangeEventCode, content, options, SendOptions.SendReliable);
    }

    /// <summary>
    /// 세션의 모든 플레이어에게 게임 종료 상태를 전송합니다.
    /// </summary>
    /// <param name="state">게임 종료 상태</param>
    protected void BroadcastGameEnd(GameEndState state)
    {
        this.session.photonView.RPC("GameEnded", RpcTarget.All, (int)state);
    }
}
