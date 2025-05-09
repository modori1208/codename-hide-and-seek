using UnityEngine;

/// <summary>
/// 플레이어의 2D 이동을 처리하는 스크립트
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    /// <summary>
    /// 플레이어의 물리 이동을 담당하는 Rigidbody2D 컴포넌트
    /// </summary>
    private Rigidbody2D playerBody;

    /// <summary>
    /// 이동 속도
    /// </summary>
    [Range(0f, 10f)]
    private float moveSpeed = 5.0f;

    /// <summary>
    /// 입력으로부터 계산된 이동 벡터 (정규화된 방향)
    /// </summary>
    private Vector2 movementVector;

    /// <summary>
    /// 플레이어가 이동 가능한 상태인지 여부
    /// </summary>
    [HideInInspector]
    public bool canMove;

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
        if (!this.canMove)
            return;

        // 플레이어 이동 처리
        playerBody.MovePosition(playerBody.position + movementVector * moveSpeed * Time.fixedDeltaTime);
    }

#region 충돌 처리

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO 충돌 시작 시 처리할 코드
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // TODO 충돌 중 처리할 코드
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // TODO: 충돌 종료 시 처리할 코드
    }

#endregion

}
