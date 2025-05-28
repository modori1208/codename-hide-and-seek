using UnityEngine;

/// <summary>
/// 게임 결과 페이즈
/// </summary>
public class GameResultPhase : Phase
{

    public GameResultPhase(GameManager session) : base(session) { }

    public override void Initiailze()
    {
        Debug.Log("[Phase - Result] 초기화 요청");
        this.UpdateTimer(GameConstants.GameResultDuration);
        this.BroadcastActionBar("");
    }

    public override void Terminate()
    {
        Debug.Log("[Phase - Result] 정리 요청");

        // 모든 플레이어를 부활 처리
        this.BroadcastExitSpectatorMode();
    }

    public override void Tick()
    {
        // 대기 페이즈 이동
        if (this.TimeRemaining <= 0f)
        {
            this.session.ChangePhase(new WaitPhase(this.session));
            return;
        }

        // 게임 결과 틱 로직
        this.UpdateTimer(this.TimeRemaining - Time.fixedDeltaTime);
    }
}
