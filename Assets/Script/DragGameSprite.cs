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

    // Boundary constraints for world position (X and Y)
    public float maxX = 19;
    public float minX = -19;
    public float maxY = 12;
    public float minY = -11;

    // Boundary constraints for Z axis
    public float maxZ = 10;
    public float minZ = -14;

    [Header("Menu Transition Settings")]
    public float menuTransitionTime = 0.5f; // Duration of the menu scale transition

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        StartCoroutine(RandomWalk());
    }

    // Smoothly toggles the menu by scaling it.
    public void OpenMenu()
    {
        if (Menu.activeSelf)
        {
            rotateMode = false;
            StartCoroutine(SmoothCloseMenu());
        }
        else
        {
            StartCoroutine(SmoothOpenMenu());
        }
    }

    IEnumerator SmoothOpenMenu()
    {
        // Activate the menu and set its scale to zero.
        Menu.SetActive(true);
        Menu.transform.localScale = Vector3.zero;
        Vector3 targetScale = new Vector3(7.5f, 0.46f, 3.3f);
        float t = 0;
        while (t < menuTransitionTime)
        {
            t += Time.deltaTime;
            Menu.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / menuTransitionTime);
            yield return null;
        }
        Menu.transform.localScale = targetScale;
    }

    IEnumerator SmoothCloseMenu()
    {
        Vector3 initialScale = Menu.transform.localScale;
        float t = 0;
        while (t < menuTransitionTime)
        {
            t += Time.deltaTime;
            Menu.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / menuTransitionTime);
            yield return null;
        }
        Menu.transform.localScale = Vector3.zero;
        Menu.SetActive(false);
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

        EnforceBounds();
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

    // Enforces the specified boundaries by clamping the object's position.
    void EnforceBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }

    // Clamp position while dragging (X, Y, and Z) using defined boundaries.
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
        float clampedZ = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        return new Vector3(clampedX, clampedY, clampedZ);
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
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-direction, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 500);
        }
    }

    // Random walk coroutine that moves the object along its local X and Y axes.
    IEnumerator RandomWalk()
    {
        while (true)
        {
            float waitTime = Random.Range(walkIntervalMin, walkIntervalMax);
            yield return new WaitForSeconds(waitTime);

            if (!isDragging && !rotateMode && (Menu == null || !Menu.activeSelf) && walkCycle)
            {
                animator.SetBool("IsWalking", true);
                float randomLocalX = Random.Range(-walkLocalRangeX, walkLocalRangeX);
                float randomLocalY = Random.Range(-walkLocalRangeY, walkLocalRangeY);

                Vector3 targetPos = transform.position + (transform.right * randomLocalX) + (transform.forward * randomLocalY);

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
                targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);

                while (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    if (isDragging || rotateMode || (Menu != null && Menu.activeSelf))
                    {
                        animator.SetBool("IsWalking", false);
                        break;
                    }

                    Vector3 moveDirection = targetPos - transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
                    FaceDirection(moveDirection);

                    yield return null;
                }
                animator.SetBool("IsWalking", false);
            }
        }
    }
}
