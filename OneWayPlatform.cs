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
    private float delay = .5f;
    //Reference of the player
    GameObject player;

    private void Start()
    {
        //Grabs a reference of the current collider on the platform
        col = GetComponent<Collider2D>();
        //
        player = FindObjectOfType<Character>().gameObject;
    }

    //Unity event that gets called once everytime something collides with the platform
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Checks to see if the gameobject colliding with the platform is the player
        if (collision.gameObject == player)
        {
            //Checks to see if the player is currently in a jumping state while also checking that the player is not above the platform so the player can stand on the platform while falling; then checkst to make sure the oneway platform type will allow the player to jump up through it
            if (!player.GetComponent<Character>().isGrounded && player.GetComponent<Collider2D>().bounds.min.y < col.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingUp))
            {
                //Sets the player as a gameobject that should ignore the platform collider so the player can pass through
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), col, true);
                //Runs coroutine to allow the player to collide with the platform again
                StartCoroutine(StopIgnoring());
            }
        }
    }

    //Unity event that constantly runs as long as something is colliding with the platform
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Checks to see if the gameobject colliding with the platform is the player
        if (collision.gameObject == player)
        {
            //Checks to see if the player is above the platform and the input detection setup on the Jump script is trying to jump downwards through the platform
            if (player.GetComponent<Jump>().fallingJump && player.GetComponent<Collider2D>().bounds.min.y > col.bounds.center.y && (type == OneWayPlatforms.Both || type == OneWayPlatforms.GoingDown))
            {
                //Sets the player as a gameobject that should ignore the platform collider so the player can pass through
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), col, true);
                StartCoroutine(StopIgnoring());
            }
        }
    }

    //Coroutine that toggles the collider on the platform to allow the player to collide with it again
    protected IEnumerator StopIgnoring()
    {
        //Waits a short delay setup at the top of this script in the variables
        yield return new WaitForSeconds(delay);
        //Sets the player as a gameobject that should collide the platform collider so the player can stand on it again
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), col, false);
    }
}
