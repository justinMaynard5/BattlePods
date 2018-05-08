using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;

public class Menu : MonoBehaviour {

	public GameObject mainMenuHolder;
	public GameObject optionsMenuHolder;
    public GameObject creditsHolder;
    public GameObject howToPlayHolder;

	public Slider[] volumeSliders;
	public Toggle fullscreenToggle;
    public Dropdown resDropDown;
	public int[] screenWidths;
	int activeScreenResIndex;
    bool fullscreen;

    public EventSystem menuManagment;
    public GameObject play;
    public GameObject masterVolume;
    public GameObject back;
    public GameObject backHowToPlay;

    public AnimationClip mainMenuFade;

    public Animator[] mainMenuAnimator;
    public Animator[] optionsAnimator;
    public Animator[] creditsAnimator;
    public Animator mainManuFader;
    public Animator optionsFader;
    public Animator creditsFader;
    public GameObject company;
    public GameObject title;

    public GameObject[] slides;
    int currentSlide = 0;

    void Start() {
		activeScreenResIndex = PlayerPrefs.GetInt ("screen res index",0);
		if (PlayerPrefs.GetInt ("fullscreen", 0) == 0)
        {
            fullscreen = true;
        }
        else
        {
            fullscreen = false;
        }
        PlayerPrefs.Save ();
        fullscreenToggle.isOn = fullscreen;
        resDropDown.value = activeScreenResIndex;

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

		//fullscreenToggle.isOn = isFullscreen;

        foreach (Animator anim in mainMenuAnimator)
        {
            anim.enabled = false;
        }

        foreach (Animator anim in optionsAnimator)
        {
            anim.enabled = false;
        }

        foreach (Animator anim in creditsAnimator)
        {
            anim.enabled = false;
        }

        StartCoroutine(Title());
	}

    public void Update()
    {
        if (menuManagment.currentSelectedGameObject == null)
        {
            if (mainMenuHolder.activeInHierarchy)
            {
                menuManagment.SetSelectedGameObject(play);
            }
            else if (optionsMenuHolder.activeInHierarchy)
            {
                menuManagment.SetSelectedGameObject(masterVolume);
            }
            else if (creditsHolder.activeInHierarchy)
            {
                menuManagment.SetSelectedGameObject(back);
            }
        }

        if (howToPlayHolder.activeInHierarchy)
        {
            if (ReInput.players.GetSystemPlayer().GetButtonDown("UIHorizontal"))
            {
                slides[currentSlide].SetActive(false);
                currentSlide++;
                if (currentSlide >= slides.Length)
                {
                    currentSlide = 0;
                }

                slides[currentSlide].SetActive(true);
            }
            else if (ReInput.players.GetSystemPlayer().GetNegativeButtonDown("UIHorizontal"))
            {
                slides[currentSlide].SetActive(false);
                currentSlide--;
                if (currentSlide < 0)
                {
                    currentSlide = slides.Length -1;
                }

                slides[currentSlide].SetActive(true);
            }
        }
    }


	public void Play() {
		SceneManager.LoadScene ("GameSettings");
	}

	public void Quit() {
		Application.Quit ();
	}

	public void OptionsMenu()
    {
        foreach (Animator anim in mainMenuAnimator)
        {
            anim.enabled = false;
        }
        mainManuFader.SetTrigger("Fade");
        StartCoroutine(MainMenuFadeOut(0));
    }

    public void HowToPlay()
    {
        foreach (Animator anim in mainMenuAnimator)
        {
            anim.enabled = false;
        }
        mainManuFader.SetTrigger("Fade");
        StartCoroutine(MainMenuFadeOut(2));
    }

    public void MainMenu(int options) {
        if (options == 0)
        {
            foreach (Animator anim in optionsAnimator)
            {
                anim.enabled = false;
            }
            optionsFader.SetTrigger("Fade");
            StartCoroutine(OptionsFadeOut());
        }
        else if (options == 1)
        {
            foreach (Animator anim in creditsAnimator)
            {
                anim.enabled = false;
            }
            creditsFader.SetTrigger("Fade");
            StartCoroutine(CreditsFadeOut());
        }
        else if (options == 2)
        {
            howToPlayHolder.SetActive(false);
            mainMenuHolder.SetActive(true);
            menuManagment.SetSelectedGameObject(mainMenuAnimator[0].gameObject);
            mainManuFader.SetTrigger("FadeIn");
            StartCoroutine(MainMenuFadeIn());
        }
	}

    public void Credits()
    {
        foreach (Animator anim in mainMenuAnimator)
        {
            anim.enabled = false;
        }
        mainManuFader.SetTrigger("Fade");
        StartCoroutine(MainMenuFadeOut(1));
    }

	public void SetScreenResolutionIndex(Dropdown dropDown)
    {
        activeScreenResIndex = dropDown.value;
        PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
        PlayerPrefs.Save();
        SetScreenResolution();
	}

    public void SetScreenResolution()
    {
        switch (activeScreenResIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, fullscreen);
                break;

            case 1:
                Screen.SetResolution(1600, 900, fullscreen);
                break;

            case 2:
                Screen.SetResolution(1280, 720, fullscreen);
                break;

            default:
                Screen.SetResolution(1920, 1080, fullscreen);
                break;
        }
    }


    public void SetFullscreen(Toggle isFullscreen) {
		if (isFullscreen.isOn) {
			fullscreen = true;
            PlayerPrefs.SetInt("fullscreen", 0);
            SetScreenResolution ();
		} 
        else 
        {
            fullscreen = false;
            PlayerPrefs.SetInt("fullscreen", 1);
			SetScreenResolution ();
		}
		PlayerPrefs.Save ();
	}

	public void SetMasterVolume(Slider value) {
		AudioManager.instance.SetVolume (value.value, AudioManager.AudioChannel.Master);
	}

	public void SetMusicVolume(Slider value) {
		AudioManager.instance.SetVolume (value.value, AudioManager.AudioChannel.Music);
	}

	public void SetSfxVolume(Slider value) {
		AudioManager.instance.SetVolume (value.value, AudioManager.AudioChannel.Sfx);
	}

    IEnumerator MainMenuFadeIn()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        mainMenuAnimator[0].SetTrigger("Highlighted");
        foreach (Animator anim in mainMenuAnimator)
        {
            anim.enabled = true;
        }
    }

    IEnumerator MainMenuFadeOut(int options)
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        mainMenuHolder.SetActive(false);
        if (options == 0)
        {
            menuManagment.SetSelectedGameObject(volumeSliders[0].gameObject);
            optionsMenuHolder.SetActive(true);
            optionsFader.SetTrigger("FadeIn");
            StartCoroutine(OptionsFadeIn());
        }
        else if (options == 1)
        {
            menuManagment.SetSelectedGameObject(back);
            creditsHolder.SetActive(true);
            creditsFader.SetTrigger("FadeIn");
            StartCoroutine(CreditsFadeIn());
        }
        else if (options == 2)
        {
            menuManagment.SetSelectedGameObject(backHowToPlay);
            howToPlayHolder.SetActive(true);
            backHowToPlay.GetComponent<Animator>().SetTrigger("Highlighted");
        }
    }

    IEnumerator OptionsFadeIn()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        optionsAnimator[0].SetTrigger("Highlighted");
        foreach (Animator anim in optionsAnimator)
        {
            anim.enabled = true;
        }
    }

    IEnumerator CreditsFadeIn()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        creditsAnimator[0].SetTrigger("Highlighted");
        foreach (Animator anim in creditsAnimator)
        {
            anim.enabled = true;
        }
    }

    IEnumerator OptionsFadeOut()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
        menuManagment.SetSelectedGameObject(mainMenuAnimator[0].gameObject);
        mainManuFader.SetTrigger("FadeIn");
        StartCoroutine(MainMenuFadeIn());
    }

    IEnumerator CreditsFadeOut()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        mainMenuHolder.SetActive(true);
        creditsHolder.SetActive(false);
        menuManagment.SetSelectedGameObject(mainMenuAnimator[0].gameObject);
        mainManuFader.SetTrigger("FadeIn");
        StartCoroutine(MainMenuFadeIn());
    }

    IEnumerator Title()
    {
        yield return new WaitForSeconds(mainMenuFade.length);
        mainMenuHolder.SetActive(true);
        StartCoroutine(MainMenuFadeIn());
    }
}
