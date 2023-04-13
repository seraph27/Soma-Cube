using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ControlFreak2;

public class CubePiece : MonoBehaviour
{
    public GameObject[] Cubes;

    // Start is called before the first frame update
    public void AdjustPivotToMidPoint()
    {
        Vector3 midPoint = Vector3.zero;
        Vector3 iniPos = transform.position;

        //Gets the middle point of a piece, used for rotation
        foreach (GameObject cube in Cubes)
        {
            midPoint += cube.transform.position;
        }
        midPoint /= Cubes.Length;

        transform.position = midPoint;


        Vector3 cubeShiftPos = iniPos - midPoint;
        foreach (GameObject cube in Cubes)
        {
            cube.transform.position += cubeShiftPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
