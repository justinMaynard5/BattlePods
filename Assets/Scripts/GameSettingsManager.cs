using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour {

    public EventSystem events;
    public SelectionWheel gamemode;
    public SelectionWheel gametime;
    public SelectionWheel slot1;
    public SelectionWheel slot2;
    public SelectionWheel itemspawn;
    public SelectionWheel hazardspawn;
    public SelectionWheel scoremethod;
    public SelectionWheel maxscore;
    public Button points;
    public Button scoreMethod;
    public Text pointsTxt;
    public Text scoreMethodTxt;

    Player player;

	// Use this for initialization
	void Start ()
    {
        player = ReInput.players.GetPlayer("System");
        gamemode.onChanged += onChnagedGameMode;
        scoremethod.onChanged += onChnagedScoreMethod;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player.GetButtonDown("UIHorizontal"))
        {
            if(events.currentSelectedGameObject.GetComponent<SelectionWheel>() != null)
            {
                events.currentSelectedGameObject.GetComponent<SelectionWheel>().Next();
            }
        }

        if (player.GetNegativeButtonDown("UIHorizontal"))
        {
            if (events.currentSelectedGameObject.GetComponent<SelectionWheel>() != null)
            {
                events.currentSelectedGameObject.GetComponent<SelectionWheel>().Previous();
            }
        }

        if (events.currentSelectedGameObject != null)
        {
            if (events.currentSelectedGameObject.GetComponent<SelectionWheel>() != null && events.currentSelectedGameObject.GetComponent<SelectionWheel>().type != SelectionWheel.Type.Num)
            {
                events.currentSelectedGameObject.GetComponent<SelectionWheel>().discription.text = events.currentSelectedGameObject.GetComponent<SelectionWheel>().discriptions[events.currentSelectedGameObject.GetComponent<SelectionWheel>().currentSelection];
            }
        }

        if (events.currentSelectedGameObject == null)
        {
            events.SetSelectedGameObject(gamemode.gameObject);
        }
    }

    void onChnagedScoreMethod()
    {
        switch (scoremethod.currentSelection)
        {
            case (int)GameManager.ScoreMethod.Points:
                pointsTxt.text = "Points";
                break;

            case (int)GameManager.ScoreMethod.Lives:
                pointsTxt.text = "Lives";
                break;
        }
    }
    
    void onChnagedGameMode()
    {
        switch (gamemode.currentSelection)
        {
            case (int)GameManager.GameModes.DeathMatch:
                points.enabled = true;
                scoreMethod.enabled = true;
                pointsTxt.color = Color.red;
                scoreMethodTxt.color = Color.red;
                break;
        }
    }

    public void Play()
    {
        GameManager.instance.gameMode = (GameManager.GameModes)gamemode.currentSelection;
        GameManager.instance.gameTime = (GameManager.Time)gametime.currentSelection;
        GameManager.instance.slot1 = (GameManager.BulletType)slot1.currentSelection;
        GameManager.instance.slot2 = (GameManager.BulletType)slot2.currentSelection;
        GameManager.instance.itemSpawn = (GameManager.SpawnTime)itemspawn.currentSelection;
        GameManager.instance.hazardSpawn = (GameManager.SpawnTime)hazardspawn.currentSelection;
        GameManager.instance.scoreMethod = (GameManager.ScoreMethod)scoremethod.currentSelection;
        GameManager.instance.maxScore = maxscore.currentSelection;

        SceneManager.LoadScene("PlayerSetup");
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
