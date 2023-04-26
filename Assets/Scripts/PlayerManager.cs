using ControlFreak2;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour
{
    public GameObject TestObject;
    public GameObject MainCamera;

    [Header("Button UI")]
    public GameObject RotationButtons;

    [Header("Select Object Handler")]
    public LayerMask raycastLayer;

    public Transform SelectReferencePosition;
    public float swipeThreshold;
    public float swipeSpeed = 2f;

    public float rotationIncrement = 90f;

    bool SelectingPiece;
    bool Rotating;
    bool Moving;

    GameObject currentPiece;
    CubePiece piece;

    Vector3 midPoint;
    Quaternion snapRotation;

    // Start is called before the first frame update
    void Start()
    {
        RotationButtons.SetActive(false);

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


        if(Mathf.Abs(x) > swipeThreshold || Mathf.Abs(y) > swipeThreshold)
        {
            Rotating = true;
        }
        if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0 && Rotating)
        {
            Rotating = false;
            RotationSnap();
        }
        RotationUpdate(x,y);

        bool moveInput = CF2Input.GetButtonDown("Fire1");
        if(moveInput)
        {
            MovePiece(); //Stop rotationmode and go to move mode
        }
        MoveUpdate();
        
    }

    //Select a piece and bring it in front of the screen, Rotation mode
    void SelectPiece(GameObject PieceSelected)
    {
        Moving = false;
        RotationButtons.SetActive(true);

        PieceSelected.transform.position = SelectReferencePosition.position;
        currentPiece = PieceSelected;
        SelectingPiece = true;

        piece = currentPiece.GetComponent<CubePiece>();
        currentPiece.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        SetLayerRecursively(currentPiece, 2);

        RotationSnap();
    }
    void UnselectPiece()
    {
        SetLayerRecursively(currentPiece,0);
        currentPiece = null;
        SelectingPiece = false;
    }





    void RotationUpdate(float rotationX, float rotationY)
    {
        if (SelectingPiece)
        {
            if (Rotating)
            {
                // currentPiece.transform.rotation *= Camera.transform.tra Quaternion.Euler(rotationX, rotationY, 0);
                currentPiece.transform.RotateAround(SelectReferencePosition.transform.position, -transform.up, rotationX * swipeSpeed);
                currentPiece.transform.RotateAround(SelectReferencePosition.transform.position, transform.right, rotationY * swipeSpeed);
            }
            else if(Moving)
            {
                currentPiece.transform.rotation = snapRotation;
            }
            else
            {
                //Lerp Piece to closest rotation snap
                currentPiece.transform.rotation = Quaternion.Lerp(currentPiece.transform.rotation, snapRotation, 10f * Time.deltaTime);
                midPoint = piece.GetMidPoint();
                Vector3 moveVec = SelectReferencePosition.transform.position - midPoint;
                piece.transform.position += moveVec;
            }
        }
    }

    void RotationSnap()
    {
        if (currentPiece != null)
        {
            float x = (Mathf.Round(currentPiece.transform.eulerAngles.x / rotationIncrement) * rotationIncrement);
            float y = (Mathf.Round(currentPiece.transform.eulerAngles.y / rotationIncrement) * rotationIncrement);
            float z = (Mathf.Round(currentPiece.transform.eulerAngles.z / rotationIncrement) * rotationIncrement);


            Vector3 newrot = new Vector3(x, y, z);
            snapRotation = Quaternion.Euler(newrot);
        }
    }


    //Go from rotation to move mode
    public void MovePiece()
    {
        currentPiece.transform.localScale = new Vector3(1f, 1f, 1f);

        Rotating = false;
        Moving = true;
        RotationButtons.SetActive(false);
    }

    void MoveUpdate()
    {
        if (Moving)
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity,raycastLayer))
            {
                midPoint = piece.GetMidPoint();
                Vector3 offset = midPoint - currentPiece.transform.position;
                currentPiece.transform.position = hit.point - offset;

                // Round the position to the nearest integer
                Vector3 roundedPosition = new Vector3(
                    Mathf.Round(currentPiece.transform.position.x),
                    Mathf.Round(currentPiece.transform.position.y),
                    Mathf.Round(currentPiece.transform.position.z)
                );

                currentPiece.transform.position = roundedPosition;
            }

        }
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
