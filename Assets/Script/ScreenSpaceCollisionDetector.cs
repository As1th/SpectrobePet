using UnityEngine;
using System.Collections.Generic;

public class ScreenSpaceCollisionDetector : MonoBehaviour
{
    [Tooltip("Other objects to check for screen-space collisions. They must also have a Renderer component.")]
    public List<GameObject> otherObjects;

    private Camera mainCamera;
    private Renderer rend;

    void Start()
    {
        mainCamera = Camera.main;
        rend = GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogError("ScreenSpaceCollisionDetector requires a Renderer component on " + gameObject.name);
        }
    }

    void Update()
    {
        if (rend == null || otherObjects == null || otherObjects.Count == 0)
            return;

        // Compute our own screen rectangle.
        Rect myRect = GetScreenRect(rend);

        // Check against every other object's screen rectangle.
        foreach (GameObject obj in otherObjects)
        {
            if (obj == null)
                continue;

            Renderer otherRend = obj.GetComponent<Renderer>();
            if (otherRend == null)
                continue;

            Rect otherRect = GetScreenRect(otherRend);
            if (myRect.Overlaps(otherRect))
            {
                // A screen-space collision has been detected.
                Debug.Log(gameObject.name + " collides (in screen space) with " + obj.name);
                obj.GetComponentInParent<DragGameSprite>().Eat(this.gameObject.transform.parent.gameObject);

                
                // Insert any collision-handling logic here.
            }
        }
    }

    /// <summary>
    /// Calculates a screen-space rectangle for the given Renderer based on its world-space bounds.
    /// </summary>
    /// <param name="rend">Renderer from which to calculate the bounds</param>
    /// <returns>A Rect representing the object's bounds in screen coordinates</returns>
    Rect GetScreenRect(Renderer rend)
    {
        Bounds bounds = rend.bounds;

        // Get the eight corners of the bounding box.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        Vector3[] worldCorners = new Vector3[8];
        worldCorners[0] = center + new Vector3(extents.x, extents.y, extents.z);
        worldCorners[1] = center + new Vector3(-extents.x, extents.y, extents.z);
        worldCorners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
        worldCorners[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
        worldCorners[4] = center + new Vector3(extents.x, extents.y, -extents.z);
        worldCorners[5] = center + new Vector3(-extents.x, extents.y, -extents.z);
        worldCorners[6] = center + new Vector3(extents.x, -extents.y, -extents.z);
        worldCorners[7] = center + new Vector3(-extents.x, -extents.y, -extents.z);

        // Convert the corners to screen space.
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < 8; i++)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldCorners[i]);
            // Ignore the z component for screen rect calculations.
            min.x = Mathf.Min(min.x, screenPoint.x);
            min.y = Mathf.Min(min.y, screenPoint.y);
            max.x = Mathf.Max(max.x, screenPoint.x);
            max.y = Mathf.Max(max.y, screenPoint.y);
        }

        // Create the original screen-space rectangle.
        Rect screenRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

        // Shrink the rectangle by a factor (e.g., 90% of its original size).
        float shrinkFactor = 0.8f;
        Vector2 rectCenter = new Vector2(screenRect.x + screenRect.width / 2, screenRect.y + screenRect.height / 2);
        float newWidth = screenRect.width * shrinkFactor;
        float newHeight = screenRect.height * shrinkFactor;
        Rect shrunkRect = new Rect(rectCenter.x - newWidth / 2, rectCenter.y - newHeight / 2, newWidth, newHeight);

        return shrunkRect;
    }
}
