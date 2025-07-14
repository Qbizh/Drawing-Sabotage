using UnityEngine;
using TMPro;
using FishNet.Connection;
using DG.Tweening;

public class Pipe : MonoBehaviour
{
    [SerializeField] TMP_Text IdDisplay;

    [SerializeField] Transform entryPoint;

    [SerializeField] float suckSpeed = 3.0f;

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
                var initialDist = Vector2.Distance(entryPoint.position, other.transform.position);
                var initialScale = other.transform.localScale;

                other.transform.DOMove(entryPoint.position, suckSpeed).SetSpeedBased(true).OnUpdate(() =>
                {
                    var dist = Vector2.Distance(entryPoint.position, other.transform.position);
                    var distRatio = dist / initialDist;

                    other.transform.localScale = initialScale * distRatio;

                    other.transform.Rotate(Vector3.forward * suckSpeed * Time.deltaTime * 100 * distRatio);
                }).OnComplete(() =>
                {
                    Destroy(other.gameObject);
                }).SetEase(Ease.Linear);
            }
        }
    }
}
