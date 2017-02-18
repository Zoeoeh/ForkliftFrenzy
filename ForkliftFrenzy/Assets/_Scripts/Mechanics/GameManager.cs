﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Difficulty { EASY = 1, MEDIUM, HARD };
public enum ForkLift { SPEEDY, ENGIE, TANK, TRICKSY };

public class GameManager : MonoBehaviour {

    private bool inPlay = false;
    private float timeLeft = 60;

    public Mission currentMission;

    private Text timer;
    private Text score;

    private int currentScore = 0;

    // var for persistence
    private PlayerData playerData;
    private GameActor playerScript;

    // to store current boxes in play
    public int totalBoxes = 0;

    // multiplier for difficulty
    public const int boxMultiplier = 3;

    public GameObject endGameUI;
    private string endGameMessage;
    private bool gameOver = false;

	// Use this for initialization
	void Start () {
        timer = GameObject.Find("Timer").GetComponent<Text>();
        score = GameObject.Find("Score").GetComponent<Text>();
        playerData = GetComponent<PlayerData>();
        playerScript = GameObject.Find("Player").GetComponent<GameActor>();

        // store original endgame message for parsing
        endGameMessage = endGameUI.GetComponentInChildren<Text>().text;

        // initial forklift unlock for player
        playerData.UnlockForklift(ForkLift.ENGIE);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!inPlay && Input.GetKeyDown(KeyCode.P))
        {
            inPlay = true;
            currentMission.ClearBoxes();
            currentMission.SpawnBoxes(Difficulty.HARD);
            totalBoxes = (int)Difficulty.HARD * boxMultiplier;

            // TODO change to var depending on difficulty
            timeLeft = 60;
            Debug.Log("Start Mission");
        }
	
        if (inPlay)
        {
            UpdateUI();

            if (timeLeft <= 0 || totalBoxes == 0)
                EndGame();
        }

        if (gameOver && Input.anyKeyDown)
        {
            ResetGame();
        }


        // gamehacks to add score and end game quickly
        if (Input.GetKeyDown(KeyCode.I))
            currentScore += 200;

        if (Input.GetKeyDown(KeyCode.N))
            EndGame();
	}

    private void UpdateUI()
    {
        timeLeft -= Time.deltaTime;

        // format time float 
        int val = (int)(timeLeft * 100.0f);
        int minutes = val / (60 * 100);
        int seconds = (val % (60 * 100)) / 100;
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // update score
        score.text = currentScore.ToString();

    }

    private void ResetGame()
    {
        gameOver = false;

        // hide UI
        endGameUI.SetActive(false);
        currentScore = 0;
        timeLeft = 0;

        UpdateUI();

    }

    private void EndGame()
    {
        inPlay = false;
        gameOver = true;

        // update player data
        bool highbeat = playerData.HasBeatenHighscore(currentScore);
        playerData.dirty = true;

        // set original end game message
        string newText = endGameMessage;

        // update text and show ui
        if (highbeat)
        {
            newText = newText.Replace("HS", "You beat your highscore!");
        }
        else
        {
            newText = newText.Replace("HS", "\n");
        }

        newText = newText.Replace("%", "" + currentScore);

        // replace money with earnings
        int earnings = currentScore / 10;

        newText = newText.Replace("$", "" + earnings);

        int timeBonus = 0;

        // time bonus for extra time
        if (timeLeft >= 10)
        {
            // get rounded 
            timeBonus = (int)timeLeft / 10;

            timeBonus *= 10;
            
            newText = newText.Replace("tb", "+ Time Bonus " + timeBonus);
        }
        else
        {
            newText = newText.Replace("tb", "");
        }

        playerData.AddMoney(earnings + timeBonus);

        endGameUI.GetComponentInChildren<Text>().text = newText;
        endGameUI.SetActive(true);
    }

    public void SendBox(GameObject box)
    {
        // TODO: Check for type of box here
        currentScore += 200;
        totalBoxes--;
    }


    // collider for respawn zone
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "backWheels" || other.tag == "frontWheels")
        {
            // TODO will need to change this to UI, just for testing 1234 keys for forklift select
            
            if (Input.GetKeyDown(KeyCode.Alpha1) && playerData.IsForkUnlocked(ForkLift.ENGIE))
                playerScript.ChangeFork(ForkLift.ENGIE);
            else if (Input.GetKeyDown(KeyCode.Alpha2) && playerData.IsForkUnlocked(ForkLift.SPEEDY))
                playerScript.ChangeFork(ForkLift.SPEEDY);
            else if (Input.GetKeyDown(KeyCode.Alpha3) && playerData.IsForkUnlocked(ForkLift.TANK))
                playerScript.ChangeFork(ForkLift.TANK);
            else if (Input.GetKeyDown(KeyCode.Alpha4) && playerData.IsForkUnlocked(ForkLift.TRICKSY))
                playerScript.ChangeFork(ForkLift.TRICKSY);

        }
    }
}
