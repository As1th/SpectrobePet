using System.Collections;
using UnityEngine;

public class DragSpriteRigid : MonoBehaviour
{
    [Header("Spring Joint Settings")]
    public float dampingRatio = 5.0f;
    public float frequency = 2.5f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;

    [Header("Movement Boundaries")]
    public float maxX = 19;
    public float maxY = 12;
    public float minY = -11;

    [Header("Jolt Settings")]
    public float joltForce = 5.0f; // Adjust the jolt force

    [HideInInspector]
    public bool isDragging;

    private SpringJoint springJoint;
    private Camera cam;
    public Rigidbody selectedRigidbody;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // If not dragging, clamp this object's position within the defined boundaries.
        if (!isDragging)
        {
            Vector3 pos = transform.position;
            if (pos.x > maxX)
                pos.x = maxX;
            else if (pos.x < -maxX)
                pos.x = -maxX;

            if (pos.y > maxY)
                pos.y = maxY;
            else if (pos.y < minY)
                pos.y = minY;

            transform.position = pos;
        }

        // On mouse down, perform a raycast into the scene.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // If a collider with a non-kinematic Rigidbody is hit, start dragging it.
                Rigidbody hitRb = hit.collider.attachedRigidbody;
                if (hitRb != null && !hitRb.isKinematic)
                {
                    // Store the hit rigidbody as the selected object.
                    selectedRigidbody = hitRb;
                    StartDragging(hit);
                }
            }
            // If no collider was hit, and we have a selected rigidbody, apply a jolt force.
            else if (selectedRigidbody != null)
            {
                ApplyJolt(GetMouseWorldPosition());
            }
        }
    }

    void StartDragging(RaycastHit hit)
    {
        // Create a helper GameObject with a Rigidbody and SpringJoint if not already created.
        if (springJoint == null)
        {
            GameObject obj = new GameObject("Rigidbody dragger");
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            springJoint = obj.AddComponent<SpringJoint>();
        }

        // Set up the spring joint.
        springJoint.transform.position = hit.point;
        springJoint.anchor = Vector3.zero;
        // Calculate the anchor point relative to the hit object.
        Vector3 localHitPoint = hit.transform.InverseTransformPoint(hit.point);
        springJoint.connectedAnchor = localHitPoint;

        springJoint.damper = dampingRatio;
        springJoint.spring = frequency;
        springJoint.enableCollision = false;
        springJoint.connectedBody = hit.collider.attachedRigidbody;
        // Set a small distance to keep the object near the mouse.
        springJoint.maxDistance = 0.2f;

        StartCoroutine(DragObject());
    }

    IEnumerator DragObject()
    {
        isDragging = true;

        // Cache original drag values.
        Rigidbody rb = springJoint.connectedBody;
        float oldDrag = rb.drag;
        float oldAngularDrag = rb.angularDrag;

        // Apply new drag settings for smoother dragging.
        rb.drag = drag;
        rb.angularDrag = angularDrag;

        while (Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            springJoint.transform.position = mousePos;
            yield return null;
        }

        // When dragging ends, reset the drag values.
        isDragging = false;
        if (springJoint.connectedBody)
        {
            rb.drag = oldDrag;
            rb.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    void ApplyJolt(Vector3 targetPosition)
    {
        if (selectedRigidbody != null)
        {
            Vector3 direction = (targetPosition - selectedRigidbody.transform.position).normalized;
            selectedRigidbody.AddForce(direction * joltForce, ForceMode.Impulse);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Define a plane for converting mouse screen position to world coordinates.
        // This example uses a plane with a normal of Vector3.forward (i.e., the XY plane).
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
