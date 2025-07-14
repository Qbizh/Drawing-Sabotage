using UnityEngine;

public class SceneAnimWrapper : MonoBehaviour
{
    public void OnLoadIn()
    {
        GameManager.instance.OnLoadIn();
    }

    public void OnLoadOut()
    {
        GameManager.instance.OnLoadOut();
    }
}
