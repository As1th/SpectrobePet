using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second
    public int axis = 0; // 0 = X, 1 = Y, 2 = Z

    void Update()
    {
        Vector3 rotationVector = Vector3.zero;

        switch (axis)
        {
            case 0: // X-axis
                rotationVector = new Vector3(rotationSpeed * Time.deltaTime, 0, 0);
                break;
            case 1: // Y-axis
                rotationVector = new Vector3(0, rotationSpeed * Time.deltaTime, 0);
                break;
            case 2: // Z-axis
                rotationVector = new Vector3(0, 0, rotationSpeed * Time.deltaTime);
                break;
        }

        transform.Rotate(rotationVector);
    }
}
