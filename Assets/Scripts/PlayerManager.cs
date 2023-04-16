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

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = CastRay();
        }
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

    IEnumerator FlashScreen()
    {
        float duration = 0.2f; 
        float t = 0f;
        while (t < duration) {
            Camera.main.backgroundColor = Color.red;
            yield return null;
            t += Time.deltaTime;
        }
        Camera.main.backgroundColor = Color.white; 
    }

    RaycastHit CastRay()
    {   
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        // check collider near the ray cast cus 
        float sphereRadius = 0.2f;
        Collider[] hitColliders = Physics.OverlapSphere(hit.point, sphereRadius);
        if (hitColliders.Length > 0)
        {
            // closest point outside of an obstacle
            Vector3 closestPoint = hit.point;   
            float minDistance = float.MaxValue;
            foreach (Collider collider in hitColliders)
            {
                Vector3 closest = collider.ClosestPoint(hit.point);
                float distance = Vector3.Distance(hit.point, closest);
                if (distance < minDistance)
                {
                    closestPoint = closest;
                    minDistance = distance;
                }
            }
            TestObject.transform.localScale = new Vector3(1f,1f,1f);
            closestPoint = new Vector3(Mathf.Round(closestPoint.x), Mathf.Round(closestPoint.y), Mathf.Round(closestPoint.z));
            TestObject.transform.position = closestPoint;
            StartCoroutine(FlashScreen()); // flash screen
        }
        else
        {
            Vector3 snappedPoint = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
            TestObject.transform.position = snappedPoint;
        }
    
        }
        return hit;
    }
}
