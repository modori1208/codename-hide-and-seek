/// <summary>
/// 게임 종료 상태 목록
/// </summary>
public enum GameEndState
{
    /// <summary>
    /// 인원 부족으로 인한 게임 종료
    /// </summary>
    NotEnoughPlayers,

    /// <summary>
    /// 도망자가 탈출하여 게임 종료
    /// </summary>
    HidersWin,

    /// <summary>
    /// 도망자가 시간 내에 탈출하지 못해 게임 종료
    /// </summary>
    SeekersWinTimeout,

    /// <summary>
    /// 술래가 모든 도망자를 잡아 게임 종료
    /// </summary>
    SeekersWinCaught
}
