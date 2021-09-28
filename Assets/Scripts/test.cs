using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Vector2 resolution;
    [SerializeField] private LayerMask imagePlaneLayer;
    [SerializeField] private Transform planeTransform;
    [SerializeField] private float visionLimit;
    private RaycastHit[,] primaryRays;

    private void Awake()
    {
        primaryRays = new RaycastHit[(int)resolution.x, (int)resolution.y];
    }

    void Update()
    {
        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                Vector3 pos = CalculatePixelPos(i, j) - this.transform.position;
                Physics.Raycast(this.transform.position, pos, out primaryRays[i,j], visionLimit, ~imagePlaneLayer);
                Debug.DrawRay(this.transform.position, pos, Color.green);
            }
        }

        foreach (var ray in primaryRays)
        {
            if (ray.transform != null)
            {
                print(ray.transform.name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(this.transform.position, planeTransform.position - transform.position, Color.blue);
    }

    Vector3 CalculatePixelPos(float x, float y)
    {
        return new Vector3((x - (resolution.x / 2 - 0.5f)) * (planeTransform.localScale.x * 10 / resolution.x), ((resolution.y / 2 - 0.5f) - y) * (planeTransform.localScale.z * 10 / resolution.y), 0) + planeTransform.position;
    }
}
