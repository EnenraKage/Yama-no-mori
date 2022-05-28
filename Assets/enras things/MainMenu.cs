using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSrc;
    public AudioClip selectSnd;
    public TextMeshProUGUI textSpeedtxt;
    public AudioMixer audioMixer;

    Resolution[] resolutions;

    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        PlayerPrefs.SetFloat("textSpeed", 0.07f);

        //gets all possible screen resolutions and puts them into an array
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        //the difference between a list and an array, is that an array has a fixed size, but the size of a list can be changed
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        //when typing a for loop, you can double tap tab to fill out the basic for loop example!!!! really good shortcut :]
        //adds all the screen resolutions to a list
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            //checks if the resolution at this index is that same as the current resolution
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        //adds possible resolutions to dropdown menu
        resolutionDropdown.AddOptions(options);

        //makes the current resolution the first one shown
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void FixedUpdate()
    {
        textSpeedtxt.text = PlayerPrefs.GetFloat("textSpeed").ToString();
    }

    public void playGame()
    {
        SelectSound();
        //getting the current scene and loading the one next to it in the scene order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame()
    {
        SelectSound();
        Application.Quit();
    }

    public void Fullscreen()
    {
        SelectSound();
        //if it's fullscreened make it windowed, and if it's windowed make it fullscreen
        if (Screen.fullScreen == false)
            Screen.fullScreen = true;
        else
            Screen.fullScreen = false;
    }

    public void SetResolution (int resolutionIndex)
    {
        //changing the resolution to the index of the resolution selected on the dropdown menu
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //unity will make the value float the position of the slider automatically :]
    public void TextSpeed(float value)
    {
        PlayerPrefs.SetFloat("textSpeed", value / 100);
    }

    public void Volume(float value)
    {
        audioMixer.SetFloat("volume", value);
    }

    public void SelectSound()
    {
        audioSrc.PlayOneShot(selectSnd);
    }
    //at some point use playerprefs for text speed and volume levels!!!!
}