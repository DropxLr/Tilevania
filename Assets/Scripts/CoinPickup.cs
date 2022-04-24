using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{

    [SerializeField] int pointsForCoinPickUp = 50;

    bool wasCollected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickUp);
            GetComponent<AudioSource>().Play();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
