/*This script is part of a larger solution, but can be used as a standalone
 * script; if you want to use this as a standalone script, make sure everything
 * that is commented off in psuedocode is no longer commented out, and that
 * you delete the Initialization method found within this script, as well as
 * change every reference of character.grabbingLedge to just grabbingLedge.
 
   If you do want to use this with my larger solution, then you don't need to 
   make any changes to this script
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to the Character to allow the Character to react to platforms that contain the Ledge script so the Character can hang and/or climb when appropriate
public class LedgeLocator : Character
{
    //The animation that plays when the Character climbs a ledge to it can Lerp the player position based on how long the animation time is
    [SerializeField]
    protected AnimationClip clip;
    //A value that will help push the Character inward on the platform when it is climbing a ledge
    [SerializeField]
    protected float climbingHorizontalOffset = .5f;

    //The top point of the Collider2D on the the Character
    private Vector2 topOfPlayer;
    //A variable to help this script know whatever GameObject is currently the ledge the Character is hanging from
    private GameObject ledge;
    //A quick float value that will get overriden by the clip variable if there is no AnimationClip selected; this value is setup as a failsafe so the script can still run if you forget to attach an animation
    private float animationTime = .5f;
    //Bool that determines if the Character is falling off a platform and no longer hanging; needs to be set so there is a tiny delay after falling so the Character doesn't grab onto the platform again
    private bool falling;
    //Bool that stops character from grabbing the same ledge they just dropped from
    private bool moved;
    //Bool that makes sure the LedgeClimbing animation doesn't play too much
    private bool climbing;
    //Reference of the Jump script on the Player
    private Jump jump;

    /*
    //These are extra variables that are setup in the Character script that this script inherits from; if you are using your own solution and don't want to use a Character script, then these variables will need to be established in order to get the logic in this script to work
    [HideInInspector]
    public bool GrabbingLedge;
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;
    private Jump jump;


    //Use this Start method if you aren't using a Character script yourself and still want the logic in this script to run
    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jump = GetComponent<Jump>();
        if (clip != null)
        {
            animationTime = clip.length;
        }
    }
    */

    //Start method that is run in the Character script; if there is no Character script in your project, then uncomment the code above and use that Start() method
    protected override void Initializtion()
    {
        //This will grab all the references setup already in the Character script
        base.Initializtion();
        jump = GetComponent<Jump>();
        //Checks to see if there is an AnimationClip plugged into the clip variable
        if (clip != null)
        {
            //If there is an AnimationClip plugged into the clip variable, then it overrides the private float animationTime to whatever the length of the animation in the clip variable is
            animationTime = clip.length;
        }
    }

    //Using the FixedUpdate because it works better with Rigidbody values so it's collision is accurate with whatever calculations are performed by Rigidbody
    protected virtual void FixedUpdate()
    {
        //Checks to see if the Character is in the "Sweet Spot" of a platform to hang from the ledge
        CheckForLedge();
        //Runs the logic that would need to happen when hanging from a ledge
        LedgeHanging();
    }

    //Method that determines if the Character should hang from a ledge based on a few different parameters defined within this method
    protected virtual void CheckForLedge()
    {
        //Determines that the Character is currently not falling after hanging from a ledge; this doesn't mean the Character isn't falling, but hasn't just pressed the input to allow the Character to fall off a platform when it was just previously hanging from it
        if (!falling && !jump.downwardJumping)
        {
            //This is a universal way to see if the Character sprite is facing right
            if (transform.localScale.x > 0)
            {
                //Sets the top of the Character collider and just barely in front of it so the RaycastHit2D can perform a better check
                topOfPlayer = new Vector2(col.bounds.max.x + .1f, col.bounds.max.y);
                //This will check to see if the topOfPlayer value is colliding with anything
                RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.right, .2f);
                //Checks to see if the topOfPlayer value is colliding with a platform that contains the Ledge script
                if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                {
                    //Sets the ledge value to whatever platform the Character is colliding with
                    ledge = hit.collider.gameObject;
                    //Sets a temprorary Collider2D variable to help clean up code and optimize
                    Collider2D ledgeCollider = ledge.GetComponent<Collider2D>();
                    //Checks to see top of the Character collider is below the top of the ledgeCollider and also that if the platform happens to be a one way platform that the Character isn't in the middle of the platform so it snaps to the ledge more accurately or allows the Charcter to pass through if the platform is a one way platform
                    if (col.bounds.max.y < ledgeCollider.bounds.max.y && col.bounds.max.y > ledgeCollider.bounds.center.y && col.bounds.min.x < ledgeCollider.bounds.min.x)
                    {
                        //Sets the GrabbingLedge bool to true
                        character.grabbingLedge = true;
                        //Runs the correct LedgeHanging animation
                        anim.SetBool("LedgeHanging", true);
                    }
                }
            }
            //This runs the exact same method as the big if statement above, but handles slightly different logic if the Character is facing left instead of right
            else
            {
                //Sets the top of the Character collider and just barely in front of it so the RaycastHit2D can perform a better check
                topOfPlayer = new Vector2(col.bounds.min.x - .1f, col.bounds.max.y);
                //This will check to see if the topOfPlayer value is colliding with anything
                RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.left, .2f);
                //Checks to see if the topOfPlayer value is colliding with a platform that contains the Ledge script
                if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                {
                    //Sets the ledge value to whatever platform the Character is colliding with
                    ledge = hit.collider.gameObject;
                    //Sets a temprorary Collider2D variable to help clean up code and optimize
                    Collider2D ledgeCollider = ledge.GetComponent<Collider2D>();
                    //Checks to see top of the Character collider is below the top of the ledgeCollider and also that if the platform happens to be a one way platform that the Character isn't in the middle of the platform so it snaps to the ledge more accurately or allows the Charcter to pass through if the platform is a one way platform
                    if (col.bounds.max.y < ledgeCollider.bounds.max.y && col.bounds.max.y > ledgeCollider.bounds.center.y && col.bounds.max.x > ledgeCollider.bounds.max.x)
                    {
                        //Sets the GrabbingLedge bool to true
                        anim.SetBool("LedgeHanging", true);
                        //Runs the correct LedgeHanging animation
                        character.grabbingLedge = true;
                    }
                }
            }
            //Checks to see if both there is a platform set as ledge and the value of GrabbingLedge is true
            if (ledge != null && character.grabbingLedge)
            {
                //Runs a method to have the Character snap to a more exact position on the platform based on a few different variables
                AdjustPlayerPosition();
                //Makes sure the player stops moving in all directions
                rb.velocity = Vector2.zero;
                //Turns off gravity for the Character while hanging from platform
                rb.bodyType = RigidbodyType2D.Kinematic;
                //I've set up my HorizontalMovement script to not run the movement logic if true, but if you're not sure how to set that up, use the line below
                //GetComponent<HorizontalMovement>().enabled = false;
            }
            else
            {
                //Turns on the gravity again if the Character is no longer hanging from ledge
                rb.bodyType = RigidbodyType2D.Dynamic;
                //I've set up my HorizontalMovement script to not run the movement logic if true, but if you're not sure how to set that up, use the line below
                //GetComponent<HorizontalMovement>().enabled = true;
            }
        }
    }

    //Handles what should happen when hanging from ledge and input receives up or down
    protected virtual void LedgeHanging()
    {
        //If Character is Grabbing ledge and the up button is pressed
        if (character.grabbingLedge && Input.GetAxis("Vertical") > 0 && !climbing)
        {
            //Stops the coroutine to allow the player to climb the ledge from playing multiple times when climbing
            climbing = true;
            //Stops playing the LedgeHanging bool
            anim.SetBool("LedgeHanging", false);
            //Makes sure the Character is facing right
            if (transform.localScale.x > 0)
            {
                //Method that runs to have the Character Lerp to top of platform based on the animationTime variable
                StartCoroutine(ClimbingLedge(new Vector2(transform.position.x + climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y + col.bounds.extents.y), animationTime));
            }
            //Makes sure the Character is facing left
            else
            {
                //Method that runs to have the Character Lerp to top of platform based on the animationTime variable
                StartCoroutine(ClimbingLedge(new Vector2(transform.position.x - climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y + col.bounds.extents.y), animationTime));
            }
        }
        //If Character is Grabbing ledge and the down button is pressed
        if (character.grabbingLedge && Input.GetAxis("Vertical") < 0)
        {
            //Sets the ledge value to null so there is no ledge stored
            ledge = null;
            //Sets the moved bool back to false so when the Character grabs another ledge, the Character can snap to the correct position based on the AdjustPlayerPosition() method
            moved = false;
            //Sets the Character state of GrabbingLedge to false so all the logic that needs to run if not GrabbingLedge can run
            character.grabbingLedge = false;
            //Stops playing the LedgeHanging animation
            anim.SetBool("LedgeHanging", false);
            //Sets teh falling bool to true very temporarily so the Character doesn't grab the same platform while falling from it
            falling = true;
            //Turns gravity back on for the Character
            rb.bodyType = RigidbodyType2D.Dynamic;
            //I've set up my HorizontalMovement script to not run the movement logic if true, but if you're not sure how to set that up, use the line below
            //GetComponent<HorizontalMovement>().enabled = true;
            //Runs the NotFalling method half a second later to make sure the falling bool gets set back to false quickly
            Invoke("NotFalling", .5f);
        }
    }

    //This method will handle moving the Character on top of the platform when this method is called
    protected virtual IEnumerator ClimbingLedge(Vector2 topOfPlatform, float duration)
    {
        //Sets a local time value to 0 for a fresh Lerp
        float time = 0;
        //Sets a local Vector2 value for a fresh Lerp
        Vector2 startValue = transform.position;
        //Runs this code in the while loop to have the Character position smoothly move on top of the platform while climbing it
        while (time <= duration)
        {
            //Plays the LedgeClimbing animation
            anim.SetBool("LedgeClimbing", true);
            //Handles the Character position based on where it started while hanging from platform to the top of the platform based on how long the LedgeClimbing animation is
            transform.position = Vector2.Lerp(startValue, topOfPlatform, time / duration);
            //Incriments the time value based on how long it has been since the Character started climbing the platform
            time += Time.deltaTime;
            //Allows to break out of the while loop when time is greater than the animation clip
            yield return null;
        }
        //Removes the ledge value and sets it back to null
        ledge = null;
        //Makes sure the moved bool is false just in case down
        moved = false;
        //Allows this coroutine to play again and prevent bad transitions
        climbing = false;
        //Gets the Character out of the GrabbingLedge state
        character.grabbingLedge = false;
        //Stops playing the LedgeClimbing animation
        anim.SetBool("LedgeClimbing", false);
    }

    //Method that snaps Character to the best on the platform based on a hangingHorizontalOffest and hangingVerticalOffset values found on the Ledge script of the platform
    protected virtual void AdjustPlayerPosition()
    {
        //If the moved bool is true, allows this logic to run
        if (!moved)
        {
            //Immedietly sets the moved bool to false so this method only runs once
            moved = true;
            //Quick local variable of the platform Collider2D component to clean up and optimize code
            Collider2D ledgeCollider = ledge.GetComponent<Collider2D>();
            //Quick local variable of the platform Ledge component to clean up and optimize code
            Ledge platform = ledge.GetComponent<Ledge>();
            //If the Character is facing right
            if (transform.localScale.x > 0)
            {
                //Snaps the Character position to the offset values found on the Ledge script as well as the platform Collider values for more exact placement when hanging from ledge
                transform.position = new Vector2((ledgeCollider.bounds.min.x - col.bounds.extents.x) + platform.hangingHorizontalOffset, (ledgeCollider.bounds.max.y - col.bounds.extents.y - .5f) + platform.hangingVerticalOffset);
            }
            //If the Character is facing left
            else
            {
                //Snaps the Character position to the offset values found on the Ledge script as well as the platform Collider values for more exact placement when hanging from ledge
                transform.position = new Vector2((ledgeCollider.bounds.max.x + col.bounds.extents.x) - platform.hangingHorizontalOffset, (ledgeCollider.bounds.max.y - col.bounds.extents.y - .5f) + platform.hangingVerticalOffset);
            }
        }
    }

    //Method called by Invoke when the Character is falling from a ledge rather than climb it
    protected virtual void NotFalling()
    {
        //Sets falling bool back to false so the Character doesn't grab the same platform it was hanging from when falling
        falling = false;
    }
}
