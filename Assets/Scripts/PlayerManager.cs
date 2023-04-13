using ControlFreak2;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour
{
    public GameObject TestObject;
    public GameObject Camera;

    [Header("Select Object Handler")]
    public Transform SelectReferencePosition;
    public float swipeThreshold;
    public float swipeSpeed = 2f;

    public float rotationIncrement = 90f;

    bool Rotating;
    GameObject currentPiece;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = CF2Input.GetAxis("Mouse X");
        float y = CF2Input.GetAxis("Mouse Y");

        if (CF2Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectPiece(TestObject);
        }

        Rotating = false;
        if(Mathf.Abs(x) > swipeThreshold || Mathf.Abs(y) > swipeThreshold)
        {
            Rotating = true;
        }
        RotationUpdate(x,y);
    }

    //Select a piece and bring it in front of the screen
    void SelectPiece(GameObject PieceSelected)
    {
        PieceSelected.transform.position = SelectReferencePosition.position;
        currentPiece = PieceSelected;

        CubePiece piece = currentPiece.GetComponent<CubePiece>();

        piece.AdjustPivotToMidPoint(); //change the pivot point of the piece to be the middle of the cubes

        piece.transform.position = SelectReferencePosition.transform.position;
        piece.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
    }

    void RotationUpdate(float rotationX, float rotationY)
    {
        if (Rotating)
        {
            // currentPiece.transform.rotation *= Camera.transform.tra Quaternion.Euler(rotationX, rotationY, 0);
            currentPiece.transform.RotateAround(currentPiece.transform.position, -transform.up, rotationX * swipeSpeed);
            currentPiece.transform.RotateAround(currentPiece.transform.position, transform.right, rotationY * swipeSpeed);
        }
        else
        {
            if (currentPiece != null)
            {
                float x = (Mathf.Round(currentPiece.transform.eulerAngles.x / rotationIncrement) * rotationIncrement);
                float y = (Mathf.Round(currentPiece.transform.eulerAngles.y / rotationIncrement) * rotationIncrement);
                float z = (Mathf.Round(currentPiece.transform.eulerAngles.z / rotationIncrement) * rotationIncrement);


                currentPiece.transform.eulerAngles = new Vector3(x,y,z);
            }
        }
    }

    void RotationSnap()
    {
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (Mathf.Round(transform.eulerAngles.y / increment) * increment), transform.eulerAngles.z);
    }
}
