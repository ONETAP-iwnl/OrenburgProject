using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class DataHandler : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectData  // ��������� ������ ��� �������� ���������� �� �������
    {
        public string gameObjectName;
        public string titleText;
        public string mainText;
        public string additionalText;
    }


    // ������ �� ��������� ����, � ������� ����� ������������ ���������� �� �������
    public Text titleText;
    public Text mainText;
    public Text additionalText;

    

    // ������� ��� �������� ���������� �� �������� �� �� ������
    private Dictionary<string, GameObjectData> gameObjectDataMap = new Dictionary<string, GameObjectData>();

    [System.Serializable]
    public class GameObjectDataArray
    {
        public GameObjectData[] items; // ������� ��� ������� �������� GameObjectData
    }

    // ����� ��� �������� ������ �� JSON-�����
    public void LoadDataFromJson(string jsonFileName)
    {
        // ������������ ���� � JSON-�����
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            // ������ JSON-�����
            string json = File.ReadAllText(filePath);
            // �������������� JSON � ������ GameObjectDataArray
            GameObjectDataArray wrapper = JsonUtility.FromJson<GameObjectDataArray>(json);

            // ���������� ������ �� �������� � �������
            foreach (var gameObjectData in wrapper.items)
            {
                gameObjectDataMap[gameObjectData.gameObjectName] = gameObjectData;
            }

            // ����� ����������� ��������� �� �������� �������� ������
            Debug.Log("Data loaded successfully. Number of items: " + gameObjectDataMap.Count);
        }
        else
        {
            // ����� ��������� �� ������, ���� ���� �� ������
            Debug.LogError("File not found: " + filePath);
        }
    }

    

    public void UpdateUI(string gameObjectName) // ����� ��� ���������� UI ����������� �� �������
    {
        if (gameObjectDataMap.ContainsKey(gameObjectName))
        {
            // ���� ���������� �� ������� ������� � �������, ��������� ��������� ���� UI
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
