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
    [SerializeField] List<Transform> regions; //������ ���� ��������

    

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
        loadGameObjects = new List<GameObject>();
        addresableObjects = new();
        _notZoomValue = camera.m_Lens.OrthographicSize;
        foreach(Transform reg in GameObject.Find("Regions").GetComponentInChildren<Transform>()) //���������� ���� �������� � ������
        {
            regions.Add(reg);
        }
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
        if (hit.collider != null && Input.GetMouseButtonDown(0)) //�������� �� ��������� �� ������� ������
        {
            selectedObject = hit.collider.gameObject; //��������� ������� �� �������� ��������� ����
            
            camera.Follow = selectedObject.transform; // ���������� ������ ���� ��� �������������
            isZoom = true;

            StartCoroutine(LoadVisibleObject());
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeleteObjects();
            
            camera.Follow = centerPoint;
            isZoom = false;

        }

        ZoomAction();

    }


    public float zoomValue = 1f; //����������� ������ ����� ����
    float _notZoomValue; //����������� ����������� ������

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
                
            }
            //�������� ������ �� ������
            addresableObjects.Clear();
        }
    }

    public IEnumerator LoadVisibleObject() //��������, ������� ��������� ��� ������� ������� ����� ������ ����������
    {
        while (camera.m_Lens.OrthographicSize != 1) // �������� �����������
        {
            yield return null;
        }

        if (loadGameObjects.Count != 0)//�������� ��������� ����� ������� �� ������
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
                
            }
            addresableObjects.Clear();
        }


        foreach (var region in regions)
        {
            if (region.GetComponent<Renderer>().isVisible) //��������� ���� ������� ��������
            {
                foreach (Transform addObj in region.GetComponentInChildren<Transform>()) //��������� ���� ������� � ����������� AddresableObject �� �������� ��������� ���������� �������
                {
                    if (addObj.tag == "addresablePoint")
                    {
                        addresableObjects.Add(addObj.GetComponent<AddresableObject>());
                    }
                }
            }
        }
       


        foreach (var reference in addresableObjects) //�������� �������� �� ���� �������� addresableObjects �������� �������
        {
            reference.assetReference.InstantiateAsync(reference.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObjects.Add(asyncOperation.Result);
        }
    }
}
