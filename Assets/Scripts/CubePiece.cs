using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ControlFreak2;
using System.Net;

public class CubePiece : MonoBehaviour
{

    List<GameObject> Cubes = new List<GameObject>();

    public Vector3 midPoint;

    public bool IsMoving;

    GameObject clone;

    PlayerManager playerManager;

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


    private void Start()
    {
        
        playerManager = FindObjectOfType<PlayerManager>();

        SetCubes();
        CreateClone();
    }

    public void SetCubes()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Cubes.Add(child.gameObject);
        }
    }

    void CreateClone()
    {
        clone = Instantiate(gameObject);
        Destroy(clone.GetComponent<CubePiece>()); //Remove the cubepiece component for the clone so it doesnt clone itself infintely
        SetLayerRecursively(clone, 2);

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
}
