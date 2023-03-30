using UnityEngine;

public class PieceDraggable : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    Vector3 mousePosition;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isDragging)
        {
            // Disable camera rotation while dragging
            mainCamera.GetComponent<CameraController>().enabled = false;

            transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        }
        else
        {
            mousePosition = Input.mousePosition - mainCamera.WorldToScreenPoint(transform.position);
            // Enable camera rotation when not dragging
            mainCamera.GetComponent<CameraController>().enabled = true;
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
