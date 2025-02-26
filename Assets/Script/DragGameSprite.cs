using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class DragGameSprite : MonoBehaviour
{
    private int speciesID = 3;
    public GameManager manager;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera mainCamera;
    public GameObject Menu;
    public GameObject Ring;
    public float zoomSpeed = 2.0f; // Zoom sensitivity
    private bool isMouseOver = false;
    public float boundaryMargin = 0.5f;
    public bool isDragging = false;
    private Vector3 initialMousePosition;
    public bool switchMode = false;
    public bool rotateMode = false; // Toggle for rotation mode
    public float rotationSpeed = 5.0f; // Rotation sensitivity (used for both manual and random turning)
    private Vector3 lastMousePosition;
    public bool walkCycle;
    private Animator animator;
    public GameObject particlePet;
    public GameObject particleJoy;
    public GameObject particleAngry;
    public GameObject particleSad;
    // Random walk settings
    public float walkSpeed = 2.0f;
    public float walkIntervalMin = 2.0f;
    public float walkIntervalMax = 5.0f;

    // Allowed random-walk range along local axes (i.e. along transform.right and transform.forward)
    public float walkLocalRangeX = 8f;
    public float walkLocalRangeY = 8f;
    public float walkMaxTime = 4f;

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
    public SpriteRenderer switchIcon;
    public SpriteRenderer rotateIcon;
    public SpriteRenderer walkIcon;

    public float repeatRate = 0.5f; // Time interval between triggers
    private float nextTriggerTime = 0f;
    private bool keyHeld = false;

    [Header("Random Animation Settings")]
    public float randomAnimIntervalMin = 120f;  // Minimum wait time in seconds before triggering a random animation
    public float randomAnimIntervalMax = 240f;  // Maximum wait time
    public float randomAnimDuration = 10f;      // Duration of the random animation before returning to Idle

    
    public float happiness = 100f;
    void Start()
    {

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Menu = manager.Menu;
        Ring = manager.Ring;
        switchIcon = manager.switchIcon;
        rotateIcon = manager.rotateIcon;
        walkIcon = manager.walkIcon;
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(RandomWalk());
        StartCoroutine(RandomAnimationRoutine());
        // Initialize lastPetMousePosition to the current mouse position.
        lastPetMousePosition = Input.mousePosition;
        walkMaxTime =  walkLocalRangeX / walkSpeed;
        outline = GetComponentInChildren<Outline>();
       
    }


    public void Eat(GameObject min)
    {
        manager.currentMinerals.Remove(min);
        if (min.GetComponent<DragSpriteRigid>().springJoint != null)
        {
            Destroy(min.GetComponent<DragSpriteRigid>().springJoint.gameObject);
        }
        Destroy(min);
        happiness = Mathf.Min(100, (happiness + 50));
        animator.SetTrigger("Joy");
        Instantiate(particleJoy, transform);
        StartCoroutine(SetIdle());
    }

    public void resetIcon(SpriteRenderer icon)
    {
        icon.color = new Color(1, 1, 1, 1);
    }

    // Smoothly toggles the menu by scaling it.
    public void OpenMenu()
    {
        if (Menu.activeSelf && Menu.GetComponent<UIFollowGameObject>().targetObject == this.transform)
        {
            rotateMode = false;
            switchMode = false;
            resetIcon(rotateIcon);
            resetIcon(switchIcon);
           
            StartCoroutine(SmoothCloseRing());
            StartCoroutine(SmoothCloseMenu());
            outline.enabled = false;
            
        }
        else
        {
            outline.enabled = true;
            
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
            if (walkCycle)
            {
                              walkIcon.color = new Color(1, 1, 1, 0.6f);
            }
            else
            {
                             walkIcon.color = new Color(1, 1, 1, 1f);
            }

            Ring.transform.localScale = Vector3.zero;
            Ring.SetActive(false);
            OpenMenu();


            
            resetIcon(rotateIcon);
            resetIcon(switchIcon);
            if(manager.SelectedSpectrobe != this)
            {
                manager.SelectedSpectrobe.outline.enabled = false;
                manager.SelectedSpectrobe.rotateMode = false;
                manager.SelectedSpectrobe.switchMode = false;
            }
            
            
            Menu.GetComponent<UIFollowGameObject>().targetObject = this.transform;
            Ring.GetComponent<UIFollowGameObject>().targetObject = this.transform;
            manager.SelectedSpectrobe = this;
            
               
            
          

            
            

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
        } else if(switchMode)
        {

            if (Input.GetKeyDown(KeyCode.W)) // Immediate trigger on press
            {
                IncrementSpecies();
                keyHeld = true;
                nextTriggerTime = Time.time + repeatRate;
            }
            else if (Input.GetKey(KeyCode.W) && keyHeld && Time.time >= nextTriggerTime) // Slow repeat
            {
                IncrementSpecies();
                nextTriggerTime = Time.time + repeatRate;
            }
            else if (Input.GetKeyUp(KeyCode.W)) // Reset when released
            {
                keyHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.S)) // Immediate trigger on press
            {
                DecrementSpecies();
                keyHeld = true;
                nextTriggerTime = Time.time + repeatRate;
            }
            else if (Input.GetKey(KeyCode.S) && keyHeld && Time.time >= nextTriggerTime) // Slow repeat
            {
                DecrementSpecies();
                nextTriggerTime = Time.time + repeatRate;
            }
            else if (Input.GetKeyUp(KeyCode.S)) // Reset when released
            {
                keyHeld = false;
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

        float decayRate = 100 / 1800f; // 30 minutes (1800 seconds) to go from startingHappiness to 0.
        happiness = Mathf.Max(0, happiness - decayRate * Time.deltaTime);
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            Vector3 zoomDirection = (transform.position - mainCamera.transform.position).normalized;
            transform.position += zoomDirection * scrollInput * zoomSpeed;
            print(scrollInput);
            
        }
    }

    public void IncrementSpecies()
    {

        speciesID++;
        if (speciesID > manager.spectrobeSpecies.Count - 1)
        { speciesID = 0;
        }
        ChangeSpectrobeModel();
    }
    public void DecrementSpecies()
    {

        speciesID--;
        if (speciesID < 0 )
        {
            speciesID = manager.spectrobeSpecies.Count - 1;
        }
        ChangeSpectrobeModel();
    }
    public void ChangeSpectrobeModel()
    { 
        Destroy(transform.GetChild(0).gameObject);
        transform.DetachChildren();
        if (transform.childCount == 0 ) {
        
            var mon = Instantiate(manager.spectrobeSpecies[speciesID], transform);
            animator = mon.GetComponent<Animator>();
            outline = mon.GetComponent<Outline>();
            foreach (GameObject min in manager.currentMinerals)
            {
                foreach (GameObject spec in manager.currentSpectrobes)
                {
                    min.GetComponentInChildren<ScreenSpaceCollisionDetector>().otherObjects.Add(spec.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
                }
            }
        }




    }
    // Enforces the specified boundaries by clamping the object's position.
    void EnforceBounds()
    {
        // Determine the current depth from the object's position.
        float depth = mainCamera.WorldToScreenPoint(transform.position).z;
        // Get the world-space boundaries at that depth.
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, depth));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));

        // Use the object's current z value to compute a scale factor.
        float currentZ = transform.position.z;
        float scaleFactor = 1f;
        if (currentZ >= 0)
        {
            // For positive z, use a smaller divisor to boost the effect.
            scaleFactor = 1 + (currentZ / 128f);
        }
        else
        {
            // For negative z, use a larger divisor for a milder effect.
            scaleFactor = 1 + (currentZ / 32f);
        }

        // Compute an effective margin based on the scale factor.
        float effectiveMargin = boundaryMargin / scaleFactor;

        // Adjust the bounds with the effective margin.
        minBounds.x += effectiveMargin;
        minBounds.y += effectiveMargin - 2f; // The "-2f" adjustment as in your original code.
        maxBounds.x -= effectiveMargin;
        maxBounds.y -= effectiveMargin;

        // Clamp the object's position based on the adjusted boundaries.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }


    // Clamp position while dragging (X, Y, and Z) using defined boundaries.
    private Vector3 ClampToScreenBounds(Vector3 targetPosition)
    {
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, screenPoint.z));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenPoint.z));

        // Get the current depth from the object's world position (optional if you need it elsewhere)
        float currentZ = transform.position.z;
        float scaleFactor = 1f;
        if (currentZ >= 0)
        {
          
            // For positive Z, use a smaller divisor (e.g., 16) to increase the scale factor faster.
            scaleFactor = 1 + (currentZ / 128f);
            
        }
        else
        {
            // For negative Z, use a larger divisor (e.g., 32) so the effect is milder.
            scaleFactor = 1 + (currentZ / 32f);
        }

        // Compute an effective margin; when scaleFactor is larger, effectiveMargin becomes smaller.
        float effectiveMargin = boundaryMargin / scaleFactor;

        minBounds.x += effectiveMargin;
        minBounds.y += effectiveMargin -2f;
        maxBounds.x -= effectiveMargin;
        maxBounds.y -= effectiveMargin;


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
    IEnumerator SmoothCloseRing()
    {
        Vector3 initialScale = Ring.transform.localScale;
        float t = 0;
        while (t < menuTransitionTime)
        {
            t += Time.deltaTime;
            Ring.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / menuTransitionTime);
            yield return null;
        }
        Ring.transform.localScale = Vector3.zero;
        Ring.SetActive(false);
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
                
               

                targetPos = ClampToScreenBounds(targetPos);
                targetPos = transform.position + Vector3.ProjectOnPlane(targetPos - transform.position, transform.up);
                // Initialize a timer.
                float elapsedTime = 0f;
                while (Vector3.Distance(transform.position, targetPos) > 0.05f && elapsedTime < walkMaxTime)
                {
                    if (isDragging || rotateMode || (Menu != null && Menu.activeSelf) ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("Joy") || animator.GetCurrentAnimatorStateInfo(0).IsName("Pet"))
                    {
                        animator.SetBool("IsWalking", false);
                        break;
                    }

                    Vector3 moveDirection = targetPos - transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
                    FaceDirectionYaw(moveDirection);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                animator.SetBool("IsWalking", false);
            }
        }
    }

    IEnumerator RandomAnimationRoutine()
    {
        while (true)
        {
            // Wait a random interval (in seconds) between random animations.
            float interval = Random.Range(randomAnimIntervalMin, randomAnimIntervalMax);
            yield return new WaitForSeconds(interval);

            // Only trigger if the animator is currently in Idle.
            if (!isDragging && !rotateMode && (Menu == null || !Menu.activeSelf) && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                if (happiness >= 40)
                {
                    // Choose a random trigger from "Random1" to "Random4".
                    int randomIndex = Random.Range(1, 5);  // generates 1, 2, 3, or 4
                    animator.SetTrigger("Random" + randomIndex);
                }
                else if (happiness > 5)
                {
                    animator.SetTrigger("Sad");
                    Instantiate(particleSad, transform);
                }
                else
                {
                    animator.SetTrigger("Angry");
                    Instantiate(particleAngry, transform);
                }
                // Wait for the random animation to play for the desired duration.
                yield return new WaitForSeconds(randomAnimDuration);

                // After the duration, return to Idle.
                StartCoroutine(SetIdle());
            }
        }
    }

    // This function is called when the mouse is rapidly moved over the object ("petting").
    void Pet()
    {
        Instantiate(particlePet, transform);
        animator.SetTrigger("Pet");
        happiness = Mathf.Min(100, (happiness + 10));
        
        StartCoroutine(SetIdle());
    }

    // After petting, trigger Idle after a set delay.
    IEnumerator SetIdle()
    {
        yield return new WaitForSeconds(IdleDelay);
        animator.SetTrigger("Idle");
    }
}
