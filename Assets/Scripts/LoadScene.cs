using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour 
{
    private void Start()
    {
        //Audio volume settings, using try as SetActive may throw error as espcially reloaded menu after gameplay since its already inactive for some
        try
        {
            if (!PlayerPrefs.HasKey("AudioVolume"))
            {
                PlayerPrefs.SetInt("AudioVolume", 1);
            }
            if (!PlayerPrefs.HasKey("MusicVolume"))
            {
                PlayerPrefs.SetInt("MusicVolume", 1);
            }

            if (PlayerPrefs.GetInt("AudioVolume") == 0)
            {
                Mute();
                GameObject.Find("Mute").SetActive(false);
            }
            else
            {
                UnMute();
                GameObject.Find("Unmute").SetActive(false);
            }
            if (PlayerPrefs.GetInt("MusicVolume") == 0)
            {
                MusicMute();
                GameObject.Find("MusicMute").SetActive(false);
            }
            else
            {
                MusicUnMute();
                GameObject.Find("MusicUnmute").SetActive(false);
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
        }
        //highscore
        if (!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }
        GameObject.Find("TxtHighscore").GetComponent<Text>().text = "Highscore: " + PlayerPrefs.GetInt("HighScore");

    }

    //After pressing start in menu
    public void LoadEarth()
    {
        SceneManager.LoadScene(1);
        GlobalController.ResourceUnit = 0;
        GlobalController.GroundIntegrity = 100;
        Meteor.CurrentRound = Meteor.Round.r1;
    }

    //Quited gameplay back to menu
    public void LoadStart()
    {
        SceneManager.LoadScene(0);
    }

    //Audio setting methods
    public void Mute()
    {
        AudioListener.volume = 0;
        PlayerPrefs.SetInt("AudioVolume", 0);
    }

    public void UnMute()
    {
        AudioListener.volume = 1;
        PlayerPrefs.SetInt("AudioVolume", 1);
    }

    public void MusicMute()
    {
        GameObject.Find("GlobalManager").GetComponent<AudioSource>().Stop();
        PlayerPrefs.SetInt("MusicVolume", 0);
    }

    public void MusicUnMute()
    {
        GameObject.Find("GlobalManager").GetComponent<AudioSource>().Play(); ;
        PlayerPrefs.SetInt("MusicVolume", 1);
    }
}
