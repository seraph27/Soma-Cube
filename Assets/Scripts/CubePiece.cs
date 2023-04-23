using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ControlFreak2;
using System.Net;

public class CubePiece : MonoBehaviour
{
    public GameObject[] Cubes;

    public Vector3 midPoint;

    public Vector3 GetMidPoint()
    {
        //Gets the middle point of a piece, used for rotation
        Vector3 newpoint = Vector3.zero;
        foreach (GameObject cube in Cubes)
        {
            newpoint += cube.transform.position;
        }
        newpoint /= Cubes.Length;
        return newpoint;
    }
    /*
    // Start is called before the first frame update
    public void AdjustPivotToMidPoint()
    {
        fromPoint = transform.position;

        //Gets the middle point of a piece, used for rotation
        foreach (GameObject cube in Cubes)
        {
            newPoint += cube.transform.position;
        }
        newPoint /= Cubes.Length;

        transform.position = newPoint;


        Vector3 cubeShiftPos = fromPoint - newPoint;
        foreach (GameObject cube in Cubes)
        {
            cube.transform.position += cubeShiftPos;
        }
    }
    */
    void Update()
    {
      
    }
}
