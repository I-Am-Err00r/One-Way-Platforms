using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    //Enum that sets up different types for one way platforms
    [SerializeField]
    protected enum OneWayPlatforms { GoingUp, GoingDown, Both }
    [SerializeField]
    protected OneWayPlatforms type;
    //The collider on the platform
    private Collider2D col;
    //A short delay to allow the player to collide with the platform again
    [SerializeField]
    private float delay = .5f;
    //Reference of the player
    private GameObject player;
    //Reference to the collider on the player
    private Collider2D playerCollider;

    private void Start()
    {
        //Grabs a reference of the current collider on the platform
        col = GetComponent<Collider2D>();
        //Less optimal way to find Player
        //player = GameObject.FindGameObjectWithTag("Player");
        //More optimal way to find Player; requires some sort of script that only the active player in the scene would have
        player = FindObjectOfType<Character>().gameObject;
        playerCollider = player.GetComponent<Collider2D>();
    }


    //Unity event that gets called once everytime something collides with the platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Checks to see if the gameobject colliding with the platform is the player
        if (collision.gameObject == player)
        {
            //Checks to see if player is not above the platform so the player can stand on the platform while jumping and then checks to see if the platform will allow the player to jump up through it;
            if (playerCollider.bounds.min.y < col.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingUp))
            {
                //Sets the player as a gameobject that should ignore the platform collider so the player can pass through
                Physics2D.IgnoreCollision(playerCollider, col, true);
                //Sets the jump passingThroughPlatform bool to true so it doesn't enter the grounded state while passing through it
                player.GetComponent<Jump>().passedThroughPlatform = col;
                //Runs coroutine to allow the player to collide with the platform again
                StartCoroutine(StopIgnoring());
            }
        }
    }

    //This handles falling through a one way platform if standing on top of platform
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Checks to see if the gameobject colliding with the platform is the player
        if (collision.gameObject == player)
        {
            //Checks to see if the Input allows for a downward jump and that the player is actually on top of the one way platform
            if (player.GetComponent<Jump>().downJumpPressed && playerCollider.bounds.min.y > col.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingDown))
            {
                //Sets the player as a gameobject that should ignore the platform collider so the player can pass through
                Physics2D.IgnoreCollision(playerCollider, col, true);
                //Sets the jump passingThroughPlatform bool to true so it doesn't enter the grounded state while passing through it
                player.GetComponent<Jump>().passedThroughPlatform = col;
                //Sets the animator Grounded bool to false so it can enter the falling animation
                player.GetComponent<Animator>().SetBool("Grounded", false);
                //Runs coroutine to allow the player to collide with the platform again
                StartCoroutine(StopIgnoring());
            }
        }
    }

    //Coroutine that toggles the collider on the platform to allow the player to collide with it again
    private IEnumerator StopIgnoring()
    {
        //Waits a short delay setup at the top of this script in the variables
        yield return new WaitForSeconds(delay);
        //Sets the player as a gameobject that should collide the platform collider so the player can stand on it again
        Physics2D.IgnoreCollision(playerCollider, col, false);
        //Sets the jump passingThroughPlatform bool to false so the player can enter the grounded state again
        player.GetComponent<Jump>().passedThroughPlatform = null;
    }
}
