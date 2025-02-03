using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGameSprite : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera mainCamera;
    public GameObject Menu;
    public float zoomSpeed = 2.0f; // Zoom sensitivity
    private bool isMouseOver = false;
    public float boundaryMargin = 0.5f;
    private bool isDragging = false;
    private Vector3 initialMousePosition;
    public bool rotateMode = false; // Toggle for rotation mode
    public float rotationSpeed = 5.0f; // Rotation sensitivity
    private Vector3 lastMousePosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OpenMenu()
    {
        Menu.SetActive(!Menu.activeSelf);
    }

    void OnMouseDown()
    {
        initialMousePosition = Input.mousePosition;
        lastMousePosition = Input.mousePosition; // Track last position for rotation
        isDragging = false;

        if (!rotateMode)
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = transform.position - Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)
            );
        }
    }

    void OnMouseDrag()
    {
        if (Vector3.Distance(initialMousePosition, Input.mousePosition) > 5.0f)
        {
            isDragging = true;
        }

        if (rotateMode)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationX = mouseDelta.y * rotationSpeed;
            float rotationY = -mouseDelta.x * rotationSpeed;

            // Rotate based on camera's perspective
            transform.Rotate(mainCamera.transform.right, rotationX, Space.World); // Pitch
            transform.Rotate(Vector3.up, rotationY, Space.World); // Yaw

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            curPosition = ClampToScreenBounds(curPosition);
            transform.position = curPosition;
        }
    }

    void OnMouseUp()
    {
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

        if (rotateMode)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(mainCamera.transform.forward, rotationSpeed, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(mainCamera.transform.forward, -rotationSpeed, Space.World);
            }
        }
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            Vector3 zoomDirection = (transform.position - Camera.main.transform.position).normalized;
            transform.position += zoomDirection * scrollInput * zoomSpeed;
        }
    }

    private Vector3 ClampToScreenBounds(Vector3 targetPosition)
    {
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, screenPoint.z));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenPoint.z));

        minBounds.x += boundaryMargin;
        minBounds.y += boundaryMargin;
        maxBounds.x -= boundaryMargin;
        maxBounds.y -= boundaryMargin;

        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }

    void OnMouseOver()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }
}
