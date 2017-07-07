using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Hit;
using TouchScript.Gestures;
using System;

public class FlickHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        GetComponent<FlickGesture>().Flicked += FlickHandle;
    }

    private void OnDisable()
    {
        GetComponent<FlickGesture>().Flicked -= FlickHandle;
    }

    private void FlickHandle(object sender, EventArgs e)
    {
        //get hit location
        var gesture = sender as FlickGesture;
        TouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Vector2 screenposition = Camera.main.WorldToViewportPoint(hit.Point);

        GlobalFiring.RocketTarget = Camera.main.ViewportToWorldPoint(new Vector3(screenposition.x, screenposition.y, 0));

        GameObject.Find("GlobalManager").GetComponent<GlobalFiring>().FireRockets();
    }
}
