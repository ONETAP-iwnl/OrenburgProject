using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Cinemachine;
using Unity.Burst.CompilerServices;

public class Zoom : MonoBehaviour
{
    GameObject selectedObject; //������ �� ������� ������� �����
    List<GameObject> loadGameObjects; //������ ������� ��� ��������� ��� �����������
    [SerializeField] CinemachineVirtualCamera camera; 
    [SerializeField] List<AssetReferenceGameObject> assetReferences; //������ �� ������ ������� ����� ��������� ��� �����������.
    Transform centerPoint; //��� ������������ ������
    bool isZoom = false;

    

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
        loadGameObjects = new List<GameObject>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
        if (hit.collider != null && Input.GetMouseButtonDown(0)) //�������� �� ��������� �� ������� ������
        {
            if (loadGameObjects.Count != 0)
            {
                foreach (var obj in loadGameObjects)
                {
                    Addressables.ReleaseInstance(obj);
                }
                //�������� ������ �� ������
            }
            selectedObject = hit.collider.gameObject; //��������� ������� �� �������� ��������� ����

            foreach (var reference in assetReferences)
            {
                Addressables.LoadAssetsAsync<GameObject>(reference, (gameObj) =>
                {
                    Debug.Log(gameObj);
                });

                reference.InstantiateAsync(selectedObject.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObjects.Add(asyncOperation.Result);
            }


            camera.Follow = selectedObject.transform; // ���������� ������ ���� ��� �������������
            isZoom = true;
        }



        if (Input.GetMouseButtonDown(1))
        {
            DeleteObjects();
            
            camera.Follow = centerPoint;
            isZoom = false;

        }

        ZoomAction();
    }

    void ZoomAction()
    {
        if (isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, 1, 8 * Time.deltaTime);
        }
        else if (!isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, 5, 8 * Time.deltaTime);
        }
    }

    void LoadObjects(RaycastHit2D hit) //�������� �������� �� ������
    {
        
        // � ������ ���� ������������ ������� addressables.loadassetsasync ��� ����������� �������� gameobject �� ���������� assetreference
        // ����� �������� gameobject � ������-������� gameobj ��������� � ������� � ������� debug.log



        
        // � ������ ������ ���������� ������������ ������� InstantiateAsync ��� ���������������� ������� � ��������� �������.
        // ����� ���������� �������� ���������������, ���������� ��������� ��������� � ���� loadgameobjects.
    }

    void DeleteObjects()
    {
        if (loadGameObjects.Count != 0)
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
            }
            //�������� ������ �� ������
        }
    }
}
