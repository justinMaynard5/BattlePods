using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour {

    public static ControllerManager instance;
    public delegate void PlayerSet(int id, Controller con);
    public event PlayerSet playerSet;
    public int players;

    public bool twoOnKeyBoard;

    public Player keyboard1;
    public Player keyboard2;

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (var j in ReInput.controllers.Joysticks)
            ReInput.players.GetSystemPlayer().controllers.AddController(ControllerType.Joystick, j.id, true);

        foreach (Player player in ReInput.players.GetPlayers())
        {
            player.isPlaying = false;
        }

        ReInput.players.GetPlayer("System").controllers.maps.SetAllMapsEnabled(true);
    }
    // Update is called once per frame
    void Update ()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PlayerSetup"))
        {
            if (ReInput.players.GetPlayer("System").GetAnyButtonDown() && !ReInput.players.GetSystemPlayer().GetButtonDown("UICancel"))
            {
                foreach (Controller con in ReInput.players.GetSystemPlayer().controllers.Controllers)
                {
                    if (con.GetAnyButtonDown())
                    {
                        if (con.type == ControllerType.Keyboard)
                        {
                            foreach (Player player in ReInput.players.Players)
                            {
                                if (player.controllers.maps.GetMap(ControllerType.Keyboard, con.id, "UI", "Default") != null && !twoOnKeyBoard && ReInput.players.GetSystemPlayer().GetButtonDown("SecondPlayer") && player != ReInput.players.GetSystemPlayer())
                                {
                                    twoOnKeyBoard = true;
                                    player.controllers.maps.RemoveMap(ControllerType.Keyboard, con.id, "UI", "Default");
                                    player.controllers.maps.RemoveMap(con.type, con.id, "Default", "Default");

                                    player.controllers.maps.LoadMap(con.type, con.id, "UILeft", "Default");
                                    player.controllers.maps.LoadMap(con.type, con.id, "DefaultLeft", "Default");
                                    player.controllers.maps.SetAllMapsEnabled(true);
                                    keyboard1 = player;
                                    break;
                                }
                                else if (twoOnKeyBoard || player.controllers.maps.GetMap(ControllerType.Keyboard, con.id, "UI", "Default") != null &&  player != ReInput.players.GetSystemPlayer())
                                {
                                    return;
                                }
                            }
                        }
                        foreach (Player player in ReInput.players.Players)
                        {
                            if (player.controllers.joystickCount == 0 && player != ReInput.players.GetSystemPlayer() && player.controllers.maps.GetMap(ControllerType.Keyboard, con.id, "UI", "Default") == null && players < 4 && !player.isPlaying)
                            {
                                playerSet(player.id, con);
                                player.isPlaying = true;
                                players++;
                                player.controllers.AddController(con, true);
                                if (con.type == ControllerType.Keyboard && !twoOnKeyBoard)
                                {
                                    player.controllers.maps.LoadMap(con.type, con.id, "UI", "Default");
                                    player.controllers.maps.LoadMap(con.type, con.id, "Default", "Default");
                                }
                                else if (con.type == ControllerType.Keyboard && twoOnKeyBoard)
                                {
                                    player.controllers.maps.LoadMap(con.type, con.id, "UIRight", "Default");
                                    player.controllers.maps.LoadMap(con.type, con.id, "DefaultRight", "Default");
                                    keyboard2 = player;
                                }
                                player.controllers.maps.SetAllMapsEnabled(true);
                                break;
                            }
                        }
                    }
                }
            }
            else if (ReInput.players.GetSystemPlayer().GetButtonDown("UICancel") && players == 0)
            {
                SceneManager.LoadScene("GameSettings");
                playerSet = null;
            }
        }
    }

    public void ResetControllers()
    {
        for (int i = 0; i < ReInput.controllers.joystickCount; i++)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(ReInput.controllers.Joysticks[i], true);
        }

        foreach (Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.maps.GetMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UI", "Default") != null && player != ReInput.players.GetSystemPlayer())
            {
                player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "UI", "Default");
                player.controllers.maps.RemoveMap(ControllerType.Keyboard, player.controllers.Keyboard.id, "Default", "Default");
            }
        }
        playerSet = null;
        players = 0;
    } 
}
