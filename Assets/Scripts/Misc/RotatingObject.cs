using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float RotationAmountPerFPS = 0.05f;

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, RotationAmountPerFPS, 0f), Space.World);
    }
}
