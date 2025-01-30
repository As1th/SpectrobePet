using System;
using UnityEngine;
using System.Collections;

public class DragSpriteRigid : MonoBehaviour
{
    public float dampingRatio = 5.0f;
    public float frequency = 2.5f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;

    private SpringJoint2D springJoint;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        //
        // If the player did not press the mouse button down, do not run
        // through Update().
        //
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        //
        // Raycast into the scene from the camera to detect objects
        //
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        //
        // Prerequisites for dragging a GameObject. Should be
        // self-explanatory, I hope!
        //
        if (hit.collider == null || !hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }

        //
        // SpringJoint2D creation.
        //
        if (!springJoint)
        {
            GameObject obj = new GameObject("Rigidbody2D dragger");
            Rigidbody2D body = obj.AddComponent<Rigidbody2D>();
            this.springJoint = obj.AddComponent<SpringJoint2D>();
            body.isKinematic = true;
        }

        //
        // SpringJoint2D property setting.
        //
        springJoint.transform.position = hit.point;
        springJoint.anchor = Vector2.zero;
        springJoint.connectedAnchor = hit.transform.InverseTransformPoint(hit.point);
        springJoint.dampingRatio = this.dampingRatio;
        springJoint.frequency = this.frequency;
        springJoint.enableCollision = false;
        springJoint.connectedBody = hit.rigidbody;
        springJoint.distance = 0.2f;
        springJoint.autoConfigureDistance = false;

        //
        // Start dragging coroutine
        //
        StartCoroutine(DragObject());
    }

    IEnumerator DragObject()
    {
        //
        // Save the original drag values
        //
        float oldDrag = this.springJoint.connectedBody.drag;
        float oldAngularDrag = this.springJoint.connectedBody.angularDrag;

        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;

        //
        // Drag while mouse button is held
        //
        while (Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            springJoint.transform.position = mousePos;
            yield return null;
        }

        //
        // Reset properties when released
        //
        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    //
    // Convert mouse position to world position in a perspective camera
    //
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // Plane at z = 0
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
