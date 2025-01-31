using System;
using UnityEngine;
using System.Collections;

public class DragSpriteRigid : MonoBehaviour
{
    public float dampingRatio = 5.0f;
    public float frequency = 2.5f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float maxX = 19;
    public float maxY = 12;
    public float minY = -11;
    public bool isDragging;
    public float joltForce = 5.0f; // Adjust the jolt force
    private SpringJoint2D springJoint;
    private Camera camera;
    public Rigidbody2D selectedRigidbody;

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (!isDragging)
        {
            if (transform.position.x > maxX)
                transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            else if (transform.position.x < -maxX)
                transform.position = new Vector3(-maxX, transform.position.y, transform.position.z);
            if (transform.position.y > maxY)
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            else if (transform.position.y < minY)
                transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null && hit.rigidbody != null && !hit.rigidbody.isKinematic)
            {
                StartDragging(hit);
            }
            else if (hit.collider == null)
            {
                ApplyJolt(GetMouseWorldPosition());
            }
        }
    }

    void StartDragging(RaycastHit2D hit)
    {
        if (!springJoint)
        {
            GameObject obj = new GameObject("Rigidbody2D dragger");
            Rigidbody2D body = obj.AddComponent<Rigidbody2D>();
            springJoint = obj.AddComponent<SpringJoint2D>();
            body.isKinematic = true;
        }

        springJoint.transform.position = hit.point;
        springJoint.anchor = Vector2.zero;
        springJoint.connectedAnchor = hit.transform.InverseTransformPoint(hit.point);
        springJoint.dampingRatio = dampingRatio;
        springJoint.frequency = frequency;
        springJoint.enableCollision = false;
        springJoint.connectedBody = hit.rigidbody;
        springJoint.distance = 0.2f;
        springJoint.autoConfigureDistance = false;

       // selectedRigidbody = hit.rigidbody;
        StartCoroutine(DragObject());
    }

    IEnumerator DragObject()
    {
        isDragging = true;
        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;

        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;

        while (Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            springJoint.transform.position = mousePos;
            yield return null;
        }

        isDragging = false;
        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    void ApplyJolt(Vector3 targetPosition)
    {
        
            Vector2 direction = (targetPosition - selectedRigidbody.transform.position).normalized;
            selectedRigidbody.AddForce(direction * joltForce, ForceMode2D.Impulse);
        
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
