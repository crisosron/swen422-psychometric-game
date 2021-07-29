using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint
{
    private float mouseStillTime;
    private float mouseTravelTime;
    private float mouseClickTime;
    private float totalTime;
    private bool wasCorrect;
    private float distanceToCentre;
    private float distanceToPoint;
    private float travelVelocity;
    

    public DataPoint(Vector3 mouseStart, Vector3 pointPosition, float mouseMoveTime, float objectEnterTime, float totalTime, bool wasCorrect)
    {
        mouseStillTime = mouseMoveTime;
        mouseTravelTime = objectEnterTime - mouseMoveTime;
        mouseClickTime = totalTime - objectEnterTime;
        this.totalTime = totalTime;
        this.wasCorrect = wasCorrect;
        
        float dx = mouseStart.x - pointPosition.x;
        float dy = mouseStart.y - pointPosition.y;
        
        float cdx = mouseStart.x - Screen.width / 2f;
        float cdy = mouseStart.y - Screen.height / 2f;

        distanceToCentre = (float) Math.Sqrt(cdx * cdx + cdy * cdy);
        distanceToPoint = (float) Math.Sqrt(dx * dx + dy * dy);

        travelVelocity = distanceToPoint / mouseTravelTime;
    }


    public string toJSON()
    {
        return "{" +
               "\"abstract-images\": " + Settings.isAbstract + "," +
               "\"was-correct\": " + wasCorrect + "," +
               "\"mouse-still-time\": " + mouseStillTime + "," +
               "\"mouse-travel-time\": " + mouseTravelTime + "," +
               "\"mouse-click-time\": " + mouseClickTime + "," +
               "\"mouse-total-time\": " + totalTime + "," +
               "\"start-pos-to-centre-distance\": " + distanceToCentre + "," +
               "\"start-pos-to-point-distance\": " + distanceToPoint + "," +
               "\"travel-velocity\": " + travelVelocity +
               "}";
    }
    
}
