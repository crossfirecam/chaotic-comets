using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Constants;

public class MusicManager : MonoBehaviour
    {

    public AudioMixer mixer;
    public AudioSource sfxDemo, currentMusicPlayer;
    public AudioClip musicMainMenu, musicGameplay, musicShop, musicTutorial;
    private int lastTrackRequested = -1; // When first created, pick the scene's chosen song

    public static MusicManager i;
    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ChangeMusic(float sliderValue) {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
        PlayerPrefs.Save();
    }

    public void ChangeSFX(float sliderValue) {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFX", sliderValue);
        PlayerPrefs.Save();
        if (!sfxDemo.isPlaying) {
            sfxDemo.Play();
        }
    }

    public void FindAllSfxAndPlayPause(int intent)
    {
        List<AudioSource> listOfSfxObjects = new List<AudioSource>();
        string[] tagsToFind = { Tag_Player1, Tag_Player2, Tag_Ufo, Tag_BulletP1, Tag_BulletP2, Tag_BulletUfoGreen, Tag_BulletUfoRed, Tag_Canister, Tag_Other_MiscSounds};
        foreach (var tag in tagsToFind)
        {
            // Find all UFO noises
            if (tag == Tag_Ufo)
            {
                GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject ufo in listOfUfos)
                    listOfSfxObjects.AddRange(ufo.GetComponent<Ufo>().ReturnAlienSounds());
            }
            // Find all Player noises
            else if (tag == Tag_Player1 || tag == Tag_Player2)
            {
                GameObject player = GameObject.FindGameObjectWithTag(tag);
                if (player != null)
                    listOfSfxObjects.AddRange(player.GetComponent<PlayerUiSounds>().ReturnPlayerSounds());
            }
            // Find all other noises
            else
            {
                GameObject[] otherObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject other in otherObjects)
                    listOfSfxObjects.Add(other.GetComponent<AudioSource>());
            }
        }

        if (intent == 0) // Pause
        {
            foreach (AudioSource sfxObject in listOfSfxObjects)
            {
                if (sfxObject.isPlaying)
                {
                    sfxObject.Pause();
                }
            }
        }
        if (intent == 1) // Resume
        {
            foreach (AudioSource sfxObject in listOfSfxObjects)
            {
                if (!sfxObject.isPlaying)
                    sfxObject.UnPause();
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
