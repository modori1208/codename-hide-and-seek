using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
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
    /// 플레이어 스프라이트 애니메이터
    /// </summary>
    private Animator animator;

    [SerializeField]
    private RuntimeAnimatorController hiderAnimator;
    [SerializeField]
    private RuntimeAnimatorController seekerAnimator;

    /// <summary>
    /// 플레이어의 2D 이동을 처리하는 컴포넌트
    /// </summary>
    private PlayerMovement playerMovement;

    void Awake()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.transform.GetChild(0).GetComponent<TMP_Text>().text = this.photonView.Owner.NickName;
        this.animator = this.transform.GetChild(1).GetComponent<Animator>();
        this.animator.speed = 0f;
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
            if (game.IsSeeker(this.photonView.OwnerActorNr) && game.IsHider(otherView.OwnerActorNr))
            {
                // 플레이어를 사망 처리하기
                game.SetPlayerState(otherView.OwnerActorNr, GamePhase.PlayerState.Dead);
                game.SendEnterSpectatorMode(otherView.OwnerActorNr);
            }
        }

        // 2. 아이템과 충돌한 경우
        if (collision.gameObject.CompareTag("Item"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item.Trigger(game, this.gameObject, this.photonView))
            {
                game.spawnedItem.Remove(collision.gameObject);
                PhotonNetwork.Destroy(collision.gameObject);
            }
        }

        // 3. 탈출구와 충돌한 경우
        if (collision.gameObject.CompareTag("EscapeDoor"))
        {
            // 내 역할이 도망자인 경우 플레이어를 탈출한 것으로 처리하기
            if (game.CanEscape() && game.IsHider(this.photonView.OwnerActorNr))
            {
                game.SetPlayerState(this.photonView.OwnerActorNr, GamePhase.PlayerState.Escaper);
                game.SendEnterSpectatorMode(this.photonView.OwnerActorNr);
            }
        }
    }

#endregion

#region RPC 처리

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += PlayerSpriteChange;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= PlayerSpriteChange;
    }

    public void PlayerSpriteChange(EventData photonEvent)
    {
        if (photonEvent.Code == GameConstants.SkinChangeEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            bool isHider = (bool)data[1];

            if (this.photonView.OwnerActorNr != actorNumber)
                return;

            this.animator.runtimeAnimatorController = isHider ? this.hiderAnimator : this.seekerAnimator;
        }
    }

    [PunRPC]
    void PlayerSpriteWalking(int actorNumber, bool isWalking)
    {
        if (this.photonView.OwnerActorNr != actorNumber)
            return;

        this.animator.Play("boo", 0, 0f);
        this.animator.speed = isWalking ? 1f : 0f;
    }

    [PunRPC]
    void PlayerSpriteFaceRight(int actorNumber, bool isRight)
    {
        if (this.photonView.OwnerActorNr != actorNumber)
            return;

        Transform transform = this.transform.GetChild(1);
        Vector3 prevScale = transform.localScale;
        float x = Mathf.Abs(prevScale.x) * (isRight ? -1 : 1);
        transform.localScale = new(x, prevScale.y, prevScale.z);
    }

    [PunRPC]
    void UpdateMovementSpeed(int actorNumber, float speed)
    {
        if (this.photonView.OwnerActorNr == actorNumber)
            this.playerMovement.SetMovementSpeed(speed);
    }

#endregion
}
