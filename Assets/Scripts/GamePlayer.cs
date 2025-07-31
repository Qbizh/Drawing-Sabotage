using UnityEngine;
using FishNet.Object;

public class GamePlayer : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            PipesManager.instance.AddPlayerPipe(Owner, PlayerDataHolder.instance.playerData);
        }
    }
}
