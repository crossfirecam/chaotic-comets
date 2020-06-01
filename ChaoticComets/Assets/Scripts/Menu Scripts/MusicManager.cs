using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
    {

    public AudioMixer mixer;
    public AudioSource sfxDemo, currentMusicPlayer;
    public AudioClip musicMainMenu, musicGameplay, musicShop, musicTutorial;
    private int lastTrackRequested = -1; // When first created, pick the scene's chosen song

    public static MusicManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ChangeMusic(float sliderValue) {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
        BetweenScenesScript.MusicVolume = sliderValue;
    }

    public void ChangeSFX(float sliderValue) {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFX", sliderValue);
        BetweenScenesScript.SFXVolume = sliderValue;
        if (!sfxDemo.isPlaying) {
            sfxDemo.Play();
        }
    }

    public void FindAllSfxAndPlayPause(int intent)
    {
        List<GameObject> listOfSfxObjects = new List<GameObject>();
        string[] tagsToFind = { "Player", "Player 2", "ufo", "bullet", "bullet2", "bullet3", "bullet4", "powerup", "MiscSounds"};
        foreach (var tag in tagsToFind)
        {
            GameObject[] listOfAudioFound = { };
            if (tag == "ufo")
            {
                GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag(tag);
                List<GameObject> listOfUfoSfx = new List<GameObject>();
                foreach (GameObject ufo in listOfUfos)
                    listOfUfoSfx.AddRange(ufo.GetComponent<Ufo>().ReturnAlienSounds());
                listOfAudioFound = listOfUfoSfx.ToArray();
            }
            else if (tag == "Player" || tag == "Player 2")
            {
                GameObject player = GameObject.FindGameObjectWithTag(tag);
                if (player != null)
                    listOfAudioFound = player.GetComponent<PlayerUiSounds>().ReturnPlayerSounds();
            }
            else
            {
                listOfAudioFound = GameObject.FindGameObjectsWithTag(tag);
            }
            listOfSfxObjects.AddRange(listOfAudioFound);

        }

        if (intent == 0) // Pause
        {
            foreach (GameObject sfxObject in listOfSfxObjects)
            {
                if (sfxObject.GetComponent<AudioSource>().isPlaying)
                {
                    sfxObject.GetComponent<AudioSource>().Pause();
                }
            }
        }
        if (intent == 1) // Resume
        {
            foreach (GameObject sfxObject in listOfSfxObjects)
            {
                if (!sfxObject.GetComponent<AudioSource>().isPlaying)
                    sfxObject.GetComponent<AudioSource>().UnPause();
            }
        }
    }

    public void ChangeMusicTrack(int index)
    {
        Debug.Log($"Music requested: {index} Music last played: {lastTrackRequested}");
        if (index != lastTrackRequested)
        {
            currentMusicPlayer.enabled = true;
            if (currentMusicPlayer.isPlaying)
                currentMusicPlayer.Stop();
            switch (index)
            {
                case 0:
                    currentMusicPlayer.clip = musicMainMenu;
                    break;

                case 1:
                    currentMusicPlayer.clip = musicGameplay;
                    break;

                case 2:
                    currentMusicPlayer.clip = musicShop;
                    break;

                case 3:
                    currentMusicPlayer.clip = musicTutorial;
                    break;
            }
            currentMusicPlayer.Play();
            lastTrackRequested = index;
        }
    }

    public void PauseMusic()
    {
        if (currentMusicPlayer.isPlaying)
            currentMusicPlayer.Pause();
    }
    public void ResumeMusic()
    {
        if (!currentMusicPlayer.isPlaying)
            currentMusicPlayer.UnPause();
    }
}
