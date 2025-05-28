using Photon.Pun;
using UnityEngine;

/// <summary>
/// 아이템 추상화 클래스
/// </summary>
public abstract class Item : MonoBehaviour
{

    /// <summary>
    /// 아이템이 플레이어와 충돌하면 호출됩니다.
    /// </summary>
    /// <param name="phase">현재 게임 페이즈</param>
    /// <param name="playerObj">플레이어 오브젝트</param>
    /// <param name="playerPhoton">충돌한 플레이어의 Photon 오브젝트</param>
    /// <returns>아이템 사용 여부</returns>
    public abstract bool Trigger(GamePhase phase, GameObject playerObj, PhotonView playerPhoton);
}
