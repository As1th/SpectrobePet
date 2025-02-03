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
    public bool isDragging = false;
    private Vector3 initialMousePosition;
    public bool rotateMode = false; // Toggle for rotation mode
    public float rotationSpeed = 5.0f; // Rotation sensitivity (used for both manual and random turning)
    private Vector3 lastMousePosition;
    public bool walkCycle;
    private Animator animator;
    // Random walk settings
    public float walkSpeed = 2.0f;
    public float walkIntervalMin = 2.0f;
    public float walkIntervalMax = 5.0f;

    // Allowed random-walk range along local axes (i.e. along transform.right and transform.forward)
    public float walkLocalRangeX = 5.0f;
    public float walkLocalRangeY = 3.0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        StartCoroutine(RandomWalk());
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
            screenPoint = mainCamera.WorldToScreenPoint(transform.position);
            offset = transform.position - mainCamera.ScreenToWorldPoint(
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

            // Rotate based on camera's perspective.
            transform.Rotate(mainCamera.transform.right, rotationX, Space.World); // Pitch
            transform.Rotate(Vector3.up, rotationY, Space.World); // Yaw

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenPoint) + offset;
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
        else
        {
            isDragging = false;
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
            Vector3 zoomDirection = (transform.position - mainCamera.transform.position).normalized;
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

    // Makes the object face in the given direction.
    void FaceDirection(Vector3 direction)
    {
        // Only change rotation if there's a significant movement direction.
        if (direction.sqrMagnitude > 0.001f)
        {
            // Invert the direction so that the object's "front" faces the movement direction.
            // (This assumes the sprite's graphic is drawn so that its back faces transform.forward.)
            Quaternion targetRotation = Quaternion.LookRotation(-direction, transform.up);
            // Smoothly rotate toward the target rotation.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 500);
        }
    }

    // Random walk coroutine that moves the object along its local X and Y axes.
    IEnumerator RandomWalk()
    {
        while (true)
        {
            
            // Wait a random interval.
            float waitTime = Random.Range(walkIntervalMin, walkIntervalMax);
            yield return new WaitForSeconds(waitTime);

            // Only move if not being dragged, rotated, and the menu is closed.
            if (!isDragging && !rotateMode && (Menu == null || !Menu.activeSelf) && walkCycle)
            {
                animator.SetBool("IsWalking", true);
                // Pick a random offset in the object's local coordinate system.
                float randomLocalX = Random.Range(-walkLocalRangeX, walkLocalRangeX);
                float randomLocalY = Random.Range(-walkLocalRangeY, walkLocalRangeY);
                // Calculate the target position using the object's local axes.
                // (Using transform.right for local X and transform.forward for local Y)
                Vector3 targetPos = transform.position +
                    (transform.right * randomLocalX) +
                    (transform.forward * randomLocalY);

                // Smoothly move toward the target position.
                while (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    
                    // If the object is dragged, rotated, or the menu is open, break out.
                    if (isDragging || rotateMode || (Menu != null && Menu.activeSelf))
                    {
                        animator.SetBool("IsWalking", false);
                        break;
                        
                    }

                    // Calculate movement direction.
                    Vector3 moveDirection = targetPos - transform.position;
                    // Move toward the target.
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
                    // Update rotation to face movement direction.
                    FaceDirection(moveDirection);

                    yield return null;
                }
                animator.SetBool("IsWalking", false);
            }
        }
    }
}
