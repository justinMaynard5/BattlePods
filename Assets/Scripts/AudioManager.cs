using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public SoundSet[] sfxLibrary;
    public AudioClip[] musicLibrary;
    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }
    public enum AudioChannel { Master, Sfx, Music };

    Dictionary<string, AudioClip[]> setDictionary = new Dictionary<string, AudioClip[]>();
    AudioSource sfxSource;
    AudioSource musicSource;
    int currentMusicTrack = 0;

    // Use this for initialization
    void Awake()
    {
        foreach (SoundSet soundSet in sfxLibrary)
        {
            setDictionary.Add(soundSet.name, soundSet.sounds);
        }
    }

    void Start ()
    {
        if (instance == null)
        {
            instance = this;

            GameObject newSfxsource = new GameObject("sfxSource");
            sfxSource = newSfxsource.AddComponent<AudioSource>();
            newSfxsource.transform.parent = transform;

            musicSource = GameObject.Find("musicSource").GetComponent<AudioSource>();

            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);

            musicSource.PlayOneShot(musicLibrary[currentMusicTrack]);
            StartCoroutine(MusicSwitch(musicLibrary[currentMusicTrack].length));
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySfx(string set)
    {
        sfxSource.PlayOneShot(setDictionary[set][Random.Range(0, setDictionary[set].Length)],sfxVolumePercent * masterVolumePercent);
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicSource.volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    IEnumerator MusicSwitch(float trackTime)
    {
        yield return new WaitForSeconds(trackTime);

        currentMusicTrack++;
        if (currentMusicTrack >= musicLibrary.Length)
        {
            currentMusicTrack = 0;
        }

        musicSource.PlayOneShot(musicLibrary[currentMusicTrack]);
        StartCoroutine(MusicSwitch(musicLibrary[currentMusicTrack].length));
    }

    [System.Serializable]
    public class SoundSet
    {
        public string name;
        public AudioClip[] sounds;
    }
}
