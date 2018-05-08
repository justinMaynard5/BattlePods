using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using PicaVoxel;
using UnityEngine.SceneManagement;

public class SelectionUI : MonoBehaviour {

    public PlayerSetupManager manager;
    public SelectionWheel color;
    public SelectionWheel team;
    public SelectionWheel ships;
    public GameObject[] selections;
    public GameObject[] types;
    public GameObject[] typesReady;
    public GameObject active;
    public GameObject start;
    public GameObject ready;
    public Image border;
    public Text colorTxt;
    public Text teamTxt;
    public Text addAI;
    public Text removeAI;
    public Text keyboardMode;
    public Image readyObj;
    public int position;
    public int playerID = -1;
    public SelectionUI[] otherUI;
    public delegate void Ready(bool ready);
    public event Ready OnReady;
    public bool AI = false;
    public bool activeCon = false;
    SelectionUI parent = null;

    public Player player;
    public int currentSelection = 0;
    public GameObject selected;
    public Color UIColor;
    Color normal;

    // Use this for initialization
    void Start ()
    {
        //player = ReInput.players.GetPlayer(playerID);
        selected = selections[currentSelection];
        color.selections = Enum.GetNames(typeof(PlayerData.Colors));

        switch (GameManager.instance.gameMode)
        {
            case GameManager.GameModes.DeathMatch:
                team.selections = new string[4];
                team.selections[0] = "Team 1";
                team.selections[1] = "Team 2";
                team.selections[2] = "Team 3";
                team.selections[3] = "Team 4";
                break;
        }

        switch (color.currentSelection)
        {
            case (int)PlayerData.Colors.Red:
                UIColor = new Color(1, 0, 0, 0.98f);
                break;

            case (int)PlayerData.Colors.Green:
                UIColor = new Color(0, 1, 0, 0.98f);
                break;

            case (int)PlayerData.Colors.Blue:
                UIColor = new Color(0, 0, 1, 0.98f);
                break;

            case (int)PlayerData.Colors.Yellow:
                UIColor = new Color(1, 1, 0, 0.98f);
                break;
        }

        normal = new Color(0.42f, 0.42f, 0.42f, 1);

        border.color = UIColor;
        colorTxt.color = UIColor;
        teamTxt.color = UIColor;
        readyObj.color = normal;
        readyObj.gameObject.GetComponentInChildren<Text>().color = normal;

        foreach(GameObject selection in selections)
        {
            if (selection.GetComponent<SelectionWheel>() != null)
            {
                selection.GetComponent<SelectionWheel>().ChangeColor(normal);
            }
        }

        foreach(GameObject obj in ships.objects)
        {
            for (int i = 0; i < obj.GetComponent<Volume>().Frames[0].Voxels.Length; i++)
            {
                if (obj.GetComponent<Volume>().Frames[0].Voxels[i].Value == 1)
                {
                    obj.GetComponent<Volume>().Frames[0].Voxels[i].Color = new Color(UIColor.r,UIColor.g,UIColor.b,0.5f);
                }
            }
            obj.GetComponent<Volume>().UpdateAllChunks();
        }

        color.onChanged += OnColorChange;

        //ControllerManager.instance.playerSet += OnPlayerSet;
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (activeCon)
        {
            if (player.GetButtonDown("UIHorizontal") && active.activeInHierarchy)
            {
                if (selected.GetComponent<SelectionWheel>() != null)
                {
                    Next(selected.GetComponent<SelectionWheel>());
                }
            }

            if (player.GetNegativeButtonDown("UIHorizontal") && active.activeInHierarchy)
            {
                if (selected.GetComponent<SelectionWheel>() != null)
                {
                    Previous(selected.GetComponent<SelectionWheel>());
                }
            }

            if (player.GetNegativeButtonDown("UIVertical") && active.activeInHierarchy)
            {
                selected.GetComponent<SelectionWheel>().ChangeColor(normal);
                currentSelection++;
                if (currentSelection >= selections.Length)
                {
                    currentSelection = 0;
                }
                selected = selections[currentSelection];
                selected.GetComponent<SelectionWheel>().ChangeColor(UIColor);
                AudioManager.instance.PlaySfx("btn");
            }

            if (player.GetButtonDown("UIVertical") && active.activeInHierarchy)
            {
                selected.GetComponent<SelectionWheel>().ChangeColor(normal);
                currentSelection--;
                if (currentSelection < 0)
                {
                    currentSelection = selections.Length - 1;
                }
                selected = selections[currentSelection];
                selected.GetComponent<SelectionWheel>().ChangeColor(UIColor);
                AudioManager.instance.PlaySfx("btn");
            }

            if (player.GetButtonDown("UISubmit"))
            {
                if (selections[currentSelection] == readyObj.gameObject && !ready.activeInHierarchy)
                {
                    PlayerData data = GameManager.instance.playerData[playerID];
                    data.playerID = playerID;
                    data.color = (PlayerData.Colors)color.currentSelection;
                    data.team = team.GetComponentInChildren<Text>().text;
                    OnReady(true);
                    data.ship = ships.currentSelection;
                    foreach (SelectionUI ui in otherUI)
                    {
                        ui.CheckForDups(ui.color, true);
                    }
                    active.SetActive(false);
                    if (AI)
                    {
                        data.type = PlayerData.Type.AI;
                        activeCon = false;
                        parent.activeCon = true;
                        addAI.text = "";
                        removeAI.text = "";
                        keyboardMode.gameObject.SetActive(false);
                    }
                    else
                    {
                        data.type = PlayerData.Type.Player;
                    }
                    ready.SetActive(true);
                }
            }

            if (player.GetButtonDown("UICancel"))
            {
                if (active.activeInHierarchy)
                {
                    if (AI)
                    {
                        activeCon = false;
                        parent.activeCon = true;
                        parent = null;
                        AI = false;
                        player = ReInput.players.GetPlayer(playerID);
                    }
                    else
                    {
                        if (player.controllers.joystickCount != 0)
                        {
                            ReInput.players.GetSystemPlayer().controllers.AddController(player.controllers.Joysticks[0], true);
                        }

                        if (player.controllers.maps.GetMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UI", "Default") != null)
                        {
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UI", "Default");
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "Default", "Default");
                        }
                        else if (player.controllers.maps.GetMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UILeft", "Default") != null)
                        {
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UILeft", "Default");
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "DefaultLeft", "Default");
                            ControllerManager.instance.keyboard1 = null;
                            ControllerManager.instance.keyboard2.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UIRight", "Default");
                            ControllerManager.instance.keyboard2.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "DefaultRight", "Default");
                            ControllerManager.instance.keyboard2.controllers.maps.LoadMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UI", "Default");
                            ControllerManager.instance.keyboard2.controllers.maps.LoadMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "Default", "Default");
                            ControllerManager.instance.keyboard2.controllers.maps.SetAllMapsEnabled(true);
                            ControllerManager.instance.keyboard2 = null;
                            ControllerManager.instance.twoOnKeyBoard = false;

                            foreach(GameObject type in manager.keyboardUI1.types)
                            {
                                type.SetActive(false);
                            }

                            foreach (GameObject type in manager.keyboardUI1.typesReady)
                            {
                                type.SetActive(false);
                            }

                            manager.keyboardUI1 = manager.keyboardUI2;
                            manager.keyboardUI2 = null;

                            manager.keyboardUI1.types[2].SetActive(false);
                            manager.keyboardUI1.typesReady[2].SetActive(false);
                            manager.keyboardUI1.types[0].SetActive(true);
                            manager.keyboardUI1.typesReady[0].SetActive(true);

                            manager.keyboardUI1.addAI.text = "Press + to add Computer";
                            manager.keyboardUI1.removeAI.text = "Press - to remove Computer";
                            manager.keyboardUI1.keyboardMode.gameObject.SetActive(true);
                        }
                        else if (player.controllers.maps.GetMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UIRight", "Default") != null)
                        {
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UIRight", "Default");
                            player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "DefaultRight", "Default");
                            ControllerManager.instance.keyboard2 = null;
                            ControllerManager.instance.keyboard1.controllers.maps.RemoveMap(ControllerType.Keyboard, ControllerManager.instance.keyboard1.controllers.Keyboard.id, "UILeft", "Default");
                            ControllerManager.instance.keyboard1.controllers.maps.RemoveMap(ControllerType.Keyboard, ControllerManager.instance.keyboard1.controllers.Keyboard.id, "DefaultLeft", "Default");
                            ControllerManager.instance.keyboard1.controllers.maps.LoadMap(ControllerType.Keyboard, ControllerManager.instance.keyboard1.controllers.Keyboard.id, "UI", "Default");
                            ControllerManager.instance.keyboard1.controllers.maps.LoadMap(ControllerType.Keyboard, ControllerManager.instance.keyboard1.controllers.Keyboard.id, "Default", "Default");
                            ControllerManager.instance.keyboard1.controllers.maps.SetAllMapsEnabled(true);
                            ControllerManager.instance.keyboard1 = null;
                            ControllerManager.instance.twoOnKeyBoard = false;

                            foreach (GameObject type in manager.keyboardUI2.types)
                            {
                                type.SetActive(false);
                            }

                            foreach (GameObject type in manager.keyboardUI2.typesReady)
                            {
                                type.SetActive(false);
                            }

                            manager.keyboardUI2 = null;

                            manager.keyboardUI1.types[1].SetActive(false);
                            manager.keyboardUI1.typesReady[1].SetActive(false);
                            manager.keyboardUI1.types[0].SetActive(true);
                            manager.keyboardUI1.typesReady[0].SetActive(true);

                            manager.keyboardUI1.addAI.text = "Press + to add Computer";
                            manager.keyboardUI1.removeAI.text = "Press - to remove Computer";
                            manager.keyboardUI1.keyboardMode.gameObject.SetActive(true);
                        }
                    }

                    foreach(GameObject type in types)
                    {
                        type.SetActive(false);
                    }

                    foreach (GameObject type in typesReady)
                    {
                        type.SetActive(false);
                    }

                    ControllerManager.instance.players--;
                    player.isPlaying = false;
                    player = null;
                    playerID = -1;
                    activeCon = false;
                    ships.objects[ships.currentSelection].SetActive(false);
                    active.SetActive(false);
                    start.SetActive(true);
                }
                else if (ready.activeInHierarchy)
                {
                    GameManager.instance.playerData[playerID].Reset();
                    ready.SetActive(false);
                    OnReady(false);
                    active.SetActive(true);
                }
            }

            if (player.GetButtonDown("Add") && !AI && ready.activeInHierarchy)
            {
                foreach (SelectionUI ui in otherUI)
                {
                    if (ui.start.activeInHierarchy)
                    {
                        ui.AI = true;
                        ui.types[4].SetActive(true);
                        ui.typesReady[4].SetActive(true);
                        ui.parent = this;
                        ui.start.SetActive(false);
                        ui.active.SetActive(true);
                        ui.selected = ui.selections[ui.currentSelection];
                        ui.selected.GetComponent<SelectionWheel>().ChangeColor(UIColor);
                        ui.player = player;
                        ui.ships.objects[ui.ships.currentSelection].SetActive(true);
                        ui.activeCon = true;
                        activeCon = false;
                        ControllerManager.instance.players++;
                        ui.playerID = ui.position;
                        ReInput.players.GetPlayer(ui.playerID).isPlaying = true;
                        return;
                    }
                }
            }

            if (player.GetButtonDown("Remove") && !AI)
            {
                foreach (SelectionUI ui in otherUI)
                {
                    if (ui.ready.activeInHierarchy && ui.AI)
                    {
                        GameManager.instance.playerData[ui.playerID].Reset();
                        ui.ready.SetActive(false);
                        ui.OnReady(false);
                        ui.active.SetActive(true);
                        ui.activeCon = true;
                        activeCon = false;
                        ui.parent = this;
                        ui.player = player;
                        break;
                    }
                }
            }
        }
    }

    void OnPlayerSet(int id)
    {
        if (id == playerID)
        {
            activeCon = true;
            start.SetActive(false);
            active.SetActive(true);
            selected = selections[currentSelection];
            selected.GetComponent<SelectionWheel>().ChangeColor(UIColor);
            foreach (Joystick joystick in player.controllers.Joysticks)
            {
                var ds4 = joystick.GetExtension<Rewired.ControllerExtensions.DualShock4Extension>();
                if (ds4 == null) continue; // this is not a DS4, skip it

                ds4.SetLightColor(UIColor);
            }
            ships.objects[ships.currentSelection].SetActive(true);
        }
    }

    void OnColorChange()
    {
        switch (color.currentSelection)
        {
            case (int)PlayerData.Colors.Red:
                UIColor = new Color(1, 0, 0, 0.98f);
                break;

            case (int)PlayerData.Colors.Green:
                UIColor = new Color(0, 1, 0, 0.98f);
                break;

            case (int)PlayerData.Colors.Blue:
                UIColor = new Color(0, 0, 1, 0.98f);
                break;

            case (int)PlayerData.Colors.Yellow:
                UIColor = new Color(1, 1, 0, 0.98f);
                break;
        }

        if (player != null)
        {
            foreach (Joystick joystick in player.controllers.Joysticks)
            {
                var ds4 = joystick.GetExtension<Rewired.ControllerExtensions.DualShock4Extension>();
                if (ds4 == null) continue; // this is not a DS4, skip it

                ds4.SetLightColor(UIColor);
            }
        }


        border.color = UIColor;
        colorTxt.color = UIColor;
        teamTxt.color = UIColor;
        readyObj.color = normal;
        readyObj.gameObject.GetComponentInChildren<Text>().color = normal;

        foreach (GameObject selection in selections)
        {
            if (selection.GetComponent<SelectionWheel>() != null)
            {
                selection.GetComponent<SelectionWheel>().ChangeColor(normal);
            }
        }

        foreach (GameObject obj in ships.objects)
        {
            for (int i = 0; i < obj.GetComponent<Volume>().Frames[0].Voxels.Length; i++)
            {
                if (obj.GetComponent<Volume>().Frames[0].Voxels[i].Value == 1)
                {
                    obj.GetComponent<Volume>().Frames[0].Voxels[i].Color = new Color(UIColor.r, UIColor.g, UIColor.b, 0.5f);
                }
            }
            obj.GetComponent<Volume>().UpdateAllChunks();
        }

        selected.GetComponent<SelectionWheel>().ChangeColor(UIColor);
    } 

    void CheckForDups(SelectionWheel wheel, bool next)
    {
        foreach(PlayerData playerData in GameManager.instance.playerData)
        {
            if(wheel == color)
            {
                if(playerData.color == (PlayerData.Colors)wheel.currentSelection && playerData.playerID != -1 && playerData.playerID != playerID)
                {
                    if (next)
                    {
                        Next(wheel);
                    }
                    else
                    {
                        Previous(wheel);
                    }
                }
            }

            //if (wheel == team)
            //{
            //    if (playerData.team == team.GetComponentInChildren<Text>().text)
            //    {
            //        if (next)
            //        {
            //            wheel.Next();
            //        }
            //        else
            //        {
            //            wheel.Previous();
            //        }
            //    }
            //}

            //if (wheel == ships)
            //{
            //    if ((int)playerData.ship == wheel.currentSelection)
            //    {
            //        if (next)
            //        {
            //            wheel.Next();
            //        }
            //        else
            //        {
            //            wheel.Previous();
            //        }
            //    }
            //}
        }
    }

    void Next(SelectionWheel wheel)
    {
        wheel.Next();
        CheckForDups(wheel, true);
    }

    void Previous(SelectionWheel wheel)
    {
        wheel.Previous();
        CheckForDups(wheel, false);
    }
}
