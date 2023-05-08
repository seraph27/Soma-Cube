using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] RaycastDownPositions; //3 by 3 grid raycast going down
    public LayerMask DefaultCubesLayer;


    public int minPieceBlockCount;
    public int maxPieceBlockCount;

    GameObject[] cubesForPiece;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void GenerateRandomPiece()
    {
        int blockCount = Random.Range(minPieceBlockCount, maxPieceBlockCount + 1);

        GameObject raycastPoint = RaycastDownPositions[Random.Range(0, RaycastDownPositions.Length)];
        RaycastHit hit;
        Physics.Raycast(raycastPoint.transform.position, -Vector3.up, out hit, 100f, DefaultCubesLayer);

        if (hit.collider != null)
        {

        }
    }
}
