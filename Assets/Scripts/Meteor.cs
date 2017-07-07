using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Meteor spawner script
//---------------------------------------------------------------------------------
public class Meteor : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================
    //get all meteor prefabs
    public GameObject MeteorTiny;
    public GameObject MeteorSmall;
    public GameObject MeteorMid;
    public GameObject MeteorBig;
    public GameObject MeteorBigger;
    public GameObject MeteorBiggest;
    public GameObject MeteorLead;

    //Wave variables
    public enum Round { r0 = -1, r1, r2};
    public static Round CurrentRound = Round.r0;

    //===================
    // Private Variables
    //===================
    private float spawnRate = 1.0f;

    //---------------------------------------------------------------------------------
    // protected mono methods. 
    // Unity5: Rigidbody, Collider, Audio and other Components need to use GetComponent<name>()
    //---------------------------------------------------------------------------------

	//---------------------------------------------------------------------------------
	// Start is when blah blah
	//---------------------------------------------------------------------------------
	protected void Start() 
	{
        //Every 2 sec runs the spawning algorithm
        InvokeRepeating("AddMeteor", 2.0f, spawnRate);
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void Update() 
	{
	}

    //spawning algorithm
    void AddMeteor()
    {
        //get spawn location
        Renderer renderer = GetComponent<Renderer>();
        float x1 = transform.position.x - renderer.bounds.size.x / 2;
        float x2 = transform.position.x + renderer.bounds.size.x / 2;
        //rate to determine spawn chance
        int rate = 0;
        switch (CurrentRound)
        {
            case Round.r1:

                //big meteor has 33% spawn chance every sec
                rate = Random.Range(0, 101);
                rate = rate % 3;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorBig, spawnPoint, (Quaternion.identity));
                    rate = 1;
                }
                else
                {
                    rate = 0;
                }

                //mid meteor spawn 0 - 1 times every second, none if big meteor just spawned
                rate = Random.Range(0, 2 - rate);
                for (int i = 0; i < rate; i++)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorMid, spawnPoint, (Quaternion.identity));
                }

                //small meteor spawn 1 - 2 times every second and less depends if a mid has spawned
                rate = Random.Range(1, 3 - rate);
                for (int i = 0; i < rate; i++)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorSmall, spawnPoint, (Quaternion.identity));
                }
                break;

            case Round.r2:
                //Lead meteor has about 2% spawn chance every sec
                rate = Random.Range(0, 101);
                Debug.Log("Lead spawn determinant value: " + rate);
                rate = rate % 50;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorLead, spawnPoint, (Quaternion.identity));
                }



                //biggest meteor has 4% spawn chance every sec
                rate = Random.Range(0, 101);
                rate = rate % 25;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorBiggest, spawnPoint, (Quaternion.identity));
                }

                //bigger meteor has 8% spawn chance every sec
                rate = Random.Range(0, 101);
                rate = rate % 12;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorBigger, spawnPoint, (Quaternion.identity));
                }

                //big meteor has about 16% spawn chance every sec
                rate = Random.Range(0, 101);
                rate = rate % 6;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorBig, spawnPoint, (Quaternion.identity));
                }

                //mid meteor has about 32% spawnrate every sec
                rate = Random.Range(0, 101);
                rate = rate % 3;
                if (rate == 0)
                {
                    Vector2 spawnPoint = new Vector2(Random.Range(x1, x2), transform.position.y);
                    Instantiate(MeteorMid, spawnPoint, (Quaternion.identity));
                }
                break;
        }
        
    }

    //Ramping is for beyond wave 2 endless play with increasing calling rate to spawn meteors
    public void StartRamping()
    {
        CancelInvoke("AddMeteor");
        InvokeRepeating("RampingCounter", 0, 2f);
        Debug.Log("Ramping Rate: " + spawnRate);
        transform.Translate(-1, 1, 0);
        transform.localScale = new Vector3(transform.localScale.x * 4, transform.localScale.y * 2, 1);
    }

    private void RampingCounter()
    {
        if (spawnRate > 0.05f)
        {
            GameObject spawnLocation = GameObject.Find("Spawn");
            CancelInvoke("AddMeteor");
            InvokeRepeating("AddMeteor", 0, spawnRate = spawnRate * 0.99f);
            //move spawner higher so meteors will appear to drop faster
            Physics2D.gravity = Physics2D.gravity * 1.015f;
            Debug.Log("Spawnrate ramping: " + spawnRate);
        }
        else
        {
            GameObject.Find("TxtRoundGoal").GetComponent<Text>().text = "Ramping Maxed";
            GameObject.Find("PanelRoundGoal").GetComponent<UIFeedBacks>().ShowRoundGoal(2.0f);
            CancelInvoke("RampingCounter");
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }

}
