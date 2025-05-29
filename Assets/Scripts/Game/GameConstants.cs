using System.Numerics;

/// <summary>
/// 게임 공용 상수
/// </summary>
public static class GameConstants
{

#region 패킷 아이디

    /// <summary>
    /// 플레이어의 스킨 변경을 요청하는 패킷 아이디
    /// </summary>
    public const byte SkinChangeEventCode = 1;

#endregion

#region 대기 규칙

    /// <summary>
    /// 시작을 위해 기다리는 시간
    /// </summary>
    public const float WaitTimeBeforeGameStart = 10f;

    /// <summary>
    /// 게임 결과 시간
    /// </summary>
    public const float GameResultDuration = 5f;

#endregion

#region 게임 규칙

    /// <summary>
    /// 플레이어 스폰 위치
    /// </summary>
    public static readonly Vector2 SpawnLocation = new(-5.8f, 5.8f);

    /// <summary>
    /// 플레이어 이동 속도
    /// </summary>
    public const float PlayerMovementSpeed = 5.0f;

    /// <summary>
    /// 게임 시간
    /// </summary>
    public const float GameDuration = 180f;

    /// <summary>
    /// 역할 선정 시간
    /// </summary>
    public const float TimeSelectRole = 165f;

    /// <summary>
    /// 탈출을 위한 열쇠의 개수
    /// </summary>
    public const int KeySizeForEscape = 5;

    /// <summary>
    /// 아이템의 수명 (초)
    /// </summary>
    public const int ItemLifeTime = 20;

#endregion
}
