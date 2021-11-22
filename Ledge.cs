using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to any platform the Character should hang off of if it collides with this platform
public class Ledge : MonoBehaviour
{
    //A value that helps the Character snap to the correct horizontal position when hanging from a ledge
    public float hangingHorizontalOffset = .5f;
    //A value that helps the Character snap to the correct vertical position when hanging from a ledge
    public float hangingVerticalOffset = .5f;
}