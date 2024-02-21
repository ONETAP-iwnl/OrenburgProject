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
    List<AddresableObject> addresableObjects;
    List<GameObject> loadGameObjects; //������ ������� ��� ��������� ��� �����������
    [SerializeField] CinemachineVirtualCamera camera; 
    Transform centerPoint; //��� ������������ ������
    bool isZoom = false;

    

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
        loadGameObjects = new List<GameObject>();
        addresableObjects = new();
        _notZoomValue = camera.m_Lens.OrthographicSize;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
        if (hit.collider != null && Input.GetMouseButtonDown(0)) //�������� �� ��������� �� ������� ������
        {

            LoadObjects(hit);

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

    [SerializeField] float zoomValue = 1f; //
    float _notZoomValue; 

    void ZoomAction()
    {
        if (isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, zoomValue, 8 * Time.deltaTime);
        }
        else if (!isZoom)
        {
            camera.m_Lens.OrthographicSize = Mathf.MoveTowards(camera.m_Lens.OrthographicSize, _notZoomValue, 8 * Time.deltaTime);
        }
    }

    void LoadObjects(RaycastHit2D hit) //�������� �������� �� ������
    {
        if (loadGameObjects.Count != 0)//�������� ������ �� ������
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
            }

        }

        selectedObject = hit.collider.gameObject; //��������� ������� �� �������� ��������� ����

        foreach (Transform addObj in selectedObject.GetComponentInChildren<Transform>()) //��������� ���� ������� � ����������� AddresableObject �� �������� ��������� ���������� �������
        {
            if (addObj.tag == "addresablePoint")
            {
                addresableObjects.Add(addObj.GetComponent<AddresableObject>());
            }
        }


        foreach (var reference in addresableObjects) //�������� �������� �� ���� �������� addresableObjects ���������� �������
        {
            reference.assetReference.InstantiateAsync(reference.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObjects.Add(asyncOperation.Result);
        }



    }

    void DeleteObjects()
    {
        if (loadGameObjects.Count != 0)
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
                addresableObjects.Clear();
            }
            //�������� ������ �� ������
        }
    }
}
