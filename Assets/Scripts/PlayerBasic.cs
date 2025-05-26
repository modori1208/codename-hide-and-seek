using Photon.Pun;
using UnityEngine;

/// <summary>
/// 플레이어 로직 스크립트
/// </summary>
public class PlayerBasic : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 게임 세션
    /// </summary>
    private GameManager gameManager;

    /// <summary>
    /// 플레이어의 2D 이동을 처리하는 컴포넌트
    /// </summary>
    private PlayerMovement playerMovement;

    void Awake()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.playerMovement = GetComponent<PlayerMovement>();
    }

    #region 충돌 처리

    void OnTriggerEnter2D(Collider2D collision)
    {
        /* 마스터 클라이언트가 아닌 경우 무시 */
        if (!PhotonNetwork.IsMasterClient)
            return;

        /* 현재 게임 상태가 아니라면 무시 */
        if (this.gameManager.Phase is not GamePhase game)
            return;

        // 1. 플레이어와 충돌한 경우
        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView otherView = collision.gameObject.GetComponent<PhotonView>();

            // 내 역할이 술래이고 상대가 도망자 경우
            if (game.IsSeeker(this.photonView.OwnerActorNr) && !game.IsSeeker(otherView.OwnerActorNr))
            {
                // TODO 플레이어 사망을 어떻게 처리할 것인가?
                game.SetPlayerState(otherView.OwnerActorNr, GamePhase.PlayerState.Dead);
            }
        }

        // 2. 아이템과 충돌한 경우 (TODO)
        
        // 3. 탈출구와 충돌한 경우 (TODO)
    }

#endregion
}
