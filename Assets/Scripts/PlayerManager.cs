using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using ControlFreak2;
using static ControlFreak2.TouchControl;

public class PlayerManager : MonoBehaviour
{
    [Header("Test")]

    public GameObject TestObject;
    public GameObject TestObject2;
    public GameObject TestObject3;


    public GameObject MainCamera;

    [Header("Button UI")]
    public GameObject RotationButtons;

    [Header("Grid")]
    public GameObject Grid;
    public GameObject GridMiddle;

    [Header("Select Object Handler")]
    public LayerMask raycastLayer;
    public Transform SelectReferencePosition;
    public float swipeThreshold;
    public float swipeSpeed = 2f;
    public float rotationIncrement = 90f;

    bool SelectingPiece;
    bool Rotating;

    bool RotateMode;
    bool MoveMode;

    GameObject currentPiece;
    CubePiece currentPieceComponent;

    Vector3 midPoint;
    Quaternion snapRotation;
    public Transform gridMidPoint; //** seraph ** 
    List<GameObject> placedPieces = new List<GameObject>(); //** seraph **

    LevelGenerator levelgenerator;

    // Start is called before the first frame update
    void Start()
    {
        RotationButtons.SetActive(false);
        levelgenerator = FindObjectOfType<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = CF2Input.GetAxis("Mouse X");
        float y = CF2Input.GetAxis("Mouse Y");



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


        //for testing
        if (CF2Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectPiece(TestObject);
        }
        if (CF2Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectPiece(TestObject2);
        }
        if (CF2Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectPiece(TestObject3);
        }

        if (CF2Input.GetKeyDown(KeyCode.R))
        {
            ResetAllPieces();
        }
        if (CF2Input.GetKeyDown(KeyCode.Q))
        {
            GridRotateLeft();
        }
        if (CF2Input.GetKeyDown(KeyCode.W))
        {
            GridRotateRight();
        }
        if (CF2Input.GetKeyDown(KeyCode.E))
        {
            if (RotateMode)
            {
                Debug.Log("MoveMode");
                MovePiece(); //Stop rotationmode and go to move mode
            }
            else if(MoveMode)
            {
                PlacePiece();
            }
        }
        
        MoveUpdate();
        
    }


    public void ResetAllPieces()
    {
        ResetPiece();
        RotateMode = false;
        MoveMode = false;
        TestObject.transform.position = new Vector3(0, -5, 0);
        TestObject2.transform.position = new Vector3(0, -5, 0);
        TestObject3.transform.position = new Vector3(0, -5, 0);
    }
    //Select a piece and bring it in front of the screen, Rotation mode
    public void SelectPiece(GameObject PieceSelected)
    {
        ResetPiece();

        MoveMode = false;
        RotateMode = true;

        RotationButtons.SetActive(true);


        PieceSelected.transform.position = SelectReferencePosition.position;
        currentPiece = PieceSelected;
        SelectingPiece = true;

        currentPieceComponent = currentPiece.GetComponent<CubePiece>();
        currentPiece.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentPieceComponent.IsMoving = false;

        SetLayerRecursively(currentPiece, 2);

        RotationSnap();
    }
    void PlacePiece() //places down current piece
    {
        RotateMode = false;
        MoveMode = false;
        placedPieces.Add(currentPiece);
        if (currentPieceComponent != null)
        {
            currentPieceComponent.PlacePiece();
            SetLayerRecursively(currentPiece, 6);
        }

        currentPieceComponent = null;
        currentPiece = null;
        SelectingPiece = false;

        //check if player filled all slots
        levelgenerator.CheckGrid();
    }
    void CancelPieceEdit() //reset piece if currently in rotation mode and switching to another piece
    {
        RotateMode = false;
        MoveMode = false;
        currentPiece.transform.position = new Vector3(0, -5, 0);
        currentPiece.transform.localScale = new Vector3(1f,1f,1f);
        currentPieceComponent = null;
        SelectingPiece = false;

    }
    //check if theres currently a piece being selected in either move or rotate mode

    void ResetPiece()
    {
        if (currentPiece != null)
        {
            if (RotateMode)
            {
                CancelPieceEdit();
            }
            if (MoveMode)
            {
                CancelPieceEdit();
            }
        }
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
            else if(MoveMode)
            {
                currentPiece.transform.rotation = snapRotation;
            }
            else
            {
                //Lerp Piece to closest rotation snap
                currentPiece.transform.rotation = Quaternion.Lerp(currentPiece.transform.rotation, snapRotation, 10f * Time.deltaTime);
                midPoint = currentPieceComponent.GetMidPoint();
                Vector3 moveVec = SelectReferencePosition.transform.position - midPoint;
                currentPieceComponent.transform.position += moveVec;
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
        currentPieceComponent.MovePiece();

        Rotating = false;
        RotateMode = false;
        MoveMode = true;
        RotationButtons.SetActive(false);

        MoveUpdateStart();
        
    }

    void MoveUpdateStart() //move the piece to the estimated center of the screen
    {

        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            midPoint = currentPieceComponent.GetMidPoint();
            Vector3 offset = midPoint - currentPiece.transform.position;
            currentPiece.transform.position = hit.point - offset + Vector3.up * 10f;

            // Round the position to the nearest integer
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(currentPiece.transform.position.x),
                Mathf.Round(currentPiece.transform.position.y),
                Mathf.Round(currentPiece.transform.position.z)
            );

            currentPiece.transform.position = roundedPosition;
        }
    }

    void MoveUpdate()
    {
        if (MoveMode) //MOVING THE OBJECT OUT OF VIEW OF THE CAMERA WITH AN OFFSET, FOR PIECE TO RAYCAST DOWN AND MOVE THE CLONE PIECE
        {
            if (CF2Input.GetKey(KeyCode.F))
            {
                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, raycastLayer))
                {
                    midPoint = currentPieceComponent.GetMidPoint();
                    Vector3 offset = midPoint - currentPiece.transform.position;
                    currentPiece.transform.position = hit.point - offset + Vector3.up * 10f;

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
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    public void GridRotateLeft()
    {
        ResetPiece();
        Grid.transform.RotateAround(GridMiddle.transform.position, transform.up, -90f);
    }

    public void GridRotateRight()
    {
        ResetPiece();
        Grid.transform.RotateAround(GridMiddle.transform.position, transform.up, 90f);
    }

    public void IsInGrid(){
        //write code that highlkights if the piece is withint the 3x3 place
        foreach(GameObject piece in placedPieces){
            Vector3 midPoint = piece.GetComponent<CubePiece>().GetMidPoint();
            if(Mathf.Abs(midPoint.x - gridMidPoint.position.x) > 1.1f ||  //1.1 to prevent floating error 
            Mathf.Abs(midPoint.y - gridMidPoint.position.y) > 1.1f ||
            Mathf.Abs(midPoint.z - gridMidPoint.position.z) > 1.1f) {
                Renderer[] ren = piece.GetComponentsInChildren<Renderer>();
                foreach(Renderer r in ren){
                    r.material.color = Color.black; //black if not in grid (idk how to check for correct spot yet)
                }
                //Debug.Log(Mathf.Abs(midPoint.x - gridMidPoint.position.x) + ", Y:" + Mathf.Abs(midPoint.y - gridMidPoint.position.y) + ", Z:" + Mathf.Abs(midPoint.z - gridMidPoint.position.z));
            }
            
        }

        
        
    }





}
