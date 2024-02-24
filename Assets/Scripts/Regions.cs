using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Regions : MonoBehaviour
{
    public bool isLoaded;
    Camera camera;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    Plane[] cameraFrustum;
    Collider2D collider;
    List<AddresableObject> addresableObjects;
    List<GameObject> loadGameObjects;
    Zoom zoom;
    void Start()
    {
        camera = Camera.main;
        collider = GetComponent<Collider2D>();
        addresableObjects = new();
        loadGameObjects = new();
        zoom = camera.GetComponent<Zoom>();
    }

    public IEnumerator DeleteObjectOnScene()
    {
        while(virtualCamera.m_Lens.OrthographicSize == 1)
        {
            yield return null;
        }
        if (loadGameObjects.Count != 0)
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
                addresableObjects.Clear();
            }
            //удаление обекта из памяти
        }
    }
}
