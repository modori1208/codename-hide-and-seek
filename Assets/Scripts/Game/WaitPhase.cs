using Photon.Pun;
using UnityEngine;

/// <summary>
/// 플레이어 대기 페이즈
/// </summary>
public class WaitPhase : Phase
{

    public WaitPhase(GameManager session) : base(session) { }

    public override void Initiailze()
    {
        Debug.Log("[Phase - Wait] 초기화 요청");
        this.UpdateTimer(GameConstants.WaitTimeBeforeGameStart);
        this.session.photonView.RPC("UpdateActionBar", RpcTarget.All, "");
    }

    public override void Tick()
    {
        // 현재 플레이어 수 확인 후, 홀수거나 2인 미만이면 대기
        int playerCount = PhotonNetwork.PlayerList.Length;
        if (playerCount < 2 || playerCount % 2 != 0)
        {
            this.UpdateTimer(GameConstants.WaitTimeBeforeGameStart);
            this.session.photonView.RPC("UpdateActionBar", RpcTarget.All, "플레이어를 기다리는 중..."); // TODO State enum을 만들어 RPC로 이전하는 것이 좋을까?
            return;
        }

        // 플레이어 수가 짝수이면 시작 카운트다운
        if (this.TimeRemaining > 0f)
        {
            this.UpdateTimer(this.TimeRemaining - Time.fixedDeltaTime);
            this.session.photonView.RPC("UpdateActionBar", RpcTarget.All, "추가 플레이어 대기 중...");
        }
        // 게임 시작
        else
        {
            // TODO 게임 페이즈로 전환
            Debug.Log($"[Phase - Wait] 게임 시작 준비");
            this.session.ChangePhase(new GamePhase(this.session));
        }
    }
}
