using UnityEngine;

public class MouseHackBehaviour : StaticItemBehaviour
{
    float maxStrength = 20f;
    float shakeSpeed = 30f;
    float maxDuration = 15f;

    public override void OnActivation()
    {
        CursorController.instance.StartShake(maxStrength * score / 100, shakeSpeed, maxDuration * score / 100);
    }
}
