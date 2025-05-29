using static GameConstants;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// 플레이어의 2D 이동을 처리하는 스크립트
/// </summary>
public class PlayerMovement : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// 플레이어의 물리 이동을 담당하는 Rigidbody2D 컴포넌트
    /// </summary>
    private Rigidbody2D playerBody;

    /// <summary>
    /// 이동 속도
    /// </summary>
    [Range(0f, 10f)]
    private float moveSpeed = PlayerMovementSpeed;

    /// <summary>
    /// 입력으로부터 계산된 이동 벡터 (정규화된 방향)
    /// </summary>
    private Vector2 movementVector;

    /// <summary>
    /// 플레이어가 이동 가능한 상태인지 여부
    /// </summary>
    [HideInInspector]
    public bool canMove;

    private bool wasWalkingLastSent;
    private bool wasFacingRightLastSent;

    void Awake()
    {
        this.playerBody = GetComponent<Rigidbody2D>();
        this.canMove = true;
    }

    void Update()
    {
        // 인풋 데이터 저장
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        this.movementVector = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        if (this.photonView == null || !this.photonView.IsMine || !this.canMove)
            return;

        /* 플레이어 이동 처리 */
        playerBody.MovePosition(playerBody.position + movementVector * moveSpeed * Time.fixedDeltaTime);

        /* 애니메이션 동기화 */
        bool isWalking = this.movementVector != Vector2.zero;
        bool isFacingRight = this.movementVector.x > 0;

        // 걷기 상태 변경 감지
        if (isWalking != this.wasWalkingLastSent)
        {
            this.wasWalkingLastSent = isWalking;
            this.photonView.RPC("PlayerSpriteWalking", RpcTarget.All, this.photonView.OwnerActorNr, isWalking);
        }

        // 바라보는 방향 변경 감지
        if (isWalking && this.movementVector.x != 0)
        {
            if (isFacingRight != this.wasFacingRightLastSent)
            {
                this.wasFacingRightLastSent = isFacingRight;
                this.photonView.RPC("PlayerSpriteFaceRight", RpcTarget.All, this.photonView.OwnerActorNr, isFacingRight);
            }
        }
    }

    /// <summary>
    /// 플레이어의 이동 속도를 가져옵니다.
    /// </summary>
    /// <returns>이동 속도</returns>
    public float GetMovementSpeed() => this.moveSpeed;

    /// <summary>
    /// 플레이어의 이동 속도를 조절합니다.
    /// </summary>
    /// <param name="speed">이동 속도 (0~10 사이의 실수)</param>
    public void SetMovementSpeed(float speed)
    {
        if (speed < 0 || speed > 10)
            throw new System.Exception($"{speed}는 허용되지 않는 속도입니다.");
        this.moveSpeed = speed;
    }
}
