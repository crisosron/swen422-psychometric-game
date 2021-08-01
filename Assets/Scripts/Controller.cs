using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public enum GameState
    {
        WAITING,
        CLICKING,
        CLICKED,
        RESULTS
    }

    // Game objects to be enabled/disabled
    public GameObject waitingObject;
    public GameObject clickingObject;
    public GameObject endRoundObject;
    public GameObject resultsObject;

    // Objects to click on
    public ObjectController red;
    public ObjectController green;
    public ObjectController alien;
    public ObjectController coin;

    private GameState state = GameState.WAITING;

    private Vector3 prevPosition;
    private ObjectController objectToClick = null;

    // all your attempts with the data collected
    private int score = 0;
    private List<DataPoint> attempts = new List<DataPoint>();

    private float timer;
    private float mouseMovedTime;
    private float mouseEnteredTime;
    private Vector3 mouseStartLocation;


    // Use this for prod
    public const string SERVER_URI = "https://swen422-telemetry-server.herokuapp.com/user-entries/create";

    // Use this for dev
    // public const string SERVER_URI = "http://localhost:5000/user-entries/create";

    // Start is called before the first frame update
    void Start()
    {
        StartWaiting();
    }

    void SetObjectEnabled(GameObject o)
    {
        waitingObject.SetActive(o == waitingObject);
        clickingObject.SetActive(o == clickingObject);
        endRoundObject.SetActive(o == endRoundObject);
        resultsObject.SetActive(o == resultsObject);
    }

    void ReadyToClick()
    {
        state = GameState.CLICKING;

        // pick object at random
        bool isLeft = Random.Range(0f, 1f) < 0.5;

        mouseMovedTime = 0;
        mouseEnteredTime = 0;
        mouseStartLocation = prevPosition;

        SetObjectEnabled(clickingObject);

        red.gameObject.SetActive(false);
        green.gameObject.SetActive(false);
        alien.gameObject.SetActive(false);
        coin.gameObject.SetActive(false);

        if (isLeft)
            if (Settings.isAbstract)
                objectToClick = green;
            else
                objectToClick = alien;
        else if (Settings.isAbstract)
            objectToClick = red;
        else
            objectToClick = coin;

        objectToClick.gameObject.SetActive(true);
        objectToClick.Move();
    }

    void ShowResults()
    {
        state = GameState.RESULTS;
        SetObjectEnabled(resultsObject);

        StartCoroutine(SendResults());

        Text[] texts = resultsObject.GetComponentsInChildren<Text>();

        foreach (Text text in texts)
        {
            if (text.name.Equals("ResultText"))
            {
                text.text = "Finished!\nYour final score: " + score;
            }
        }
    }

    // Send results results to companion telemetry web app
    IEnumerator SendResults()
    {
        List<string> json = new List<string>();

        foreach (DataPoint dp in attempts)
        {
            json.Add(dp.toJSON());
        }

        string arrayString = string.Join(", ", json);
        string jsonString = "{ \"attempts\": [" + arrayString + "] }";
        
        Debug.Log(jsonString);

        UnityWebRequest req = new UnityWebRequest(SERVER_URI, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        req.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
    }

    // the user did something wrong, tell them what they did wrong.
    void ShowIncorrect(string message)
    {
        SetObjectEnabled(endRoundObject);
        state = GameState.CLICKED;

        Text[] texts = endRoundObject.GetComponentsInChildren<Text>();

        DataPoint dp = new DataPoint(mouseStartLocation, objectToClick.transform.position, mouseMovedTime,
            mouseEnteredTime, Time.time - timer, false);
        attempts.Add(dp);

        foreach (Text text in texts)
        {
            if (text.name.Equals("EndText"))
            {
                text.text = message + "\nAttempt: " + attempts.Count + "/10";
            }
        }
    }

    // Show that you got the correct button and your reaction time
    void ShowCorrect()
    {
        SetObjectEnabled(endRoundObject);
        int reaction = (int) Math.Round(1000 * (Time.time - timer));
        int points = (int) (1000 * Math.Max(0, 2 - Time.time + timer));


        state = GameState.CLICKED;

        score += points;
        DataPoint dp = new DataPoint(mouseStartLocation, objectToClick.transform.position, mouseMovedTime,
            mouseEnteredTime, Time.time - timer, true);
        attempts.Add(dp);

        Text[] texts = endRoundObject.GetComponentsInChildren<Text>();

        foreach (Text text in texts)
        {
            if (text.name.Equals("EndText"))
            {
                text.text = "You got it!\nTime Taken: " + reaction + "ms\nPoints Earned: " +
                            points + "\nAttempt: " + attempts.Count + "/10";
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
        timer = Time.time + Random.Range(2f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseChange = prevPosition - Input.mousePosition;
        // Waiting
        if (state == GameState.WAITING)
        {
            // if the player moves their mouse then keep waiting
            if (!mouseChange.Equals(new Vector3(0, 0, 0)))
            {
                StartWaiting();
            }

            // Get ready
            if (Time.time >= timer)
            {
                ReadyToClick();
            }
        }
        else if (state == GameState.CLICKING)
        {
            if (!mouseChange.Equals(new Vector3(0, 0, 0)) && mouseMovedTime == 0)
            {
                mouseMovedTime = Time.time - timer;
                
            }

            if (objectToClick.InsideBounds(Input.mousePosition) && mouseEnteredTime == 0)
            {
                mouseEnteredTime = Time.time - timer;

                // when the user spawns inside the point
                if (mouseMovedTime == 0) mouseMovedTime = mouseEnteredTime;
            }


            if (Input.GetMouseButtonDown(0) && objectToClick.InsideBounds(Input.mousePosition))
            {
                if (objectToClick.isLeftClick)
                    ShowCorrect();
                else
                    ShowIncorrect(Settings.isAbstract
                        ? "Wrong button! You are supposed to left click the green circle."
                        : "Wrong button! You are supposed to left click to shoot the alien!");
            }
            else if (Input.GetMouseButtonDown(1) && objectToClick.InsideBounds(Input.mousePosition))
            {
                if (!objectToClick.isLeftClick)
                    ShowCorrect();
                else
                    ShowIncorrect(Settings.isAbstract
                        ? "Wrong button! You are supposed to right click the red circle."
                        : "Wrong button! You are supposed to right click to collect the coin!");
            }
        }

        prevPosition = Input.mousePosition;
    }
}