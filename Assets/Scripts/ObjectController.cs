using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectController : MonoBehaviour
{
    public float colliderRadius = 32;
    public bool isLeftClick = false;

    public void Move()
    {
        float x = Random.Range(colliderRadius, Screen.width - colliderRadius);
        float y = Random.Range(colliderRadius, Screen.height - colliderRadius);
        transform.position = new Vector3(x, y, 0);
    }


    public bool InsideBounds(Vector3 mousePos)
    {
        float dx = mousePos.x - transform.position.x;
        float dy = mousePos.y - transform.position.y;

        return dx * dx + dy * dy < colliderRadius * colliderRadius;
    }
}