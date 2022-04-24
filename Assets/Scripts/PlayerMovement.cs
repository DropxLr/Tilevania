using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    // [SerializeField] float jumpSpeed = 15f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    [SerializeField] GameObject fireball;
    [SerializeField] Transform firePoint;


    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    public float airTime = 0.55f;
    private float jumpSpeed;


    bool isAlive = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        jumpSpeed = 9.81f * (airTime / 2f) * myRigidBody.gravityScale;
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }


    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(fireball, firePoint.position, transform.rotation);
    }


    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        bool isTouchingGround = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool isTouchingLadder = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        if (!isTouchingGround && !isTouchingLadder) { return; }

        if (value.isPressed)
        {
            myRigidBody.velocity = new Vector2(0f, jumpSpeed);
            // myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }

    }

    void ClimbLadder()
    {

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        //This line fixes a bug where the run animation stops when going past a ladder, it was necessary to stop the running animation
        // as it conflicting with the climbing animation when using a controller as input

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isRunning", false);
        }

        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;


        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);

    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

}
