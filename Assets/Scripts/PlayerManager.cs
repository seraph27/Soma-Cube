using ControlFreak2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject TestObject;
    public GameObject Camera;

    [Header("Select Object Handler")]
    public Transform SelectReferencePosition;
    public float swipeThreshold;

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

    void SelectPiece(GameObject PieceSelected)
    {
        PieceSelected.transform.position = SelectReferencePosition.position;
        currentPiece = PieceSelected;
    }

    void RotationUpdate(float rotationX, float rotationY)
    {
        if(Rotating)
        {
            // currentPiece.transform.rotation *= Camera.transform.tra Quaternion.Euler(rotationX, rotationY, 0);
            currentPiece.transform.RotateAround(currentPiece.transform.position, Camera.transform.up, rotationX);
            currentPiece.transform.RotateAround(currentPiece.transform.position, Camera.transform.right, rotationY);

        }
    }

    void RotationSnap()
    {
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, (Mathf.Round(transform.eulerAngles.y / increment) * increment), transform.eulerAngles.z);
    }
}
