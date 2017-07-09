using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: The main script for the gameplay
//---------------------------------------------------------------------------------
public class GlobalController : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================
    //For ground shake effects
    public GameObject Plateform;
    public static bool GroundShake = false;
    public static bool GroundDrop = false;
    public static float PlateformShakeTime = 0.5f;
    public float GroundDropTime = 5f;

    //Wave setter
    public static Meteor.Round CurrentRound = Meteor.Round.r0;

    //Scoring and Health
    public static int ResourceUnit = 0;
    public static int GroundIntegrity = 100;
    public static int Highscore;

    //UI Audio Controls
    public AudioClip SuccessAudio;
    public AudioClip FailAudio;
    public AudioClip LevelupAudio;
    public static AudioClip LevelupAudioStatic;

    //Vibration Setting
    public static bool AllowShake = true;

    //===================
    // Private Variables
    //===================
    //For ground shake effects
    private float shakedist = 0.01f;
    private float currentshaked = 0f;
    private float toshake = .05f;
    private bool shakedown = true;
    private Vector2 originalPlateformPos;
    private float groundDropTime;

    //Ground integrity repair values
    private static int repairCost = 33;
    private static int repairRate = 10;
    private static int maxGI = 100;

    //Wave win condition is reaching certain resource count
    private static int resourceGoal;

    //UI Audio Controls
    private static AudioSource uiAudio;

    protected void Start() 
	{
        GameObject.Find("GestureObject").GetComponent<TapHandler>().enabled = true;
        GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = true;

        //Stop the ground from shaking upon start, when player dies and starts again
        GroundDrop = false;
        originalPlateformPos = Plateform.transform.position;
        groundDropTime = GroundDropTime;

        //Update the resource and ground integrity UI to initial values
        UpdateScores();
        Highscore = PlayerPrefs.GetInt("HighScore");

        //initialise ui audio
        uiAudio = GetComponents<AudioSource>()[1];
        Debug.Log("Audio UI: " + uiAudio.outputAudioMixerGroup.audioMixer);
        LevelupAudioStatic = LevelupAudio;
        uiAudio.PlayOneShot(LevelupAudioStatic, 1);

        //Start the round
        ChangeResource(200);
        CurrentRound = Meteor.Round.r0;
        IncreaseRound();
    }

	protected void Update() 
	{
        //play defeated method will change ground drop to true the ground will start shaking the dropping when defeated
        if (GroundDrop == true && groundDropTime > 0)
        {
            groundDropTime = groundDropTime - Time.deltaTime;
            GroundShake = false;
            //Shaking and dropping the ground by translation and rotation per frame
            Plateform.transform.Translate(0f, Random.Range(-0.2f, 0.05f) * Time.deltaTime * 60, 0f);
            Plateform.transform.Rotate(0, 0, Random.Range(-1f, 0.5f));
            GameObject.Find("Atmosphere").GetComponent<Transform>().Translate(0f, Random.Range(-0.2f, 0.05f) * Time.deltaTime * 60, 0f);
            GameObject.Find("Atmosphere").GetComponent<Transform>().Rotate(0, 0, Random.Range(-1f, 0.5f) * Time.deltaTime * 60); ;
        }

        //Big enough meteors will shake the ground when hit
        if (GroundShake == true)
        {
            //Time to shake is reduced every shake
            PlateformShakeTime = PlateformShakeTime - Time.deltaTime;
            //Shake ground by translation
            if (shakedown)
            {
                currentshaked = currentshaked - shakedist;
                Plateform.transform.Translate(0f, -shakedist * Time.deltaTime * 60, 0);
            }
            else
            {
                currentshaked = currentshaked + shakedist;
                Plateform.transform.Translate(0f, shakedist * Time.deltaTime * 60, 0);
            }
            if (currentshaked <= (-toshake))
            {
                shakedown = false;
            }
            if (currentshaked >= 0)
            {
                shakedown = true;
            }
            //Stop ground shake if the shake time is over
            if (PlateformShakeTime <= 0)
            {
                Plateform.transform.position = originalPlateformPos;
                GroundShake = false;
                currentshaked = 0f;
                PlateformShakeTime = 0.5f;
            }
        }

	}

    //Called when objective is reached
    private static void IncreaseRound()
    {
        CurrentRound++;
        uiAudio.PlayOneShot(LevelupAudioStatic, 1);
        //Check and do the wave parameters
        switch (CurrentRound)
        {
            case Meteor.Round.r1:
                Meteor.CurrentRound = Meteor.Round.r1;
                Physics2D.gravity = new Vector2(0, -9.81f);
                maxGI = 100;
                repairRate = 10;
                repairCost = 33;
                resourceGoal = 500;
                //GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = false;
                //GameObject.Find("RocketTower").transform.position = new Vector2(rocketTowerPosition.x - 10, rocketTowerPosition.y);
                GameObject.Find("TxtRoundGoal").GetComponent<Text>().text = "Wave 1\nReach 500 Resource Units";
                GameObject.Find("PanelRoundGoal").GetComponent<UIFeedBacks>().ShowRoundGoal();
                GameObject.Find("TxtRepair").GetComponent<Text>().text = "Repair 10 Ground Integrity with 33 Resource units";
                break;
            case Meteor.Round.r2:
                Meteor.CurrentRound = Meteor.Round.r2;
                resourceGoal = 1000;
                //enabling rocket tower for wave 2
                //GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = true;
                //GameObject.Find("RocketTower").transform.position = rocketTowerPosition;
                GameObject.Find("TxtRoundGoal").GetComponent<Text>().text = "Wave 2\nReach 1000 Resource Units";
                GameObject.Find("PanelRoundGoal").GetComponent<UIFeedBacks>().ShowRoundGoal();
                break;
                //Endless play after wave 2 with increasing difficulty periodically
            default:
                resourceGoal = int.MaxValue;
                Time.timeScale = 0;
                GameObject.Find("Spawn").GetComponent<Meteor>().StartRamping();
                GameObject.Find("BtnMenu").GetComponent<Button>().enabled = false;
                GameObject.Find("BtnRepair").GetComponent<Button>().enabled = false;
                GameObject.Find("BtnViewHelp").GetComponent<Button>().enabled = false;
                GameObject.Find("GestureObject").GetComponent<TapHandler>().enabled = false;
                GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = false;
                GameObject.Find("CanvasSucc").GetComponent<Canvas>().enabled = true;
                GameObject.Find("TxtRoundGoal").GetComponent<Text>().text = "Endless Play with ramping.\nGround Integrity now max 1000 and repair rate is 100.";
                GameObject.Find("PanelRoundGoal").GetComponent<UIFeedBacks>().ShowRoundGoal(5.0f);
                GameObject.Find("TxtRepair").GetComponent<Text>().text = "Repair 100 Ground Integrity with 100 Resource units";
                maxGI = 1000;
                repairRate = 100;
                repairCost = 100;
                break;
        }

    }

    //Accesser method to shake the ground
    public static void ShakeGround()
    {
        GroundShake = true;
        PlateformShakeTime = 0.5f;
        if (AllowShake)
        {
            Handheld.Vibrate();
        }
    }

    //When repair button is pressed
    public void RepairGround()
    {
        //Check if the purchase is valid
        if (ResourceUnit - repairCost >= 0 && GroundIntegrity + repairRate <= maxGI)
        {
            ChangeResource(-repairCost);
            ChangeGroundIntegrity(repairRate);
            //Display positive UI feed back
            GameObject.Find("TxtRepaired").GetComponent<UIFeedBacks>().DisplayText();
            uiAudio.PlayOneShot(SuccessAudio, 3);
        }
        else
        {
            //Display negative UI feed back
            GameObject.Find("TxtCantRepair").GetComponent<UIFeedBacks>().DisplayText();
            uiAudio.PlayOneShot(FailAudio);
        }
        
    }

    //Method to modify resource
    public static void ChangeResource(int RU)
    {
        if (GroundIntegrity == 0)
        {
            return;
        }
        ResourceUnit += RU;
        UpdateScores();
        if (RU >= 0)
        {
            //UI feedbacks
            GameObject.Find("MoneyUI").GetComponent<UIFeedBacks>().ShowGreenText();
        }
        else
        {
            GameObject.Find("MoneyUI").GetComponent<UIFeedBacks>().ShowRedText();
        }
        if (ResourceUnit + GroundIntegrity > Highscore)
        {
            Highscore = ResourceUnit + GroundIntegrity;
            GameObject.Find("TxtCurrentHighscore").GetComponent<Text>().text = "Current Highscore: " + Highscore;
            //Save highscore
            PlayerPrefs.SetInt("HighScore", Highscore);
        }
        //Check if wave objective is reached
        if (ResourceUnit >= resourceGoal)
        {
            IncreaseRound();
        }
        
    }
    //Method to modify ground integrity
    public static void ChangeGroundIntegrity(int GI)
    {
        if (GroundIntegrity + GI <= 0)
        {
            GroundIntegrity = 0;
            GI = 0;
        }
        GroundIntegrity += GI;
        UpdateScores();
        if (GI>0)
        {
            GameObject.Find("HealthUI").GetComponent<UIFeedBacks>().ShowGreenText();
        }
        else
        {
            GameObject.Find("HealthUI").GetComponent<UIFeedBacks>().ShowRedText();
        }
        // below 0 means the player has defeated
        if (ResourceUnit + GroundIntegrity > Highscore)
        {
            Highscore = ResourceUnit + GroundIntegrity;
            GameObject.Find("TxtCurrentHighscore").GetComponent<Text>().text = "Current Highscore: " + Highscore;
            //Save highscore
            PlayerPrefs.SetInt("HighScore", Highscore);
        }
        if (GroundIntegrity <= 0)
        {
            //show death screen UI
            GameObject.Find("CanvasDeath").GetComponent<Canvas>().enabled = true;
            GameObject.Find("BtnMenu").GetComponent<Button>().enabled = false;
            GameObject.Find("BtnRepair").GetComponent<Button>().enabled = false;
            GameObject.Find("BtnViewHelp").GetComponent<Button>().enabled = false;
            GameObject.Find("GestureObject").GetComponent<TapHandler>().enabled = false;
            GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = false;
            GlobalFiring.LaserAutoFire = false;

            //drop the ground
            GroundDrop = true;
        }
    }

    //Refreshing UI values for resource and groud integrity
    public static void UpdateScores()
    {
        GameObject.Find("HealthUI").GetComponent<Text>().text = "Ground Integrity: " + GroundIntegrity;
        GameObject.Find("MoneyUI").GetComponent<Text>().text = "Resource: " + ResourceUnit;
    }

    //Pause and resume functions
    public void PauseGame()
    {
        Time.timeScale = 0;
        //Disable use of certain in game UI and controls
        GameObject.Find("GestureObject").GetComponent<TapHandler>().enabled = false;
        GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = false;
        GameObject.Find("BtnMenu").GetComponent<Button>().enabled = false;
        GameObject.Find("BtnRepair").GetComponent<Button>().enabled = false;
        GameObject.Find("BtnViewHelp").GetComponent<Button>().enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameObject.Find("GestureObject").GetComponent<TapHandler>().enabled = true;
        GameObject.Find("GestureObject").GetComponent<LongPressHandler>().enabled = true;
        GameObject.Find("BtnMenu").GetComponent<Button>().enabled = true;
        GameObject.Find("BtnRepair").GetComponent<Button>().enabled = true;
        GameObject.Find("BtnViewHelp").GetComponent<Button>().enabled = true;
    }
}
