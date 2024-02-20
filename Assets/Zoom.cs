using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Cinemachine;

public class Zoom : MonoBehaviour
{
    GameObject selectedObject;
    [SerializeField] CinemachineVirtualCamera camera;
    [SerializeField] AssetReferenceGameObject assetReference;
    Transform centerPoint;
    bool isZoom = false;

    GameObject loadGameObject;

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
    }

    private void OnLoadCompleted(AsyncOperationHandle<GameObject> handle)
    {


        if (handle.Status == AsyncOperationStatus.Succeeded)
        {

            loadGameObject = Instantiate(handle.Result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && Input.GetMouseButtonDown(0))
        {
            if(loadGameObject != null)
            {
                Addressables.ReleaseInstance(loadGameObject);
            }
            selectedObject = hit.collider.gameObject;

            Addressables.LoadAssetsAsync<GameObject>(assetReference, (gameObj) =>
            {
                Debug.Log(gameObj);
            });

            camera.Follow = selectedObject.transform;
            

            assetReference.InstantiateAsync(selectedObject.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObject = asyncOperation.Result;
            isZoom = true;
        }



        if (Input.GetMouseButtonDown(1))
        {
            if (loadGameObject != null)
            {
                Addressables.ReleaseInstance(loadGameObject);
            }
            camera.Follow = centerPoint;
            isZoom = false;

        }
        if (isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, 1, 8 * Time.deltaTime);
        }
        else if (!isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, 5, 8 * Time.deltaTime);
        }
    }

}
