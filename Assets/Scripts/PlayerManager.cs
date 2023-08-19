using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using ControlFreak2;
using static ControlFreak2.TouchControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Test")]
    public SwipeManager SwipePiece;
    public SwipeManager SwipeGrid;

    public GameObject MainCamera;
    public GameObject emptyPrefab;

    [Header("Button UI")]
    public GameObject RotationButtons;
    public GameObject PlaceRotateButton;
    public GameObject ConfirmButton;


    public GameObject HintButton;


    public Text PlaceRotateButton_Text;
    public Sprite moveSprite;
    public Sprite rotateSprite;
    public Image RotateMoveButtonSprite;

    [Header("Grid")]

   public  List<GameObject> curPieces = new List<GameObject>();

    public GameObject Grid;
    public GameObject GridMiddle;

    [Header("Select Object Handler")]
    public LayerMask raycastLayer;
    public Transform SelectReferencePosition;
    public float swipeThreshold;
    public float swipeSpeed = 2f;
    public float rotationIncrement = 90f;

    bool SelectingPiece;
    bool RotatingPiece;

    public bool RotateMode;
    public bool MoveMode;
    public bool IsMovingPiece;

    GameObject currentPiece;
    CubePiece currentPieceComponent;

    [Header("Rotate Grid")]
    bool RotatingGrid;

    [Header("Colors")]
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
        float x = SwipePiece.swipeDelta.x;
        float y = SwipePiece.swipeDelta.y;
        float x2 = SwipeGrid.swipeDelta.x;
        float y2 = SwipeGrid.swipeDelta.y;

        //Piece Rotation ------------------------------------------------------------------------------------

        if (Input.GetMouseButton(0))
        {
            RotatingPiece = true;
        }
        if (!Input.GetMouseButton(0) && RotatingPiece)
        {
            RotatingPiece = false;
            RotationSnap();
        }

        RotationUpdate(x, y);
        //Grid rotation------------------------------------------------------------------------------------------------------

        if (Input.GetMouseButton(0))
        {
            RotatingGrid = true;
        }
        if (!Input.GetMouseButton(0) && RotatingGrid)
        {
            RotatingGrid = false;
            GridRotationSnap();
        }

        GridRotationUpdate(x2, y2);


        PlaceRotateButton.SetActive(false);
        if (SelectingPiece)
        {
            PlaceRotateButton.SetActive(true);
            if(MoveMode)
            {
                PlaceRotateButton_Text.text = "Rotate";
                RotateMoveButtonSprite.sprite = rotateSprite;
            }
            if (RotateMode)
            {
                PlaceRotateButton_Text.text = "Move";
                RotateMoveButtonSprite.sprite = moveSprite;
            }
        }
        ConfirmButton.SetActive(false);
        if (MoveMode)
        {
            ConfirmButton.SetActive(true);
        }



        //MOVE MODE------------------------------------------------------------------------------------------------------
        MoveUpdate();
        HintUpdate();
    }

    public void RotateMoveButton()
    {
        if (RotateMode)
        {
            MovePiece(); //Stop rotationmode and go to move mode
        }
        else if (MoveMode)
        {
            RotatePiece(currentPiece);
        }
    }

    public void ResetAllPieces()
    {
        ResetPiece();
        RotateMode = false;
        MoveMode = false;

        foreach(GameObject g in curPieces)
        {
            g.transform.position = new Vector3(0, -5, 0);
        }
    }

    public void RotatePieceButton(int piece)
    {
        RotatePiece(curPieces[piece]);
    }
    //Select a piece and bring it in front of the screen, Rotation mode
     void RotatePiece(GameObject piece)
    {
        if (currentPiece != null && MoveMode) 
        {
            PlacePiece();
        }
        ResetPiece();

        MoveMode = false;
        RotateMode = true;

        RotationButtons.SetActive(true);

        currentPiece = piece;

        currentPiece.transform.position = SelectReferencePosition.position;
        SelectingPiece = true;

        currentPieceComponent = currentPiece.GetComponent<CubePiece>();
        currentPieceComponent.IsMoving = false;

        SetLayerRecursively(currentPiece, 2);
        currentPiece.transform.parent = null;

        RotationSnap();
    }

    
    public void PlacePiece() //places down current piece
    {
        RotateMode = false;
        MoveMode = false;
        placedPieces.Add(currentPiece);
        if (currentPieceComponent != null)
        {
            currentPieceComponent.PlacePiece();
            SetLayerRecursively(currentPiece, 6);
            currentPiece.transform.parent = Grid.transform;
        }

        PlaceRotateButton.SetActive(true);
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





    ////////////////////////////////////////////////////


    // MOVING  //


    ////////////////////////////////////////////////////

    //Go from rotation to move mode
    public void MovePiece()
    {
        currentPieceComponent.MovePiece();

        RotatingPiece = false;
        RotateMode = false;
        MoveMode = true;
        RotationButtons.SetActive(false);

        MoveUpdateStart();
        
    }

    void MoveUpdateStart() //move the piece to the estimated center of the screen
    {

        RaycastHit hit;
        if (Physics.Raycast(SelectReferencePosition.transform.position,-Vector3.up, out hit, Mathf.Infinity, raycastLayer))
        {
            midPoint = currentPieceComponent.GetMidPoint();
            Vector3 offset = midPoint - currentPiece.transform.position;
            currentPiece.transform.position = hit.point - offset + Vector3.up * 20f;

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
        IsMovingPiece = false;
        if (MoveMode) //MOVING THE OBJECT OUT OF VIEW OF THE CAMERA WITH AN OFFSET, FOR PIECE TO RAYCAST DOWN AND MOVE THE CLONE PIECE
        {
            float xScreenHalf = Screen.height / 2;
            if (Input.GetMouseButton(0) && Input.mousePosition.y < xScreenHalf)
            {
                IsMovingPiece = true;

                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, raycastLayer))
                {
                    midPoint = currentPieceComponent.GetMidPoint();
                    Vector3 offset = midPoint - currentPiece.transform.position;
                    currentPiece.transform.position = hit.point - offset + Vector3.up * 20f;

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




    ////////////////////////////////////////////////////


    // ROTATION  //


    ////////////////////////////////////////////////////

    void RotationUpdate(float rotationX, float rotationY)
    {
        if (SelectingPiece)
        {
            if (RotatingPiece)
            {
                // currentPiece.transform.rotation *= Camera.transform.tra Quaternion.Euler(rotationX, rotationY, 0);
                currentPiece.transform.RotateAround(SelectReferencePosition.transform.position, -transform.up, rotationX * swipeSpeed);
                currentPiece.transform.RotateAround(SelectReferencePosition.transform.position, transform.right, rotationY * swipeSpeed);
            }
            else if (MoveMode)
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



    Quaternion snapRotationGrid  = Quaternion.identity; 
    void GridRotationUpdate(float rotationX, float rotationY)
    {
        if (RotatingGrid && !MoveMode)
        {
            // currentPiece.transform.rotation *= Camera.transform.tra Quaternion.Euler(rotationX, rotationY, 0);
            Grid.transform.RotateAround(GridMiddle.transform.position, -transform.up, rotationX * swipeSpeed);
        }
        else
        {
            //Lerp Piece to closest rotation snap
            Grid.transform.rotation = Quaternion.Lerp(Grid.transform.rotation, snapRotationGrid, 10f * Time.deltaTime);
        }
    }

    void GridRotationSnap()
    {
        if (Grid != null)
        {
            float x = (Mathf.Round(Grid.transform.eulerAngles.x / rotationIncrement) * rotationIncrement);
            float y = (Mathf.Round(Grid.transform.eulerAngles.y / rotationIncrement) * rotationIncrement);
            float z = (Mathf.Round(Grid.transform.eulerAngles.z / rotationIncrement) * rotationIncrement);

            Vector3 newrot = new Vector3(x, y, z);
            snapRotationGrid = Quaternion.Euler(newrot);
        }
    }




    ////////////////////////////////////////////////////


    // CHECKS  //


    ////////////////////////////////////////////////////

    void HintUpdate()
    {
        if (MoveMode)
        {
            HintButton.SetActive(true);
        }
        else
        {
            HintButton.SetActive(false);
        }
    }

    public void HintOpen()
    {
        currentPieceComponent.HintCheck();
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





    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
