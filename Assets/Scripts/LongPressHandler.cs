using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Hit;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Handling controls for rockets
//---------------------------------------------------------------------------------
public class LongPressHandler : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================

    //===================
    // Private Variables
    //===================

    //---------------------------------------------------------------------------------
    // protected mono methods. 
    // Unity5: Rigidbody, Collider, Audio and other Components need to use GetComponent<name>()
    //---------------------------------------------------------------------------------

    private void OnEnable()
    {
        GetComponent<LongPressGesture>().LongPressed += LongPressedHandle;
    }

    private void OnDisable()
    {
        Debug.Log("Long Press disabled");
        GetComponent<LongPressGesture>().LongPressed -= LongPressedHandle;
    }

    private void LongPressedHandle(object sender, System.EventArgs e)
    {
        //Get hit location
        var gesture = sender as LongPressGesture;
        TouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Debug.Log("Long Pressed!" + hit.Point);

        GlobalFiring.LaserAutoFire = (GlobalFiring.LaserAutoFire) ? false : true;

        //RocketScript.Target = hit.Point;
        ////Unshow idle rocket after firing one
        //RocketIdle.GetComponent<Renderer>().enabled = false;
        ////show idle rocket after sometime
        //Invoke("RespawnRocket", 0.75f);
        ////Create rocket missle
        //GameObject rt = Instantiate(Rocket, FirePoint.position, Quaternion.identity);
    }

    //private void RespawnRocket()
    //{
    //    RocketIdle.GetComponent<Renderer>().enabled = true;
    //}
}
