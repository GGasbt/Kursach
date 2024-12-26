using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;
using UnityEngine;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private GameObject _controller;
    [SerializeField] private GameObject _cellPrefab;

    private void Start()
    {
        LoadCell();
    }

    private void LoadCell()
    {
        string[] filePath = GetFilesWithExtension(LoadData.GetSolutionFolderPath(), "*.xml");
        Debug.Log(filePath.Length);
        for (int i = 0; i < filePath.Length; i++)
        {
            SaveData saveData = GetSaveDataWithPath(filePath[i]);
            Debug.Log("pp: " + saveData.ResultLink);
            InstantiateCell(saveData);
        }
    }

    private void InstantiateCell(SaveData saveData)
    {
        if (saveData.ResultLink != null && saveData.ImageLink != null)
        {
            GameObject cellObj = Instantiate(_cellPrefab, _controller.transform);
            var cell = cellObj.GetComponent<CellObj>();
            cell.DefineInfo(saveData);
        }
    }
    
    [CanBeNull]
    private SaveData GetSaveDataWithPath(string path)
    {
        if (File.Exists(path))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            using (StreamReader reader = new StreamReader(path))
            {
                return (SaveData)serializer.Deserialize(reader);
            }
        }
        else
        {
            Debug.LogError($"Файл не найден: {path}");
            return null;
        } 
    }
    
    private string[] GetFilesWithExtension(string path, string extension)
    {
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path, extension, SearchOption.AllDirectories);
        }
        else
        {
            Debug.LogError("Directory not found: " + path);
            return new string[0];
        }
    }
}
