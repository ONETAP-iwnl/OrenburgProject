using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class DataHandler : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectData  // Структура данных для хранения информации об объекте
    {
        public string gameObjectName;
        public string titleText;
        public string mainText;
        public string additionalText;
    }


    // Ссылки на текстовые поля, в которых будет отображаться информация об объекте
    public Text titleText;
    public Text mainText;
    public Text additionalText;

    

    // Словарь для хранения информации об объектах по их именам
    private Dictionary<string, GameObjectData> gameObjectDataMap = new Dictionary<string, GameObjectData>();

    [System.Serializable]
    public class GameObjectDataArray
    {
        public GameObjectData[] items; // Обертка для массива объектов GameObjectData
    }

    // Метод для загрузки данных из JSON-файла
    public void LoadDataFromJson(string jsonFileName)
    {
        // Формирование пути к JSON-файлу
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            // Чтение JSON-файла
            string json = File.ReadAllText(filePath);
            // Десериализация JSON в объект GameObjectDataArray
            GameObjectDataArray wrapper = JsonUtility.FromJson<GameObjectDataArray>(json);

            // Добавление данных об объектах в словарь
            foreach (var gameObjectData in wrapper.items)
            {
                gameObjectDataMap[gameObjectData.gameObjectName] = gameObjectData;
            }

            // Вывод отладочного сообщения об успешной загрузке данных
            Debug.Log("Data loaded successfully. Number of items: " + gameObjectDataMap.Count);
        }
        else
        {
            // Вывод сообщения об ошибке, если файл не найден
            Debug.LogError("File not found: " + filePath);
        }
    }

    

    public void UpdateUI(string gameObjectName) // Метод для обновления UI информацией об объекте
    {
        if (gameObjectDataMap.ContainsKey(gameObjectName))
        {
            // Если информация об объекте найдена в словаре, обновляем текстовые поля UI
            GameObjectData data = gameObjectDataMap[gameObjectName];
            titleText.text = data.titleText;
            mainText.text = data.mainText;
            additionalText.text = data.additionalText;
        }
        else
        {
            Debug.LogError("Data not found for GameObject: " + gameObjectName);
        }
    }

    public void ClearUI()
    {
        titleText.text = "";
        mainText.text = "";
        additionalText.text = "";
    }
}
