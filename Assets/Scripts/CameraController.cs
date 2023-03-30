using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform pivot; // The pivot point to rotate around
    public float speed = 1.0f; // The sensitivity of the rotation
    public float minFov = 35f;
    public float maxFov = 90f;
    public float sensitivity = 10f;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                transform.RotateAround(pivot.position, transform.up, delta.x * speed);
                transform.RotateAround(pivot.position, transform.right, delta.y * -speed);
            }
        }

        if (Input.GetMouseButton(0))
        {   
            
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
            transform.RotateAround(pivot.position, transform.up, delta.x * speed);
            transform.RotateAround(pivot.position, transform.right, delta.y * -speed);
        }

        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;

    }
}
