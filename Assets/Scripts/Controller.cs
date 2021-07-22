using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public enum GameState
    {
        WAITING, CLICKING, CLICKED
    }
    
    // Game objects to be enabled/disabled
    public GameObject waitingObject;
    public GameObject leftClickObject;
    public GameObject rightClickObject;
    public GameObject correctObject;
    public GameObject incorrectObject;

    private GameState state = GameState.WAITING;
    bool shouldLeftClick = false;
    
    // all your attempts. -1 = Failed
    private List<int> attempts = new List<int>();
    
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartWaiting();
    }

    void SetObjectEnabled(GameObject o)
    {
        waitingObject.SetActive(o == waitingObject);
        leftClickObject.SetActive(o == leftClickObject);
        rightClickObject.SetActive(o == rightClickObject);
        correctObject.SetActive(o == correctObject);
        incorrectObject.SetActive(o == incorrectObject);
    }

    void ReadyToClick()
    {
        state = GameState.CLICKING;
        shouldLeftClick = Random.Range(0f, 1f) < 0.5;
        SetObjectEnabled(shouldLeftClick ? leftClickObject : rightClickObject);
    }

    // the user did something wrong, tell them what they did wrong.
    void ShowIncorrect(string message)
    {
        SetObjectEnabled(incorrectObject);
        state = GameState.CLICKED;
        
        attempts.Add(-1);

        Text[] texts = incorrectObject.GetComponentsInChildren<Text>();
        
        foreach (Text text in texts )
        {
            if (text.name.Equals("MessageText"))
            {
                text.text = message;
            }
            else if (text.name.Equals("AttemptText"))
            {
                text.text = "Attempt: " + attempts.Count;
            }
        }
    }
    
    // Show that you got the correct button and your reaction time
    void ShowCorrect()
    {
        SetObjectEnabled(correctObject);
        int reaction = (int) Math.Round(1000 * (Time.time - timer));
        attempts.Add(reaction);
        timer = Time.time;

        state = GameState.CLICKED;
        
        Text[] texts = correctObject.GetComponentsInChildren<Text>();
        
        foreach (Text text in texts )
        {
            if (text.name.Equals("TimeText"))
            {
                text.text = "Reaction Time: " + reaction + "ms";
            }
            else if (text.name.Equals("AttemptText"))
            {
                text.text = "Attempt: " + attempts.Count;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if the timer reaches the point where you 
        if (Time.time >= timer && state == GameState.WAITING)
        {
            ReadyToClick();
        }
        
        // Ready to click - show correct or incorrect screen based on what button you pressed.
        if (state == GameState.CLICKING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (shouldLeftClick)
                    ShowCorrect();
                else
                    ShowIncorrect("Incorrect button! You are meant to right click when the screen turns green.");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (!shouldLeftClick)
                    ShowCorrect();
                else
                    ShowIncorrect("Incorrect button! You are meant to left click when the screen turns blue.");
            }
        }
        // Waiting - don't let the user press the button too early
        else if (state == GameState.WAITING)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                ShowIncorrect("Too fast! Wait until the screen turns blue/green before you press the corresponding button.");
            }
        }
        // Clicked screen - wait 2 seconds
        else if (state == GameState.CLICKED)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
            {
                StartWaiting();
            }
        }
    }

    void StartWaiting()
    {
        SetObjectEnabled(waitingObject);

        state = GameState.WAITING;
        timer = Time.time + Random.Range(1f, 8f);
    }
}