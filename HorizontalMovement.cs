using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles moving the Character Horizontally
public class HorizontalMovement : Character
{
    //How fast the player should move
    [SerializeField]
    protected float speed = 500;
    //How far away from a platform the player should check to see if it is a walkable gameobject
    [SerializeField]
    protected float distanceToCollider = .08f;
    //The layers the player should check and see for movement restrictions
    [SerializeField]
    protected LayerMask collisionLayer;
    //Float that checks how much value in the horizontal direction the input is receiving to better calculate speed
    private float horizontalInput;

    protected override void Initializtion()
    {
        base.Initializtion();
    }

    void Update()
    {
        //Sets the horizontalInput value to the float value for Input.GetAxis("Horizontal") when not taking damage or climbing a ledge
        if (Input.GetAxis("Horizontal") != 0 && !character.takingDamage && !character.grabbingLedge)
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
        //If no input or taking damage, horizontalInput = 0
        else
        {
            horizontalInput = 0;
        }
    }

    //Handles all the logic for Rigidbody movement
    private void FixedUpdate()
    {
        //Basic movement based on speed
        Movement();
        //More specific movement based on certain criteria such as colliding with a wall
        SpeedModifier();
    }

    //All the basic horizontal movement logic
    private void Movement()
    {
        //Sets the Rigidbody2D velocity to speed
        rb.velocity = new Vector3(horizontalInput * speed * Time.deltaTime, rb.velocity.y);
        //Checks to see if there is movement input for various needed logic
        if (horizontalInput != 0)
        {
            //Stops playing the idle animation
            anim.SetBool("Idle", false);
            //Starts playing the walking animation
            anim.SetBool("Walking", true);
            //Checks to see if the input would move the character rightwards but also checks if the player is facing left
            if (horizontalInput > 0 && character.isFacingLeft)
            {
                //Sets the isFacingLeft bool to false so the Flip() method can run the appropriate logic
                character.isFacingLeft = false;
                //Method found in Character script to flip the character as needed
                Flip();
            }
            //Same as above if statement, but if character is moving leftward and is facing right
            if (horizontalInput < 0 && !character.isFacingLeft)
            {
                //Sets the isFacingLeft bool to true so the Flip() method can run the apprpriate logic
                character.isFacingLeft = true;
                //Method found in Character script to flip the character as needed
                Flip();
            }
        }
        //If there is no movement input
        else
        {
            //Plays the idle animation
            anim.SetBool("Idle", true);
            //Stops playing the walking animation
            anim.SetBool("Walking", false);
        }
    }

    //Method that handles more specific movement logic; right now this makes sure that if the player is jumping into a wall, they don't stick to the wall like velcro
    private void SpeedModifier()
    {
        //Long if statement that checks to see if character is jumping or falling and running into a wall
        if((rb.velocity.x > 0 && CollisionCheck(Vector2.right, distanceToCollider, collisionLayer)) 
            || (rb.velocity.x < 0 && CollisionCheck(Vector2.left, distanceToCollider, collisionLayer)) 
            && !character.isGrounded)
        {
            //If that wall or platform is a one way platform, then do nothing
            if(currentPlatform.GetComponent<OneWayPlatform>() 
                && currentPlatform.GetComponent<OneWayPlatform>().type != OneWayPlatform.OneWayPlatforms.GoingDown)
            {
                return;
            }
            //Sets a very small horizontal velocity value so the player can naturally fall if touching a wall while jumping
            rb.velocity = new Vector2(.01f, rb.velocity.y);
        }
    }
}
