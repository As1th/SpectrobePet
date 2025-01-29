using UnityEngine;

public class UIFollowGameObject : MonoBehaviour
{
    public Transform targetObject; // The GameObject to follow
    public float verticalOffset = 6f; // Vertical offset distance
    public float horizontalOffset = 3f; // Horizontal offset distance
    public float lerpSpeed = 5f; // Speed of lerping (adjust for smoother or faster transitions)

    private Vector3 currentOffset;

    private void Update()
    {
        if (targetObject != null)
        {
            UpdateUIPosition();
        }
    }

    void UpdateUIPosition()
    {
        Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(targetObject.position);

        // Check if the object is in front of the camera
        if (objectScreenPos.z < 0) return;

        // Convert screen position back to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(objectScreenPos.x, objectScreenPos.y, objectScreenPos.z));

        // Get screen center and screen bounds
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenCenterX = screenWidth / 2f;
        float screenCenterY = screenHeight / 2f;

        // Initialize offsets
        Vector3 offset = Vector3.zero;

        // Adjust horizontal offset
        if (objectScreenPos.x > screenCenterX)
        {
            // On right side -> move UI slightly to the left
            float rightEdgeFactor = Mathf.InverseLerp(screenCenterX, screenWidth, objectScreenPos.x);
            offset.x = -horizontalOffset * Mathf.Lerp(0.5f, 1.0f, rightEdgeFactor);
        }
        else
        {
            // On left side -> move UI slightly to the right
            float leftEdgeFactor = Mathf.InverseLerp(0, screenCenterX, objectScreenPos.x);
            offset.x = horizontalOffset * Mathf.Lerp(1.0f, 0.5f, leftEdgeFactor);
        }

        // Adjust vertical offset
        if (objectScreenPos.y > screenCenterY)
        {
            // On top side -> move UI downward
            float topEdgeFactor = Mathf.InverseLerp(screenCenterY, screenHeight, objectScreenPos.y);
            offset.y = -verticalOffset * Mathf.Lerp(0.5f, 1.0f, topEdgeFactor);
        }
        else
        {
            // On bottom side -> move UI upward
            offset.y = verticalOffset;
        }

        // Lerp the current position towards the new offset position
        currentOffset = Vector3.Lerp(currentOffset, offset, Time.deltaTime * lerpSpeed);

        // Apply the lerped position
        transform.position = worldPos + currentOffset;
    }
}
