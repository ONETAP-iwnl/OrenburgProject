using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Camera camera;
    SpriteRenderer renderer;
    Plane[] cameraFrustum;
    Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = collider.bounds;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds)) 
        { 
            renderer.sharedMaterial.color = Color.green;
        }
        else
        {
            renderer.sharedMaterial.color = Color.red;
        }
    }
}
