using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameSprite : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 previousMousePosition; // Track the previous mouse position for rotation calculation
    private bool isDraggingForRotation = false; // Track if rotation dragging is active

    public float zoomSpeed = 2.0f; // Adjust this value for zoom sensitivity
    private bool isMouseOver = false; // Track if the mouse is hovering over the GameObject

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl)) // Left-click for dragging position
        {
            // Calculate the distance between the camera and the object
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            // Calculate the offset between the object's world position and the mouse position
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)
            );
        }
        else if (Input.GetMouseButton(1) || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))) // Right-click or Ctrl+Left-click for rotation
        {
            // Start tracking mouse position for rotation
            previousMousePosition = Input.mousePosition;
            isDraggingForRotation = true;
        }
    }

    void OnMouseDrag()
    {
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl)) // Left-click drag to change position
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            // Convert the screen point back to world coordinates and apply the offset
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

            transform.position = curPosition;
        }
        else if (isDraggingForRotation) // Right-click or Ctrl+Left-click drag to change rotation
        {
            Vector3 currentMousePosition = Input.mousePosition;

            // Calculate mouse movement
            Vector3 mouseDelta = currentMousePosition - previousMousePosition;

            // Rotate the object based on the mouse movement
            float rotationSpeed = 1.2f; // Adjust this value for sensitivity
            float rotationX = -mouseDelta.y * rotationSpeed; // Vertical mouse movement rotates around the X-axis
            float rotationY = mouseDelta.x * rotationSpeed;  // Horizontal mouse movement rotates around the Y-axis

            transform.Rotate(-rotationX, -rotationY, 0, Space.World); // Rotate in world space

            previousMousePosition = currentMousePosition; // Update the previous mouse position
        }
    }

    void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0)) // Stop dragging for rotation
        {
            isDraggingForRotation = false;
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

    void OnMouseOver()
    {
        isMouseOver = true; // Set the flag when the mouse is hovering over the GameObject
    }

    void OnMouseExit()
    {
        isMouseOver = false; // Reset the flag when the mouse leaves the GameObject
    }
}
