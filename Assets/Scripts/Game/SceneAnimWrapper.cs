using UnityEngine;

public class SceneAnimWrapper : MonoBehaviour
{
    public void OnLoadIn()
    {
        GamePhaseManager.instance.OnLoadIn();
    }

    public void OnLoadOut()
    {
        GamePhaseManager.instance.OnLoadOut();
    }
}
