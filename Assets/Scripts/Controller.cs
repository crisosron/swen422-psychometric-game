using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public enum GameState
    {
        WAITING, CLICKING, CLICKED, RESULTS
    }
    
    // Game objects to be enabled/disabled
    public GameObject waitingObject;
    public GameObject leftClickObject;
    public GameObject rightClickObject;
    public GameObject correctObject;
    public GameObject incorrectObject;
    public GameObject resultsObject;

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
        resultsObject.SetActive(o == resultsObject);
    }

    void ReadyToClick()
    {
        state = GameState.CLICKING;
        shouldLeftClick = Random.Range(0f, 1f) < 0.5;
        SetObjectEnabled(shouldLeftClick ? leftClickObject : rightClickObject);
    }

    void ShowResults()
    {
        state = GameState.RESULTS;
        SetObjectEnabled(resultsObject);

        int avg = 0;
        int failed = 0;
        int min = Int32.MaxValue;
        int max = Int32.MinValue;

        foreach (int i in attempts)
        {
            if (i == -1)
            {
                failed++;
            }
            else
            {
                if (i > max)
                    max = i;
                if (i < min)
                    min = i;
                avg += i;
            }
        }

        avg /= attempts.Count - failed;
        
        Text[] texts = resultsObject.GetComponentsInChildren<Text>();
        
        foreach (Text text in texts )
        {
            if (text.name.Equals("ResultText"))
            {
                text.text = "Average Time: " + avg +
                            "ms\nBest Time: " + min +
                            "ms\nWorst Time: " + max +
                            "ms\nFails: " + failed;
            }
        }
        
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
                text.text = "Attempt: " + attempts.Count + "/10";
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
                text.text = "Attempt: " + attempts.Count + "/10";
            }
        }
    }

    public void Next()
    {
        if (attempts.Count >= 10)
            ShowResults();
        else
            StartWaiting();
    }

    public void Quit()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Restart()
    {
        attempts.Clear();
        StartWaiting();
    }

    void StartWaiting()
    {
        SetObjectEnabled(waitingObject);

        state = GameState.WAITING;
        timer = Time.time + Random.Range(1f, 6f);
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
    }
}