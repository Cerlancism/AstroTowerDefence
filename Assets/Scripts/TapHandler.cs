using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using System;
using TouchScript.Hit;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Handling laser beam
//---------------------------------------------------------------------------------
public class TapHandler : MonoBehaviour 
{
    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += TapHandle;
    }

    private void OnDisable()
    {

        Debug.Log("Tap Disabled!");
        GetComponent<TapGesture>().Tapped -= TapHandle;

    }

    private void TapHandle(object sender, EventArgs e)
    {
        //get hit location
        var gesture = sender as TapGesture;
        TouchHit hit;
        gesture.GetTargetHitResult(out hit);

        Vector3 target = hit.Point;
        GlobalFiring.LaserTarget = target;

        if (!GlobalFiring.LaserAutoFire)
        {
            GameObject.Find("GlobalManager").GetComponent<GlobalFiring>().FireLasers();
        }


        //Find fire direction
        //Vector3 direction = target - FirePoint.position;
        //direction = direction.normalized;

        ////animate turret fire
        //Turret.GetComponent<Animator>().SetTrigger("Fire");

        ////rotate turret
        //Vector3 relativePos = target - Turret.transform.position;
        //float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        //Turret.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        //GameObject lb = Instantiate(LaserBeam, FirePoint.position, Quaternion.identity);
        //lb.GetComponent<Rigidbody2D>().velocity = direction;
    }

    protected void Start() 
	{
	}

	protected void Update() 
	{
        //force idle animation after playing fire animation
        //if (Turret.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LaserTowerFireAni"))
        //{
        //    Turret.GetComponent<Animator>().SetInteger("State", 0);
        //}

    }
}

