using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;

public class LoadScene : MonoBehaviour 
{
    public AudioMixer MasterAudio;

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
            if (!PlayerPrefs.HasKey("Vibrate"))
            {
                PlayerPrefs.SetInt("Vibrate", 1);
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
            if (PlayerPrefs.GetInt("Vibrate") == 0)
            {
                VibrateOff();
                GameObject.Find("NoVibrate").SetActive(false);
            }
            else
            {
                VibrateOn();
                GameObject.Find("Vibrate").SetActive(false);
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

    public void VibrateOn()
    {
        PlayerPrefs.SetInt("Vibrate", 1);
        Handheld.Vibrate();
        GlobalController.AllowShake = true;
    }

    public void VibrateOff()
    {
        PlayerPrefs.SetInt("Vibrate", 0);
        GlobalController.AllowShake = false;
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
        MasterAudio.SetFloat("SFXVolume", -80f);
        PlayerPrefs.SetInt("AudioVolume", 0);
    }

    public void UnMute()
    {
        MasterAudio.SetFloat("SFXVolume", 0f);
        PlayerPrefs.SetInt("AudioVolume", 1);
    }

    public void MusicMute()
    {
        MasterAudio.SetFloat("MusicVolume", -80f);
        PlayerPrefs.SetInt("MusicVolume", 0);
    }

    public void MusicUnMute()
    {
        MasterAudio.SetFloat("MusicVolume", 0f);
        PlayerPrefs.SetInt("MusicVolume", 1);
    }

}
