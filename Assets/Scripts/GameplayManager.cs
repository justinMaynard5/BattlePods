using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Rewired;

public class GameplayManager : MonoBehaviour {

    public float time;
    bool gamePlaying = true;
    string winner = null;

    int[] teamPoints;

    public Text txtTime;
    public Text winText;
    public PlayerController[] players;
    public AIPlayer[] AiPlayers;
    public GameObject rematch;
    public GameObject pauseMenu;
    public EventSystem eventSystem;
    public GameObject firstOnRematch;
    public GameObject firstOnPause;
    public CanvasGroup inGame;
    public CanvasGroup menu;

	// Use this for initialization
    void Awake()
    {
        for(int i = 0; i < players.Length; i++)
        {
            if(GameManager.instance.playerData[i].type == PlayerData.Type.AI)
            {
                Destroy(players[i]);
            }
            else
            {
                Destroy(players[i].GetComponent<AIPlayer>());
            }
        }
    }
    
	void Start ()
    {
        teamPoints = new int[4];
        switch (GameManager.instance.gameTime)
        {
            case GameManager.Time.pUnlimited:
                txtTime.gameObject.SetActive(false);
                break;

            case GameManager.Time.p5:
                time = 300;
                txtTime.text = Mathf.Floor(time / 60).ToString() + ":" + (time % 60).ToString("00");
                break;

            case GameManager.Time.p10:
                time = 600;
                txtTime.text = Mathf.Floor(time / 60).ToString() + ":" + (time % 60).ToString("00");
                break;

            case GameManager.Time.p15:
                time = 900;
                txtTime.text = Mathf.Floor(time / 60).ToString() + ":" + (time % 60).ToString("00");
                break;
        }

        StartCoroutine(WaitForSetup());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gamePlaying)
        {
            time -= Time.deltaTime;
            txtTime.text = Mathf.Floor(time / 60).ToString() + ":" + (time % 60).ToString("00");

            if (time <= 0 && GameManager.instance.gameTime != GameManager.Time.pUnlimited)
            {
                EndGame();
            }
        }
        
        if (eventSystem.currentSelectedGameObject == null)
        {
            if (Time.timeScale == 0)
            {
                eventSystem.SetSelectedGameObject(firstOnPause);
            }
            else
            {
                eventSystem.SetSelectedGameObject(firstOnRematch);
            }
        }
    }

    void EndGame()
    {
        gamePlaying = false;
        txtTime.text = "00:00";
        if (winner != null)
        {
            winText.text = winner + " Wins!";
        }
        else
        {
            int tempWinner = -1;
            bool tie = false;
            for (int i = 0; i < teamPoints.Length; i++)
            {
                for (int j = 0; j < teamPoints.Length; j++)
                {
                    if (teamPoints[i] > teamPoints[j])
                    {
                        if (tempWinner != -1)
                        {
                            if (teamPoints[i] > teamPoints[tempWinner])
                            {
                                tempWinner = i;
                                tie = false;
                            }
                            else if (teamPoints[i] == teamPoints[tempWinner] && i != tempWinner)
                            {
                                tie = true;
                            }
                        }
                        else
                        {
                            tempWinner = i;
                            tie = false;
                        }
                    }
                    else if(teamPoints[i] == teamPoints[j])
                    {
                        if (i != j)
                        {
                            if (i == tempWinner || tempWinner == -1)
                            {
                                tie = true;
                            }
                        }
                    }
                }
            }

            if (tie)
            {
                winText.text = "Draw";
            }
            else
            {
                switch (tempWinner)
                {
                    case 0:
                        winText.text = "Team 1 Wins!";
                        break;

                    case 1:
                        winText.text = "Team 2 Wins!";
                        break;

                    case 2:
                        winText.text = "Team 3 Wins!";
                        break;

                    case 3:
                        winText.text = "Team 4 Wins!";
                        break;
                }
            }
        }
        foreach (PlayerController player in players)
        {
            player.gamePlaying = false;
        }
        foreach (AIPlayer player in AiPlayers)
        {
            player.gamePlaying = false;
        }
        rematch.SetActive(true);
        inGame.interactable = false;
        menu.interactable = true;
        eventSystem.SetSelectedGameObject(firstOnRematch);
    }

    void onScoreIncrease(int point, string team)
    {
        if (GameManager.instance.scoreMethod == GameManager.ScoreMethod.Points)
        {
            switch (team)
            {
                case "Team 1":
                    teamPoints[0] += point;
                    if (teamPoints[0] >= GameManager.instance.maxScore && GameManager.instance.maxScore != 0)
                    {

                        winner = team;
                        EndGame();
                    }
                    break;

                case "Team 2":
                    teamPoints[1] += point;
                    if (teamPoints[1] >= GameManager.instance.maxScore && GameManager.instance.maxScore != 0)
                    {
                        winner = team;
                        EndGame();
                    }
                    break;

                case "Team 3":
                    teamPoints[2] += point;
                    if (teamPoints[2] >= GameManager.instance.maxScore && GameManager.instance.maxScore != 0)
                    {
                        winner = team;
                        EndGame();
                    }
                    break;

                case "Team 4":
                    teamPoints[3] += point;
                    if (teamPoints[3] >= GameManager.instance.maxScore && GameManager.instance.maxScore != 0)
                    {
                        winner = team;
                        EndGame();
                    }
                    break;
            }
        }
    }
    void onNoLives(string team)
    {
        if (GameManager.instance.scoreMethod == GameManager.ScoreMethod.Lives)
        {
            switch (team)
            {
                case "Team 1":
                    teamPoints[0]--;
                    break;

                case "Team 2":
                    teamPoints[1]--;
                    break;

                case "Team 3":
                    teamPoints[2]--;
                    break;

                case "Team 4":
                    teamPoints[3]--;
                    break;
            }

            int Winner = 0;

            for (int i = 0; i < 4; i++)
            {
                if (teamPoints[i] > 0)
                {
                    if (Winner == 0)
                    {
                        Winner = i + 1;
                        winner = "Team " + Winner;
                    }
                    else
                    {
                        winner = null;
                        return;
                    }
                }
            }


            EndGame();
        }
    }

    public void Rematch()
    {
        SceneManager.LoadScene("game");
    }

    public void MatchSettings()
    {
        foreach(PlayerData player in GameManager.instance.playerData)
        {
            player.Reset();
        }

        ControllerManager.instance.ResetControllers();
        
        SceneManager.LoadScene("GameSettings");
    }

    public void Quit()
    {
        Time.timeScale = 1;

        foreach (PlayerData player in GameManager.instance.playerData)
        {
            player.Reset();
        }

        ControllerManager.instance.ResetControllers();

        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            foreach (PlayerController player in players)
            {
                player.gamePlaying = false;
            }
            inGame.interactable = false;
            menu.interactable = true;
            pauseMenu.SetActive(true);
        }
        else
        {
            StartCoroutine(UnPause());
        }
    }

    IEnumerator UnPause()
    {
        yield return new WaitForEndOfFrame();

        inGame.interactable = true;
        menu.interactable = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        foreach (PlayerController player in players)
        {
            player.gamePlaying = true;
        }
    }

    IEnumerator WaitForSetup()
    {
        yield return new WaitForEndOfFrame();

        foreach (PlayerController player in players)
        {
            player.onScoreIncrease += onScoreIncrease;
            player.onNoLives += onNoLives;

            if (GameManager.instance.scoreMethod == GameManager.ScoreMethod.Lives)
            {
                switch (player.team.text)
                {
                    case "Team 1":
                        teamPoints[0] += GameManager.instance.maxScore;
                        break;

                    case "Team 2":
                        teamPoints[1] += GameManager.instance.maxScore; ;
                        break;

                    case "Team 3":
                        teamPoints[2] += GameManager.instance.maxScore; ;
                        break;

                    case "Team 4":
                        teamPoints[3] += GameManager.instance.maxScore; ;
                        break;
                }
            }
        }

        foreach (AIPlayer player in AiPlayers)
        {
            player.onScoreIncrease += onScoreIncrease;
            player.onNoLives += onNoLives;

            //if (GameManager.instance.scoreMethod == GameManager.ScoreMethod.Lives)
            //{
            //    switch (player.team.text)
            //    {
            //        case "Team 1":
            //            teamPoints[0] += GameManager.instance.maxScore;
            //            break;

            //        case "Team 2":
            //            teamPoints[1] += GameManager.instance.maxScore; ;
            //            break;

            //        case "Team 3":
            //            teamPoints[2] += GameManager.instance.maxScore; ;
            //            break;

            //        case "Team 4":
            //            teamPoints[3] += GameManager.instance.maxScore; ;
            //            break;
            //    }
            //}
        }
    }
}
