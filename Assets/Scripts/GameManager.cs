using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    
    public enum GameModes {DeathMatch}
    public enum Time {p5, p10, p15, pUnlimited}
    public enum BulletType {Missile, Bomb, Empty}
    public enum SpawnTime {p10, p15, p20, p25, pNone}
    public enum ScoreMethod { Points,Lives}
    public GameModes gameMode;
    public Time gameTime;
    public BulletType slot1;
    public BulletType slot2;
    public SpawnTime itemSpawn;
    public SpawnTime hazardSpawn;
    public ScoreMethod scoreMethod;

    public int maxScore;

    public PlayerData[] playerData;

    // Use this for initialization
    void Start ()
    {
	    if(instance  == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        

        SceneManager.activeSceneChanged += SceneChange;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SceneChange(Scene previous, Scene current)
    {
        if (current == SceneManager.GetSceneByName("GameSettings"))
        {
            SelectionWheel gameModeUI = GameObject.Find("gamemodes").GetComponent<SelectionWheel>();

            gameModeUI.selections = Enum.GetNames(typeof(GameModes));
            gameModeUI.txt.text = gameModeUI.selections[0];

            SelectionWheel timeUI = GameObject.Find("time").GetComponent<SelectionWheel>();

            timeUI.selections = Enum.GetNames(typeof(Time));
            for (int i = 0; i < timeUI.selections.Length; i++)
            {
                timeUI.selections[i] = timeUI.selections[i].Substring(1);
            }
            timeUI.txt.text = timeUI.selections[0];

            SelectionWheel slot1UI = GameObject.Find("slot1").GetComponent<SelectionWheel>();

            slot1UI.selections = Enum.GetNames(typeof(BulletType));
            slot1UI.txt.text = slot1UI.selections[0];

            SelectionWheel slot2UI = GameObject.Find("slot2").GetComponent<SelectionWheel>();

            slot2UI.selections = Enum.GetNames(typeof(BulletType));
            slot2UI.txt.text = slot2UI.selections[0];

            SelectionWheel itemSpawnUI = GameObject.Find("itemSpawn").GetComponent<SelectionWheel>();

            itemSpawnUI.selections = Enum.GetNames(typeof(SpawnTime));
            for (int i = 0; i < itemSpawnUI.selections.Length; i++)
            {
                itemSpawnUI.selections[i] = itemSpawnUI.selections[i].Substring(1);
            }
            itemSpawnUI.txt.text = itemSpawnUI.selections[0];

            SelectionWheel hazardSpawnUI = GameObject.Find("hazardSpawn").GetComponent<SelectionWheel>();

            hazardSpawnUI.selections = Enum.GetNames(typeof(SpawnTime));
            for (int i = 0; i < itemSpawnUI.selections.Length; i++)
            {
                hazardSpawnUI.selections[i] = hazardSpawnUI.selections[i].Substring(1);
            }
            hazardSpawnUI.txt.text = hazardSpawnUI.selections[0];

            SelectionWheel scoreMethodUI = GameObject.Find("scoreMethod").GetComponent<SelectionWheel>();

            scoreMethodUI.selections = Enum.GetNames(typeof(ScoreMethod));
            scoreMethodUI.txt.text = scoreMethodUI.selections[0];
        }
    }
}
