using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [Range(0, 100)] public float rotationSpeed;
    void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * 10);
    }
}
