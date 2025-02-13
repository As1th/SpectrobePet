using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

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

    [Header("Petting Settings")]
    [Tooltip("Minimum movement (in screen pixels) required to trigger petting. Increase to reduce sensitivity.")]
    public float pettingThreshold = 100f;
    [Tooltip("Cooldown (in seconds) between pet triggers.")]
    public float petCooldown = 0.5f;
    [Tooltip("Delay (in seconds) after petting before triggering Idle animation.")]
    public float IdleDelay = 2.0f;
    private float petTimer = 0f;
    private Vector3 lastPetMousePosition;
    private Outline outline;
    public SpriteRenderer rotateIcon;
    void Start()
    {
        outline = GetComponent<Outline>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        StartCoroutine(RandomWalk());
        // Initialize lastPetMousePosition to the current mouse position.
        lastPetMousePosition = Input.mousePosition;
    }

    public void Eat()
    {
        animator.SetTrigger("Joy");
        StartCoroutine(SetIdle());
    }

    // Smoothly toggles the menu by scaling it.
    public void OpenMenu()
    {
        if (Menu.activeSelf)
        {
            rotateMode = false;
            rotateIcon.color = new Color(1, 1, 1, 1);
            StartCoroutine(SmoothCloseMenu());
            outline.enabled = false;
        }
        else
        {
            
            StartCoroutine(SmoothOpenMenu());
            outline.enabled = true;
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
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            transform.Rotate(-deltaMouse.x * rotationSpeed * Vector3.up, Space.World);
            transform.Rotate(-deltaMouse.y * rotationSpeed * Vector3.left, Space.World);
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
                transform.Rotate(mainCamera.transform.forward, rotationSpeed*4, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(mainCamera.transform.forward, -rotationSpeed*4, Space.World);
            }
        }

        EnforceBounds();

        // Petting detection: when the mouse is rapidly moved over the object.
        // Only trigger petting if the mouse button is NOT held down.
        if (isMouseOver && !isDragging && !rotateMode  && !Input.GetMouseButton(0))
        {
            petTimer -= Time.deltaTime;
            Vector3 currentMousePos = Input.mousePosition;
            float delta = (currentMousePos - lastPetMousePosition).magnitude;
            if (delta > pettingThreshold && petTimer <= 0)
            {
                Pet();
                petTimer = petCooldown;
            }
            lastPetMousePosition = currentMousePos;
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
        if (lastPetMousePosition == Vector3.zero)
            lastPetMousePosition = Input.mousePosition;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }

    // Makes the object face in the given direction (only adjusts yaw).
    void FaceDirectionYaw(Vector3 direction)
    {
        Vector3 horizontalDir = Vector3.ProjectOnPlane(direction, transform.up);
        if (horizontalDir.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(-horizontalDir, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime * 500);
        }
    }

    // Random walk coroutine that moves the object along its local horizontal plane.
    IEnumerator RandomWalk()
    {
        while (true)
        {
            float waitTime = Random.Range(walkIntervalMin, walkIntervalMax);
            yield return new WaitForSeconds(waitTime);

            if (!isDragging && !rotateMode && (Menu == null || !Menu.activeSelf) && walkCycle && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetBool("IsWalking", true);

                // Generate random offsets along the object's local right and forward.
                float randomOffsetX = Random.Range(-walkLocalRangeX, walkLocalRangeX);
                float randomOffsetZ = Random.Range(-walkLocalRangeY, walkLocalRangeY); // Using walkLocalRangeY for forward direction.

                Vector3 targetPos = transform.position + (transform.right * randomOffsetX) + (transform.forward * randomOffsetZ);
                targetPos.y = transform.position.y;

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
                targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);

                while (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    if (isDragging || rotateMode || (Menu != null && Menu.activeSelf) || animator.GetCurrentAnimatorStateInfo(0).IsName("Joy") || animator.GetCurrentAnimatorStateInfo(0).IsName("Pet"))
                    {
                        animator.SetBool("IsWalking", false);
                        break;
                    }

                    Vector3 moveDirection = targetPos - transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
                    FaceDirectionYaw(moveDirection);

                    yield return null;
                }
                animator.SetBool("IsWalking", false);
            }
        }
    }

    // This function is called when the mouse is rapidly moved over the object ("petting").
    void Pet()
    {
        Debug.Log(gameObject.name + " has been petted!");
        animator.SetTrigger("Pet");
        StartCoroutine(SetIdle());
    }

    // After petting, trigger Idle after a set delay.
    IEnumerator SetIdle()
    {
        yield return new WaitForSeconds(IdleDelay);
        animator.SetTrigger("Idle");
    }
}
