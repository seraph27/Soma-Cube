using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlFreak2;

public class SwipeManager : MonoBehaviour
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public static event Action<SwipeDirection> Swipe;
    public bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;

    void Update()
    {
       // Debug.Log(Input.GetTouch(0).deltaPosition.sqrMagnitude);
        if (CF2Input.touchCount == 0)
            return;

        if (CF2Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
        {
            if (swiping == false)
            {
                swiping = true;
                lastPosition = CF2Input.GetTouch(0).position;
                return;
            }
            else
            {
                if (!eventSent)
                {
                    if (Swipe != null)
                    {
                        Vector2 direction = CF2Input.GetTouch(0).position - lastPosition;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                                Swipe(SwipeDirection.Right);
                            else
                                Swipe(SwipeDirection.Left);
                        }
                        else
                        {
                            if (direction.y > 0)
                                Swipe(SwipeDirection.Up);
                            else
                                Swipe(SwipeDirection.Down);
                        }

                        eventSent = true;
                    }
                }
            }
        }
        else
        {
            swiping = false;
            eventSent = false;
        }
    }
}