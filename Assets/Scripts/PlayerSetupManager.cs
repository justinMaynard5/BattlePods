using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerSetupManager : MonoBehaviour {

    public SelectionUI[] UIs;
    int numReady = 0;
    int currentSlide;
    public bool isReady = false;
    bool isLoading = false;

    public Text barTxt;
    public Text progressTxt;
    public Slider loadingBar;
    public GameObject playBtn;
    public GameObject playerSetup;
    public GameObject loading;
    public GameObject slideCanvas;
    public GameObject[] ships;
    public GameObject[] slides;
    public EventSystem eventSystem;

    AsyncOperation ao;
    public SelectionUI keyboardUI1;
    public SelectionUI keyboardUI2;

    // Use this for initialization
    void Start ()
    {
		foreach(SelectionUI ui in UIs)
        {
            ui.OnReady += OnReady;
        }
        ControllerManager.instance.playerSet += OnPlayerSet;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (isReady)
        {
            foreach(PlayerData data in GameManager.instance.playerData)
            {
                if(data.playerID >= 0)
                {
                    if (ao == null)
                    {
                        if (ReInput.players.GetPlayer(data.playerID).GetButtonDown("UISubmit") || ReInput.players.GetPlayer(data.playerID).GetButtonDown("Pause"))
                        {
                            StartCoroutine(Waitforlastplayer());
                        }
                    }
                    else
                    {
                        if (ReInput.players.GetPlayer(data.playerID).GetButtonDown("UISubmit") || ReInput.players.GetPlayer(data.playerID).GetButtonDown("Pause"))
                        {
                            if (ao.progress >= 0.9f)
                            {
                                Play();
                            }
                        }
                    }
                }
            }
        }

        if (ao != null)
        {
            if (isLoading && ao.progress < 0.9f || loadingBar.value != 1)
            {
                loadingBar.value = ao.progress + 0.1f;
                progressTxt.text = Mathf.Floor((ao.progress + 0.1f / 1) * 100).ToString() + "%";
            }
            else if (ao.progress >= 0.9f && progressTxt.gameObject.activeInHierarchy)
            {
                playBtn.SetActive(true);
                eventSystem.SetSelectedGameObject(playBtn);
                slides[currentSlide].SetActive(true);
                slideCanvas.SetActive(true);
                progressTxt.gameObject.SetActive(false);
            }

            if(ao.progress >= 0.9f)
            {
                foreach(Player player in ReInput.players.GetPlayers())
                {
                    if (player.GetButtonDown("UIHorizontal"))
                    {
                        slides[currentSlide].SetActive(false);
                        currentSlide++;
                        if (currentSlide >= slides.Length)
                        {
                            currentSlide = 0;
                        }
                        slides[currentSlide].SetActive(true);
                    }
                    else if (player.GetNegativeButtonDown("UIHorizontal"))
                    {
                        slides[currentSlide].SetActive(false);
                        currentSlide--;
                        if (currentSlide < 0)
                        {
                            currentSlide = slides.Length-1;
                        }
                        slides[currentSlide].SetActive(true);
                    }
                }
            }
        }
        
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(playBtn);
        }
	}

    void OnReady(bool ready)
    {
        isReady = false;
        List<string> teams = new List<string>();
        int numTeams = 0;
        if (ready)
        {
            numReady++;
            Debug.Log(numReady);
            foreach (PlayerData data in GameManager.instance.playerData)
            {
                if (data.team != "" && data.team != null)
                {
                    if (!teams.Contains(data.team))
                    {
                        numTeams++;
                        teams.Add(data.team);
                    }
                }
            }
            if (numTeams >= 2)
            {
                barTxt.text = "Press Start";
                StartCoroutine(Wait(true));
                return;
            }
            else
            {
                barTxt.text = "Need two teams to start";
            }
        }
        else
        {
            numReady--;
            Debug.Log(numReady);
            foreach (PlayerData data in GameManager.instance.playerData)
            {
                if (!teams.Contains(data.team))
                {
                    numTeams++;
                    teams.Add(data.team);
                }
            }

            if (numTeams < 2)
            {
                barTxt.text = "Select your ship, team and color";
                StartCoroutine(Wait(false));
                return;
            }
        }
    }

    public void Play()
    {
        ao.allowSceneActivation = true;
    }

    void OnPlayerSet(int id,Controller con)
    {
        foreach(SelectionUI ui in UIs)
        {
            if (ui.playerID == -1 && !ui.AI)
            {
                ui.playerID = id;
                ui.player = ReInput.players.GetPlayer(id);
                ui.player.isPlaying = true;
                ui.activeCon = true;
                ui.start.SetActive(false);
                ui.active.SetActive(true);
                ui.selected = ui.selections[ui.currentSelection];
                ui.selected.GetComponent<SelectionWheel>().ChangeColor(ui.UIColor);
                foreach (Joystick joystick in ui.player.controllers.Joysticks)
                {
                    var ds4 = joystick.GetExtension<Rewired.ControllerExtensions.DualShock4Extension>();
                    if (ds4 == null) continue; // this is not a DS4, skip it

                    ds4.SetLightColor(ui.UIColor);
                }
                ui.ships.objects[ui.ships.currentSelection].SetActive(true);

                switch (con.type)
                {
                    case ControllerType.Keyboard:
                        if (ControllerManager.instance.twoOnKeyBoard)
                        {
                            ui.types[2].SetActive(true);
                            ui.typesReady[2].SetActive(true);
                            ui.addAI.text = "Press O to add Computer";
                            ui.removeAI.text = "Press U to remove Computer";
                            ui.keyboardMode.gameObject.SetActive(false);
                            keyboardUI2 = ui;
                            keyboardUI1.types[0].SetActive(false);
                            keyboardUI1.typesReady[0].SetActive(false);
                            keyboardUI1.types[1].SetActive(true);
                            keyboardUI1.typesReady[1].SetActive(true);
                            keyboardUI1.addAI.text = "Press E to add Computer";
                            keyboardUI1.removeAI.text = "Press Q to remove Computer";
                            keyboardUI1.keyboardMode.gameObject.SetActive(false);
                        }
                        else
                        {
                            ui.types[0].SetActive(true);
                            ui.typesReady[0].SetActive(true);
                            ui.addAI.text = "Press + to add Computer";
                            ui.removeAI.text = "Press - to remove Computer";
                            ui.keyboardMode.gameObject.SetActive(true);
                            keyboardUI1 = ui;
                        }
                        break;

                    case ControllerType.Joystick:
                        ui.types[3].SetActive(true);
                        ui.typesReady[3].SetActive(true);
                        ui.addAI.text = "Press Right Bumper to add Computer";
                        ui.removeAI.text = "Press Left Bumper to remove Computer";
                        ui.keyboardMode.gameObject.SetActive(false);
                        break;
                }
                break;
            }
        }
    }

    IEnumerator Wait(bool ready)
    {
        yield return new WaitForEndOfFrame();
        if (ready)
        {
            isReady = true;
        }
        else
        {
            isReady = false;
        }
    }

    IEnumerator Waitforlastplayer()
    {
        yield return new WaitForEndOfFrame();

        ao = SceneManager.LoadSceneAsync("game");
        ao.allowSceneActivation = false;

        isLoading = true;

        playerSetup.SetActive(false);
        foreach (GameObject ship in ships)
        {
            ship.SetActive(false);
        }
        loading.SetActive(true);
    }
}
