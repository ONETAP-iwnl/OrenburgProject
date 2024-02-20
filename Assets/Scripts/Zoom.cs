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
    List<GameObject> loadGameObjects; //объект который был подгружен при приближении
    [SerializeField] CinemachineVirtualCamera camera; 
    [SerializeField] List<AssetReferenceGameObject> assetReferences; //сслыка на объект который будет подгружен при приближении.
    Transform centerPoint; //для отслеживания камеры
    bool isZoom = false;

    

    void Start()
    {
        centerPoint = GameObject.Find("CenterPoint").transform;
        loadGameObjects = new List<GameObject>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 
        if (hit.collider != null && Input.GetMouseButtonDown(0)) //проверка на попадание по объекту кликом
        {
            if (loadGameObjects.Count != 0)
            {
                foreach (var obj in loadGameObjects)
                {
                    Addressables.ReleaseInstance(obj);
                }
                //удаление обекта из памяти
            }
            selectedObject = hit.collider.gameObject; //выделение объекта по которому произошел клик

            foreach (var reference in assetReferences)
            {
                Addressables.LoadAssetsAsync<GameObject>(reference, (gameObj) =>
                {
                    Debug.Log(gameObj);
                });

                reference.InstantiateAsync(selectedObject.transform.position, Quaternion.identity).Completed += (asyncOperation) => loadGameObjects.Add(asyncOperation.Result);
            }


            camera.Follow = selectedObject.transform; // назнчаание камере цели для преследования
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

    void LoadObjects(RaycastHit2D hit) //загрузка обьектов из памяти
    {
        
        // В данном коде используется функция addressables.loadassetsasync для асинхронной загрузки gameobject из указанного assetreference
        // После загрузки gameobject в колбэк-функции gameobj выводится в консоль с помощью debug.log



        
        // В данной задаче необходимо использовать функцию InstantiateAsync для инстанциирования объекта в указанной позиции.
        // После завершения операции инстанцирования, необходимо сохранить результат в лист loadgameobjects.
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
        }
    }
}
