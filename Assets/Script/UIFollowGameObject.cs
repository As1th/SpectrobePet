using UnityEngine;
using UnityEngine.UI;

public class UIFollowGameObject : MonoBehaviour
{
    public Transform targetObject; // The GameObject to follow
    public Canvas uiCanvas; // Reference to the UI Canvas
    public float offsetDistance = 50f; // Distance from the GameObject

    private RectTransform rectTransform;
    private RectTransform canvasRect;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (uiCanvas != null)
        {
            canvasRect = uiCanvas.GetComponent<RectTransform>();
        }

        if (targetObject == null)
        {
            Debug.LogWarning("UIFollowGameObject: Target Object is not assigned!");
        }

        if (uiCanvas == null)
        {
            Debug.LogWarning("UIFollowGameObject: UI Canvas is not assigned!");
        }
    }

    void Update()
    {
        if (targetObject != null && uiCanvas != null)
        {
            UpdateUIPosition();
        }
    }

    void UpdateUIPosition()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.position);

        // Convert screen position to UI canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            uiCanvas.worldCamera,
            out Vector2 localPoint
        );

        // Get screen center in local canvas space
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenCenter,
            uiCanvas.worldCamera,
            out Vector2 centerLocalPoint
        );

        // Determine UI placement direction
        Vector2 direction = centerLocalPoint - localPoint;
        direction.Normalize();

        // Default offset (above)
        Vector2 offset = new Vector2(0, offsetDistance);

        // Adjust position based on screen location
        if (localPoint.x > centerLocalPoint.x) offset.x = -offsetDistance; // Move UI left if object is on the right
        if (localPoint.y > centerLocalPoint.y) offset.y = -offsetDistance-200; // Move UI below if object is at the top

        rectTransform.anchoredPosition = localPoint + offset;
    }
}
