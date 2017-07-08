using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFiring : MonoBehaviour
{
    public static List<GameObject> LaserTowers;
    public static List<GameObject> RocketTowers;
    public static List<GameObject> SonicwaveTowers;

    private List<float> laserTowerCD;
    private float rocketTowerCD;

    public static Vector2 LaserTarget;
    public static bool LaserAutoFire;

    public static Vector2 RocketTarget;

    public GameObject LaserBeam;
    public GameObject Rocket;
    public GameObject SonicWave;

    public GameObject LaserTower;
    public GameObject RocketTower;
    public GameObject SonicWaveTower;

    public int LaserTowerCost;
    public int RocketTowerCost;
    public int SonicTowerCost;

    public float LaserFireRate;
    public float RocketTowerCD;
    public float LaserTowerCD;

    private int laserTowerOrder = 0;

    private bool isPlacingTower = false;
    private GameObject towerToPlace;

    // Use this for initialization
    void Start()
    {
        LaserTowers = new List<GameObject>();
        RocketTowers = new List<GameObject>();
        SonicwaveTowers = new List<GameObject>();

        laserTowerCD = new List<float>();

        LaserTarget = new Vector2();
        LaserAutoFire = false;

        RocketTarget = new Vector2();

        InvokeRepeating("AutoLasers", 0, 0.1f);
    }

    public void PlaceLaserTower()
    {
        if (GlobalController.ResourceUnit < LaserTowerCost)
        {
            GameObject.Find("TowerHint").GetComponent<Text>().text = "Not enough resource!";
            GameObject.Find("TowerHint").GetComponent<UIFeedBacks>().DisplayText();
            GameObject.Find("BtnCloseTower").SetActive(false);
            return;
        }
        towerToPlace = LaserTower;
        Invoke("StartPlacingTowers", 0.1f);
    }

    public void PlaceRocketTower()
    {
        if (GlobalController.ResourceUnit < RocketTowerCost)
        {
            GameObject.Find("TowerHint").GetComponent<Text>().text = "Not enough resource!";
            GameObject.Find("TowerHint").GetComponent<UIFeedBacks>().DisplayText();
            GameObject.Find("BtnCloseTower").SetActive(false);
            return;
        }
        towerToPlace = RocketTower;
        Invoke("StartPlacingTowers", 0.1f);
    }

    private void StartPlacingTowers()
    {
        isPlacingTower = true;
        foreach (var btn in GameObject.FindGameObjectsWithTag("TowerButton"))
        {
            btn.GetComponent<Button>().interactable = false;
        }
        GameObject.Find("BtnCloseTower").GetComponent<Button>().enabled = true;
    }

    public void CancelPlacingTowers()
    {
        isPlacingTower = false;
        foreach (var btn in GameObject.FindGameObjectsWithTag("TowerButton"))
        {
            btn.GetComponent<Button>().interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacingTower)
        {
            if (Input.touchCount == 1)
            {
                Vector2 touchpositionVP = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);
                Vector2 spawnpos = Camera.main.ViewportToWorldPoint(new Vector3(touchpositionVP.x, 0.3f));
                if (touchpositionVP.x > 0.08)
                {
                    if (IsTowerObstructed(spawnpos))
                    {
                        return;
                    }
                    Instantiate(towerToPlace, spawnpos, Quaternion.identity);
                    if (towerToPlace.tag == "LaserTower")
                    {
                        GlobalController.ChangeResource(-LaserTowerCost);
                    }
                    if (towerToPlace.tag == "RocketTower")
                    {
                        GlobalController.ChangeResource(-RocketTowerCost);
                    }
                    isPlacingTower = false;
                    foreach (var btn in GameObject.FindGameObjectsWithTag("TowerButton"))
                    {
                        btn.GetComponent<Button>().interactable = true;
                    }
                    GameObject.Find("BtnCloseTower").SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < laserTowerCD.Count; i++)
        {
            laserTowerCD[i] = (laserTowerCD[i] > 0) ? laserTowerCD[i] - Time.fixedDeltaTime : 0f;
        }
        rocketTowerCD = (rocketTowerCD > 0) ? rocketTowerCD - Time.fixedDeltaTime : 0f;
    }

    private bool IsTowerObstructed(Vector2 spawnpos)
    {
        foreach (var tower in LaserTowers)
        {
            if (IsTowerObstructedType(tower, spawnpos))
            {
                return true;
            }
        }
        var rockettowers = GameObject.FindGameObjectsWithTag("RocketTower");
        if (RocketTowers.Count != rockettowers.Length)
        {
            RocketTowers.Clear();
            foreach (var rockettower in rockettowers)
            {
                RocketTowers.Add(rockettower);
            }
        }
        foreach (var tower in RocketTowers)
        {
            if (IsTowerObstructedType(tower, spawnpos))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTowerObstructedType(GameObject tower, Vector2 spawnpos)
    {
        //TODO: Better calculation needed for obstruction detection
        float extend = tower.GetComponent<Collider2D>().bounds.extents.x;
        float xposmin = tower.transform.position.x - extend * 2f;
        float xposmax = tower.transform.position.x + extend * 2f;
        Debug.Log(spawnpos.x + "vs" + xposmin);
        if (spawnpos.x > xposmin && spawnpos.x < xposmax)
        {
            GameObject.Find("TowerHint").GetComponent<Text>().text = "Not enough space!";
            GameObject.Find("TowerHint").GetComponent<UIFeedBacks>().DisplayText();
            return true;
        }
        if (spawnpos.x > xposmin && spawnpos.x < xposmax)
        {
            GameObject.Find("TowerHint").GetComponent<Text>().text = "Not enough space!";
            GameObject.Find("TowerHint").GetComponent<UIFeedBacks>().DisplayText();
            return true;
        }
        return false;
    }

    public void FireLasers()
    {
        var lasertowers = GameObject.FindGameObjectsWithTag("LaserTower");
        if (LaserTowers.Count != lasertowers.Length)
        {
            LaserTowers.Clear();
            laserTowerCD.Clear();
            foreach (var item in lasertowers)
            {
                LaserTowers.Add(item);
                laserTowerCD.Add(0);
            }
            laserTowerOrder = 0;
        }

        float delayfire = 0;
        foreach (var currentlasertower in LaserTowers)
        {
            Invoke("FireLaser", delayfire);
            delayfire = delayfire + LaserFireRate;
        }
    }

    private void FireLaser()
    {
        Debug.Log("Count:" + LaserTowers.Count);
        laserTowerOrder = (laserTowerOrder % LaserTowers.Count == 0) ? 0 : laserTowerOrder;
        var lasertower = LaserTowers[laserTowerOrder];
        laserTowerOrder++;
        if (laserTowerCD[laserTowerOrder - 1] > 0)
        {
            return;
        }
        var turret = lasertower.transform.Find("LTwrTurrentIdle");
        var firePoint = turret.Find("LTTFirePoint");
        //Find fire direction
        Vector2 direction = LaserTarget - (Vector2)firePoint.position;
        direction = direction.normalized;

        //animate turret fire
        turret.GetComponent<Animator>().SetTrigger("Fire");

        //rotate turret
        Vector2 relativePos = LaserTarget - (Vector2)turret.transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        turret.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        GameObject lb = Instantiate(LaserBeam, firePoint.position, Quaternion.LookRotation(direction));
        lb.GetComponent<Rigidbody2D>().velocity = direction;
        laserTowerCD[laserTowerOrder - 1] = LaserTowerCD;
    }

    private void AutoLasers()
    {
        if (LaserAutoFire)
        {
            FireLasers();
        }
    }

    public void FireRockets()
    {
        if (rocketTowerCD > 0)
        {
            return;
        }
        var rockettowers = GameObject.FindGameObjectsWithTag("RocketTower");
        if (RocketTowers.Count != rockettowers.Length)
        {
            RocketTowers.Clear();
            foreach (var rockettower in rockettowers)
            {
                RocketTowers.Add(rockettower);
            }
        }
        foreach (var rockettower in RocketTowers)
        {
            rockettower.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = false;
            Invoke("RespawnRocket", 0.75f);
            GameObject rt = Instantiate(Rocket, rockettower.transform.GetChild(0).GetChild(0).position, Quaternion.identity);
            RocketScript.Target = RocketTarget;
        }
        rocketTowerCD = RocketTowerCD;
    }

    private void RespawnRocket()
    {
        foreach (var rockettower in RocketTowers)
        {
            rockettower.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = true;
        }
    }
}
