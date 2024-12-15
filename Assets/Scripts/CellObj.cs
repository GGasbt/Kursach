using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class CellObj : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI name;
    [SerializeField] public TextMeshProUGUI pointCount;
    [SerializeField] public Image image;
    private string result;
    private SaveData _saveData;
    
    public void DefineInfo(SaveData saveData)
    {
        _saveData = saveData;
        name.text += GetSaveDataName(saveData.ResultLink);
        pointCount.text += GetSaveDataPointCount(saveData.ResultLink);
        result = saveData.ResultLink;
        GetCellImage(saveData.ImageLink);
    }
    
    public void OpenFileInNotepad()
    {
        if (System.IO.File.Exists(result))
        {
            Process.Start("notepad.exe", result);
        }
        else
        {
            UnityEngine.Debug.LogError("Файл не найден: " + result);
        }
    }

    private string GetSaveDataName(string path)
    {
        string p = path.Remove(0, path.LastIndexOf('/') + 1);
        return p.Remove(p.LastIndexOf('.'), 4);
    }

    private string GetSaveDataPointCount(string filePath)
    {
        if (File.Exists(filePath))
        {
            int lineCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }
            return (lineCount-1).ToString();
        }
        else
        {
            return "0";
        }
    }

    private void GetCellImage(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(imageData)) // Загружаем изображение из байтов
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.sprite = newSprite;
            }
        }
    }

    public void OpenProject()
    {
        if (LoadData.LoadSolution(_saveData.InputDataLink))
        {
            LoadData.StartPanelActive = false;
        } 
    }
}
