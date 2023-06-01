using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ControlFreak2;
using System.Net;

public class CubePiece : MonoBehaviour
{

    List<GameObject> Cubes = new List<GameObject>();

    List<GameObject> CloneCubes = new List<GameObject>();

    List<Vector3> Positions = new List<Vector3>();

    public Vector3 midPoint;

    public bool IsMoving;

    GameObject clone;

    PlayerManager playerManager;

    Material inimat;

    private void Start()
    {

        playerManager = FindObjectOfType<PlayerManager>();

        SetCubes();
        CreateClone();

        transform.position = new Vector3(0, -5f, 0);
        inimat = Cubes[0].GetComponent<MeshRenderer>().material;
    }

    public void SetCubes()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Cubes.Add(child.gameObject);
            Positions.Add(child.gameObject.transform.position);
        }
    }

    void CreateClone()
    {
        clone = Instantiate(gameObject);
        Destroy(clone.GetComponent<CubePiece>()); //Remove the cubepiece component for the clone so it doesnt clone itself infintely
        SetLayerRecursively(clone, 2);

        for (int i = 0; i < clone.transform.childCount; i++)
        {
            Transform child = clone.transform.GetChild(i);
            CloneCubes.Add(child.gameObject);
        }
    }
    void LateUpdate()
    {
        clone.SetActive(false);

        if (IsMoving)
        {
            clone.SetActive(true);

            float minDistance = 50f;
            foreach (GameObject cube in Cubes)
            {
                RaycastHit hit;
                Physics.Raycast(cube.transform.position, -Vector3.up, out hit, Mathf.Infinity, playerManager.raycastLayer);
                if(hit.collider != null)
                {
                    minDistance = Mathf.Min(minDistance,(hit.point - cube.transform.position).magnitude);
                }
            }

            clone.transform.position = transform.position + -Vector3.up * minDistance + new Vector3(0,0.5f,0); 
            clone.transform.rotation = transform.rotation;
        }
    }


    public void MovePiece()
    {
        IsMoving = true;
    }
    public void PlacePiece()
    {
        transform.position = clone.transform.position;
        IsMoving = false;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }



    public Vector3 GetMidPoint()
    {
        //Gets the middle point of a piece, used for rotation
        Vector3 newpoint = Vector3.zero;
        foreach (GameObject cube in Cubes)
        {
            newpoint += cube.transform.position;
        }
        newpoint /= Cubes.Count;
        return newpoint;
    }


    bool HintMode;

    public void HintCheck()
    {
        Debug.Log("Checking");
        foreach (GameObject cube in CloneCubes)
        {
            cube.GetComponent<MeshRenderer>().material = playerManager.redTranslucent;
            Debug.Log("RED");

            foreach (Vector3 inipos in Positions)
            {
                if (cube.transform.position == inipos)
                {
                    cube.GetComponent<MeshRenderer>().material = playerManager.greenTranslucent;
                }

            }
        }
        HintMode = true;
    }

    public void FinishHint()
    {
        Debug.Log("Finish Check");

        foreach (GameObject cube in CloneCubes)
        {
            cube.GetComponent<MeshRenderer>().material = inimat;
        }
        HintMode = false;
    }

    void Update()
    {
        if((!playerManager.MoveMode || playerManager.IsMovingPiece) && HintMode )
        {
            FinishHint();
        }
    }
}
