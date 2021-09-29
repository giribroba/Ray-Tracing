using System;
using UnityEngine;

public class EyeScript : MonoBehaviour
{
    [SerializeField] private Vector2 resolution;
    [SerializeField] private LayerMask dontRender, lightSourceLayer;
    [SerializeField] private Transform imagePlaneTransform;
    [SerializeField] private GameObject lightSource;
    [SerializeField] private float visionLimit;
    private Texture2D finalTexture;
    private RaycastHit[,] primaryRays, shadowRays;
    //private Color[,] texColor;

    private void Awake()
    {
        finalTexture = new Texture2D((int)resolution.x, (int)resolution.y);
        primaryRays = new RaycastHit[(int)resolution.x, (int)resolution.y];
        shadowRays = new RaycastHit[(int)resolution.x, (int)resolution.y];
        //texColor = new Color[(int)resolution.x, (int)resolution.y];

        Console.WriteLine("asd");
    }

    void Update()
    {
        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                Vector3 pos = CalculatePixelPos(i, j) - this.transform.position;
                Physics.Raycast(this.transform.position, pos, out primaryRays[i, j], visionLimit, ~dontRender);

                if (primaryRays[i, j].transform != null)
                {
                    var pixelPosInTexture = primaryRays[i, j].textureCoord;
                    var renderer = primaryRays[i, j].transform.gameObject.GetComponent<Renderer>();
                    var texture = (Texture2D)renderer.material.mainTexture;
                    var color = renderer.material.GetColor("_Color") * CalculateShadowRays(i, j, primaryRays[i, j].point);
                    var tiling = renderer.material.mainTextureScale;

                    Color textColor;

                    pixelPosInTexture.x *= texture.width;
                    pixelPosInTexture.y *= texture.height;
                    //Debug.DrawRay(primaryRays[i, j].point, lightSourceTransform.position - primaryRays[i, j].point, Color.yellow);


                    if (texture == null)
                        finalTexture.SetPixel(i, j, color);
                    //texColor[i, j] = color;
                    else
                    {
                        textColor = texture.GetPixel(Mathf.FloorToInt(pixelPosInTexture.x * tiling.x), Mathf.FloorToInt(pixelPosInTexture.y * tiling.y)) * color;
                        finalTexture.SetPixel(i, j, textColor);
                    }
                }
                else
                    finalTexture.SetPixel(i, j, Color.black);
                //texColor[i, j] = Color.cyan;

                //Debug.DrawRay(this.transform.position, pos, Color.green);
            }
        }

        finalTexture.Apply();
        imagePlaneTransform.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", finalTexture);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(this.transform.position, CalculatePixelPos(0, 0) - transform.position, Color.blue);
        Debug.DrawRay(this.transform.position, CalculatePixelPos(0, resolution.y - 1) - transform.position, Color.blue);
        Debug.DrawRay(this.transform.position, CalculatePixelPos(resolution.x - 1, 0) - transform.position, Color.blue);
        Debug.DrawRay(this.transform.position, CalculatePixelPos(resolution.x - 1, resolution.y - 1) - transform.position, Color.blue);
    }

    float CalculateShadowRays(int x, int y, Vector3 hitPos)
    {
        Physics.Raycast(hitPos, lightSource.transform.position - hitPos, out shadowRays[x, y], visionLimit, ~lightSourceLayer);

        //Debug.DrawRay(hitPos, lightSourceTransform.position - hitPos, Color.yellow);
        if (shadowRays[x, y].transform == lightSource.transform && shadowRays[x, y].transform)
        {
            Debug.DrawRay(hitPos, lightSource.transform.position - hitPos, Color.yellow);
            return visionLimit * lightSource.GetComponent<LightSource>().intensity - Vector3.Distance(hitPos, lightSource.transform.position) * Vector3.Distance(hitPos, lightSource.transform.position) / visionLimit;
        }
        else
            return visionLimit * lightSource.GetComponent<LightSource>().intensity - Vector3.Distance(hitPos, lightSource.transform.position) * Vector3.Distance(hitPos, lightSource.transform.position) / visionLimit;
    }

    Vector3 CalculatePixelPos(float x, float y)
    {
        return new Vector3((x - (resolution.x / 2 - 0.5f)) * (imagePlaneTransform.localScale.x * 10 / resolution.x), ((resolution.y / 2 - 0.5f) - y) * (imagePlaneTransform.localScale.z * 10 / resolution.y), 0) + imagePlaneTransform.position;
    }
}
