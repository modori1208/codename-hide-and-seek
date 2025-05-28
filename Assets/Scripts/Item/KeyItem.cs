using Photon.Pun;
using UnityEngine;

/// <summary>
/// 열쇠 아이템
/// </summary>
public class KeyItem : Item
{

    public override bool Trigger(GamePhase phase, GameObject playerObj, PhotonView playerPhoton)
    {
        if (phase.IsHider(playerPhoton.OwnerActorNr))
        {
            phase.KeyCollect();
            return true;
        }
        else
        {
            return false;
        }
    }
}
