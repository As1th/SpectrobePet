using System.Collections;
using UnityEngine;

public class DragSpriteRigid : MonoBehaviour
{
    [Header("Spring Joint Settings")]
    public float springStrength = 50.0f;  // Increased for faster response
    public float damper = 0.5f;           // Reduced for less resistance
    public float drag = 5.0f;
    public float angularDrag = 2.0f;

    [Header("Movement Boundaries")]
    public float maxX = 19;
    public float maxY = 12;
    public float minY = -11;

    [Header("Jolt Settings")]
    public float joltForce = 5.0f;

    [HideInInspector]
    public bool isDragging;

    // Static variable to ensure only one object is actively dragged at a time.
    public static DragSpriteRigid activeDragger = null;

    public SpringJoint springJoint;
    private Camera cam;
    public Rigidbody selectedRigidbody;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Only process input for the active dragger (or if no dragger is active, allow new drag).
        if (activeDragger != null && activeDragger != this)
            return;

        // Movement boundaries (only when not dragging)
        if (!isDragging)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Only start dragging if the clicked collider belongs to this object.
                if (hit.collider.gameObject == gameObject)
                {
                    Rigidbody hitRb = hit.collider.attachedRigidbody;
                    if (hitRb != null && !hitRb.isKinematic)
                    {
                        selectedRigidbody = hitRb;
                        activeDragger = this;
                        StartDragging(hit);
                    }
                }
            }
            else if (selectedRigidbody != null)
            {
                // If we clicked empty space and we have a previously dragged object, apply jolt.
                ApplyJolt(GetMouseWorldPosition());
            }
        }
    }

    void StartDragging(RaycastHit hit)
    {

        // Create a unique dragger GameObject for this instance if not already created.
        if (springJoint == null)
        {
            GameObject obj = new GameObject(gameObject.name + " Dragger");
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.isKinematic = true;  // Ensures it moves instantly
            rb.mass = 0.1f;         // Lower mass for less inertia
            springJoint = obj.AddComponent<SpringJoint>();
        }

        springJoint.transform.position = hit.point;
        springJoint.anchor = Vector3.zero;
        springJoint.connectedAnchor = hit.transform.InverseTransformPoint(hit.point);
        springJoint.damper = damper;
        springJoint.spring = springStrength;
        springJoint.enableCollision = false;
        springJoint.connectedBody = hit.collider.attachedRigidbody;
        springJoint.maxDistance = 0.1f;  // Reduce max distance for tighter control

        StartCoroutine(DragObject());
    }

    IEnumerator DragObject()
    {
        isDragging = true;
        Rigidbody rb = springJoint.connectedBody;
        float oldDrag = rb.drag;
        float oldAngularDrag = rb.angularDrag;

        rb.drag = drag;
        rb.angularDrag = angularDrag;

        while (Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            // Smooth follow using Lerp
            springJoint.transform.position = Vector3.Lerp(springJoint.transform.position, mousePos, 0.9f);
            yield return null;
        }

        isDragging = false;
        
            rb.drag = oldDrag;
            rb.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        

        // Reset the active dragger so another object can be dragged.
        activeDragger = null;

    }

    void ApplyJolt(Vector3 targetPosition)
    {
        if (selectedRigidbody != null)
        {
            // If object is currently being dragged, detach it first
            if (springJoint != null && springJoint.connectedBody == selectedRigidbody)
            {
                springJoint.connectedBody = null; // Release from spring
                selectedRigidbody.drag = 0f; // Reset drag
                selectedRigidbody.angularDrag = 0.05f;
                selectedRigidbody.AddForce(Vector3.down * 0.2f, ForceMode.Impulse); // Ensure it falls naturally
            }

            // Apply the jolt force
            Vector3 direction = (targetPosition - selectedRigidbody.transform.position).normalized;
            selectedRigidbody.AddForce(direction * joltForce, ForceMode.Impulse);
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
