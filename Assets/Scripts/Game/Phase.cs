using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 게임 페이즈
/// </summary>
public abstract class Phase
{

    protected readonly GameManager session;

    private float timeRemaining = 0f;
    public virtual float TimeRemaining => timeRemaining;

    protected Phase(GameManager session)
    {
        this.session = session;
    }

    /// <summary>
    /// 페이즈 초기화를 시작할 때 호출됩니다.
    /// </summary>
    public abstract void Initiailze();

    /// <summary>
    /// 페이즈가 종료되어 정리할 때 호출됩니다.
    /// </summary>
    public abstract void Terminate();

    /// <summary>
    /// 플레이어가 세션에 입장하면 호출됩니다.
    /// </summary>
    /// <param name="newPlayer">입장 플레이어</param>
    public abstract void OnJoin(Player newPlayer);

    /// <summary>
    /// 플레이어가 세션에서 퇴장하면 호출됩니다.
    /// </summary>
    /// <param name="oldPlayer">퇴장 플레이어</param>
    public abstract void OnLeft(Player oldPlayer);

    /// <summary>
    /// 매 게임 시간(틱)마다 호출됩니다.
    /// </summary>
    public abstract void Tick();

    /// <summary>
    /// 게임 타이머를 업데이트합니다.
    /// </summary>
    /// <param name="time">게임 타이머</param>
    protected virtual void UpdateTimer(float time)
    {
        this.timeRemaining = time;
        this.session.photonView.RPC("UpdateTimer", RpcTarget.All, time);
    }
}
