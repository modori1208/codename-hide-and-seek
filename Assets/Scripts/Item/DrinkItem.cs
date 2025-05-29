using System.Collections;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// 에너지 드링크 아이템
/// </summary>
public class DrinkItem : Item
{

    private const float Cooldown = 3.0f;
    private const float BoostedSpeed = 7.0f;

    private bool alreadyUse;

    public override bool Trigger(GamePhase phase, GameObject playerObj, PhotonView playerPhoton)
    {
        // 사용 중인 아이템이면 무시
        if (this.alreadyUse)
            return false;

        // 아이템 사용 가능한 플레이어인지 확인 후 처리
        PlayerMovement movement = playerObj.GetComponent<PlayerMovement>();
        if (movement.GetMovementSpeed() == GameConstants.PlayerMovementSpeed)
        {
            this.alreadyUse = true;
            this.transform.position = new(1000, 1000, 0); // ...
            phase.spawnedItem.Remove(this.gameObject);
            StartCoroutine(Process(movement, playerPhoton));
        }

        return false;
    }

    private IEnumerator Process(PlayerMovement movement, PhotonView photonView)
    {
        this.UpdateSpeed(movement, photonView, DrinkItem.BoostedSpeed);
        yield return new WaitForSeconds(Cooldown);
        this.UpdateSpeed(movement, photonView, GameConstants.PlayerMovementSpeed);
        PhotonNetwork.Destroy(this.gameObject); // 완전히 제거
    }

    private void UpdateSpeed(PlayerMovement movement, PhotonView photonView, float speed)
    {
        photonView.RPC("UpdateMovementSpeed", RpcTarget.All, photonView.OwnerActorNr, speed);
        movement.SetMovementSpeed(speed);
    }
}
