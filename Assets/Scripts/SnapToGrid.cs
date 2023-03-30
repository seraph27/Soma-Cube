using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    private void Update()
    {
        // Get the current position of the object
        Vector3 currentPosition = transform.position;

        // Round the position to the nearest integer
        Vector3 roundedPosition = new Vector3(
            Mathf.Round(currentPosition.x),
            Mathf.Round(currentPosition.y),
            Mathf.Round(currentPosition.z)
        );

        // Set the position of the object to the rounded position
        transform.position = roundedPosition;
    }
}
