using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float fireballSpeed = 10f;
    Rigidbody2D myRigidBody;
    PlayerMovement player;
    float xSpeed;

    // RT Added :-)Fire direction so that the sprite flips when shooting left
    float fireDir;


    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * fireballSpeed;

        // RT Added :-) Sets the direction of fire as the direction of the player
        fireDir = player.transform.localScale.x;
    }


    void Update()
    {
        myRigidBody.velocity = new Vector2(xSpeed, 0f);
        
        // RT Added :-) sets the fire direction as above, ensuring the fireball faces the right way when fired
        transform.localScale = new Vector3(fireDir, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
