using UnityEngine;
using System.Collections;

public class AutoScaleAndDestroy : MonoBehaviour
{
    public float lifetime = 5f;      // Time before destruction
    public float scaleDuration = 1f; // Time taken to scale up/down

    private Vector3 targetScale;     // The final scale of the object

    void Start()
    {
        targetScale = transform.localScale; // Store the intended final scale
        transform.localScale = Vector3.zero; // Start at zero scale

        StartCoroutine(ScaleOverTime(Vector3.zero, targetScale, scaleDuration)); // Scale up
        StartCoroutine(DestroyAfterTime()); // Start destruction countdown
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime - scaleDuration); // Wait before scaling down
        yield return StartCoroutine(ScaleOverTime(transform.localScale, Vector3.zero, scaleDuration)); // Scale down
        Destroy(gameObject); // Destroy object
    }

    IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = to; // Ensure final scale is set
    }
}
