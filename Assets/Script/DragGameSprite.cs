using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameSprite : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera mainCamera;
    public GameObject Menu;
    public float zoomSpeed = 2.0f; // Adjust this value for zoom sensitivity
    private bool isMouseOver = false; // Track if the mouse is hovering over the GameObject
    public float boundaryMargin = 0.5f;
    private bool isDragging = false; // Track if dragging is happening
    private Vector3 initialMousePosition; // Track the mouse position on click

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OpenMenu()
    {
        if (Menu.activeSelf)
        {
            Menu.SetActive(false);
        }
        else
        {
            Menu.SetActive(true); 
        }
    }

    void OnMouseDown()
    {
        // Calculate the distance between the camera and the object
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        // Calculate the offset between the object's world position and the mouse position
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)
        );

        // Store the initial mouse position
        initialMousePosition = Input.mousePosition;

        // Reset dragging flag
        isDragging = false;
    }

    void OnMouseDrag()
    {
        // Check if the mouse moved significantly to qualify as a drag
        if (Vector3.Distance(initialMousePosition, Input.mousePosition) > 5.0f) // Threshold for drag detection
        {
            isDragging = true;
        }

        // Dragging logic for left-click
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        // Convert the screen point back to world coordinates and apply the offset
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        // Clamp position to screen bounds
        curPosition = ClampToScreenBounds(curPosition);

        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        // Trigger the custom function only if no drag occurred
        if (!isDragging)
        {
            OpenMenu();
        }
    }

    void Update()
    {
        if (isMouseOver)
        {
            HandleZoom();
        }
    }

    void HandleZoom()
    {
        // Get the scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // Calculate the zoom direction
            Vector3 zoomDirection = (transform.position - Camera.main.transform.position).normalized;

            // Adjust the GameObject's position based on the scroll input
            transform.position += zoomDirection * scrollInput * zoomSpeed;
        }
    }

    private Vector3 ClampToScreenBounds(Vector3 targetPosition)
    {
        // Get screen bounds in world space
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, screenPoint.z));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenPoint.z));

        // Apply the boundary margin
        minBounds.x += boundaryMargin;
        minBounds.y += boundaryMargin;
        maxBounds.x -= boundaryMargin;
        maxBounds.y -= boundaryMargin;

        // Clamp the position within the shrunk bounds
        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }

    void OnMouseOver()
    {
        isMouseOver = true; // Set the flag when the mouse is hovering over the GameObject
    }

    void OnMouseExit()
    {
        isMouseOver = false; // Reset the flag when the mouse leaves the GameObject
    }
}
