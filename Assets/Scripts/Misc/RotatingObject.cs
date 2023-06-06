using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float RotationAmount = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, RotationAmount, 0f), Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}