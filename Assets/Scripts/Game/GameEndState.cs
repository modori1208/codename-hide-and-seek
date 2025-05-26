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
    /// 술래가 승리하여 게임 종료
    /// </summary>
    SeekersWin
}
