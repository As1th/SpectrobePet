using UnityEngine;

public class RotateAroundY : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
