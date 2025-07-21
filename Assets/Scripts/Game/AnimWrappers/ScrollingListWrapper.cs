using UnityEngine;

public class ScrollingListWrapper : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        GetComponentInParent<InputDisplayPanel>().OnAnimationEnd();
    }
}
