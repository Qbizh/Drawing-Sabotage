using UnityEngine;
using TMPro;
using FishNet.Connection;

public class Pipe : MonoBehaviour
{
    [SerializeField] TMP_Text IdDisplay;

    [SerializeField] Transform entryPoint;

    [SerializeField] float minDist = 0.05f;

    Rigidbody2D targetRb;

    float initialDist;
    float initialScale;

    NetworkConnection client;

    public void SetPlayer(NetworkConnection id)
    {
        client = id;
        IdDisplay.text = id.ToString();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Card"))
        {
            if (CardsManager.instance.SendCard(client))
            {
                targetRb = other.attachedRigidbody;

                initialDist = (targetRb.position - (Vector2)entryPoint.position).magnitude;
                initialScale = targetRb.transform.localScale.x;
            }
        }
    }

    private void FixedUpdate()
    {
        if (targetRb != null)
        {
            var dir = ((Vector2)entryPoint.position - targetRb.position).normalized;

            targetRb.AddForce(dir * 10f);

            var dist = (targetRb.position - (Vector2)entryPoint.position).magnitude;

            targetRb.transform.localScale = Vector3.one * Mathf.Lerp(initialScale, 0.1f, 1 - dist / initialDist);

            if (dist < minDist)
            {
                Destroy(targetRb.gameObject);
            }
        }
    }
}
