using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlFreak2;

public class SwipeManager : MonoBehaviour
{
    public Vector2 swipeDelta;

    public float DecreaseRate;
    public float Sens;
    Vector2 iniSwipe;
    Vector2 newSwipe;

    void Update()
    {
        if(iniSwipe != newSwipe)
        {
            if (iniSwipe != Vector2.zero)
            {
                swipeDelta = (newSwipe - iniSwipe) * -Sens * Time.deltaTime;
            }
            iniSwipe = newSwipe;
        }

        
        float x = 0;
        float y = 0;
        if(swipeDelta.x > 0)
        {
            x = 1;
        }
        if (swipeDelta.x < 0)
        {
            x = -1;
        }
        if (swipeDelta.y > 0)
        {
            y = 1;
        }
        if (swipeDelta.y < 0)
        {
            y = -1;
        }
        Vector2 a = new Vector2(x,y);

        swipeDelta -= a * Time.deltaTime * DecreaseRate;
        
        if(swipeDelta.magnitude < 0.1f)
        {
            swipeDelta = Vector2.zero;
        }

    }
    public void SwipeVector(Vector2 swipe)
    {
        newSwipe = swipe;
    }
}