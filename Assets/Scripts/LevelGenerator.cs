using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [Header("Game")]
    public GameObject LevelFinishScreen;

    public GameObject[] RaycastDownPositions; //3 by 3 grid raycast going down
    public GameObject[] RaycastDownOutsidePositions; //Raycasts outside of the grid

    public LayerMask PlacedCubesLayer;


    public bool check;


    public int minCubeCount;
    public int maxCubeCount;
    List<GameObject> cubes = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void GenerateRandomPiece()
    {
        GameObject curObj = null;

        int blockCount = Random.Range(minCubeCount, maxCubeCount + 1);

        GameObject raycastPoint = RaycastDownPositions[Random.Range(0, RaycastDownPositions.Length)];
        RaycastHit hit;
        Physics.Raycast(raycastPoint.transform.position, -Vector3.up, out hit, 100f, PlacedCubesLayer);

        if (hit.collider != null)
        {
            cubes.Add(hit.collider.gameObject); 
            curObj = hit.collider.gameObject;
        }
    }


    private void Update()
    {
        if(check)
        {
            CheckGrid();
            check = false;
        }
    }



    //Level finish

    public void CheckGrid()
    {
        Invoke("CheckGridDelay", 0.5f);
       

    }
    void CheckGridDelay()
    {
        bool falseFlag = false;

        foreach (GameObject g in RaycastDownPositions)
        {
            RaycastHit[] hit;
            hit = Physics.RaycastAll(g.transform.position, -Vector3.up, 5f, PlacedCubesLayer);
            if(hit.Length != 3)
            {
                falseFlag = true;
            }
        }

        int outsideCubesHit = 0;

        foreach (GameObject g in RaycastDownOutsidePositions)
        {
            RaycastHit[] hit;
            hit = Physics.RaycastAll(g.transform.position, -Vector3.up, 5f, PlacedCubesLayer);
            outsideCubesHit += hit.Length;
        }

        if (falseFlag == false && outsideCubesHit == 0)
        {
            LevelFinishScreen.SetActive(true);
        }

    }
    public void Reset()
    {
        LevelFinishScreen.SetActive(false);
    }
}
