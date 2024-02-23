using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Cinemachine;
using Unity.Burst.CompilerServices;

public class Zoom : MonoBehaviour
{
    GameObject selectedObject; //объект на который кликнул игрок
    List<AddresableObject> addresableObjects;
    List<GameObject> loadGameObjects; //объект который был подгружен при приближении
    [SerializeField] CinemachineVirtualCamera camera;
    Transform centerPoint; //для отслеживания камеры
    bool isZoom = false;
    [SerializeField] List<Transform> regions; //список всех регионов

    

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
        loadGameObjects = new List<GameObject>();
        addresableObjects = new();
        _notZoomValue = camera.m_Lens.OrthographicSize;
        foreach(Transform reg in GameObject.Find("Regions").GetComponentInChildren<Transform>()) //добавление всех регионов в массив
        {
            regions.Add(reg);
        }
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
        if (hit.collider != null && Input.GetMouseButtonDown(0)) //проверка на попадание по объекту кликом
        {
            selectedObject = hit.collider.gameObject; //выделение объекта по которому произошел клик
            
            camera.Follow = selectedObject.transform; // назнчаание камере цели для преследования
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


    public float zoomValue = 1f; //криближение камеры после зума
    float _notZoomValue; //изначальное приближение камеры

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

    void LoadObjects(RaycastHit2D hit) //загрузка обьектов из памяти
    {
        if (loadGameObjects.Count != 0)//удаление обекта из памяти
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
            }

        }

        

        foreach (Transform addObj in selectedObject.GetComponentInChildren<Transform>()) //получение всех обектов с компонентом AddresableObject из дочерних элементов выделеного объекта
        {
            if (addObj.tag == "addresablePoint")
            {
                addresableObjects.Add(addObj.GetComponent<AddresableObject>());
            }
        }


        foreach (var reference in addresableObjects) //загрузка префабов из всех дочерних addresableObjects выделеного объекта
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
            //удаление обекта из памяти
            addresableObjects.Clear();
        }
    }

    public IEnumerator LoadVisibleObject() //куратина, которая загружает все видимые объекты когда камера приближена
    {
        while (camera.m_Lens.OrthographicSize != 1) // ожидание приближения
        {
            yield return null;
        }

        if (loadGameObjects.Count != 0)//удаление созданных ранее обектов из памяти
        {
            foreach (var obj in loadGameObjects)
            {
                Addressables.ReleaseInstance(obj);
                
            }
            addresableObjects.Clear();
        }


        foreach (var region in regions)
        {
            if (region.GetComponent<Renderer>().isVisible) //получение всех видимых регионов
            {
                foreach (Transform addObj in region.GetComponentInChildren<Transform>()) //получение всех обектов с компонентом AddresableObject из дочерних элементов выделеного объекта
                {
                    if (addObj.tag == "addresablePoint")
                    {
                        addresableObjects.Add(addObj.GetComponent<AddresableObject>());
                    }
                }
            }
        }
       


        foreach (var reference in addresableObjects) //загрузка префабов из всех дочерних addresableObjects видымого региона
        {
            reference.assetReference.InstantiateAsync(reference.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObjects.Add(asyncOperation.Result);
        }
    }
}
