using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    public Image waitingRect;

    public Image leftClickRect;

    public Image rightClickRect;


    bool isWaiting = false;
    bool shouldLeftClick = false;

    float startTime;

    float shouldStopWaitingTime;

    // Start is called before the first frame update
    void Start()
    {
        StartWaiting();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= shouldStopWaitingTime && isWaiting) {
            isWaiting = false;
            waitingRect.enabled = false;
            leftClickRect.enabled = shouldLeftClick;
            rightClickRect.enabled = !shouldLeftClick;

            Debug.Log("You can click now: (" + shouldLeftClick + ")");
        }
        if(!isWaiting) {
            if(Input.GetMouseButtonDown(0)) {
                if(shouldLeftClick) {
                    Debug.Log("Yay you got it");


                    float reaction = Time.time - shouldStopWaitingTime;

                    Debug.Log(reaction);

                }else{

                     Debug.Log("no");
                }
                    StartWaiting();
            } else
            if(Input.GetMouseButtonDown(1)) {
                if(shouldLeftClick) {
                    Debug.Log("No");
                }else{

                    Debug.Log("You gottit");

                     float reaction = Time.time - shouldStopWaitingTime;

                    Debug.Log(reaction);
                }
                    StartWaiting();
            }  
        } else {
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                  Debug.Log("Too fast");
                StartWaiting();
            }
        }
    }

    void StartWaiting()
    {
        waitingRect.enabled = true;
        leftClickRect.enabled = false;
        rightClickRect.enabled = false;
        isWaiting = true;
        startTime = Time.time;
        shouldStopWaitingTime = startTime + Random.Range(1f, 8f);
        shouldLeftClick = Random.Range(0f, 1f) < 0.5;
    }
}
