using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using SimpleFileBrowser;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject _errorPanel;
    private SaveData _saveData;
    
    private List<Transform> _points;
    private List<int> _bestPath;

    public static Action saveResult;
    
    private FileBrowser.OnSuccess _onSuccess;
    private FileBrowser.OnCancel _onCancel;

    private string _path;
    
    private void OnEnable()
    {
        Littl.StartAlgoritm += list => _points = list;
        Littl.FinishAlgoritm += ints => _bestPath = ints;
        
        saveResult += SaveResult;
    }

    private void Start()
    {
        _onSuccess = paths =>
        {
            File.WriteAllText(paths[0], SaveTxtResult());
            _saveData.ResultLink = paths[0];
            Debug.Log(paths[0]);
            
            CreateScreenshot();
            SaveInputData();
            SaveXmlData(_saveData, $"{LoadData.GetSolutionFolderPath()}\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}-Solution.xml");
        };
    }

    private string GetPath(string[] pathList)
    {
        return pathList[0];
    }

    private void OnDisable()
    {
        saveResult -= SaveResult;
    }
    
    private void SaveResult()
    {
        if (_bestPath == null)
        {
            _errorPanel.SetActive(true);
            return;
        }
         _saveData = new SaveData();
         SaveDecisionFile();
    }

    private void SaveInputData()
    {
        //File.WriteAllText($"{saveData.ResultLink.Remove(saveData.ResultLink.Length - 5)}-InputData.json", saveData.Points);
        //InputData data = new InputData();
        List<PointData> pointList = new List<PointData>();
        for (int i = 0; i < _points.Count; i++)
        {
            PointData point = new PointData(i, (float)Math.Round(_points[_bestPath[i]].position.x, 2), (float)Math.Round(_points[_bestPath[i]].position.z, 2));
            
            pointList.Add(point);
        }
        
        _saveData.InputDataLink = SaveXmlData(pointList);
    }
    
    private string SaveDecisionFile()
    {
        FileBrowser.SetFilters( false, new FileBrowser.Filter( "Solution", ".txt") );
        FileBrowser.ShowSaveDialog(_onSuccess, _onCancel, FileBrowser.PickMode.Files, false, "", "solution.txt", "Save", "Save");
        return _path;
    }

    private void CreateScreenshot()
    {
        
        string link = _saveData.ResultLink.Remove(_saveData.ResultLink.Length - 4);
         
        //Создание скриншота
        string screenshotPath = $"{link}.png";
        _saveData.ImageLink = screenshotPath;
        StartCoroutine(TakeScreenshot(screenshotPath));
    }
    
    private IEnumerator TakeScreenshot(string screenshotName)
    {
        // Отключаем слой UI
        
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            canvas.enabled = false;
        }
        

        // Ждём кадр, чтобы UI успел отключиться
        yield return new WaitForEndOfFrame();
        
        ScreenCapture.CaptureScreenshot(screenshotName, 1); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
        Debug.Log(screenshotName);

        // Включаем UI обратно
        
        foreach (var canvas in canvases)
        {
            if (canvas != null) canvas.enabled = true;
        }
        
    }

    private string SaveTxtResult()
    {
        string str = "Оптимальный путь составляет: \n";

        for (int i = 0; i < _points.Count; i++)
        {
            str += $"{_bestPath[i]} с координатами: ({Math.Round(_points[_bestPath[i]].position.x, 2)};{Math.Round(_points[_bestPath[i]].position.z, 2)})\n";
        }
        
        return str;
    }

    private string SaveXmlData<T>(T data, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, data);
        }
        return filePath;
    }

    private string SaveXmlData<T>(List<T> list)
    {
        string filePath = _saveData.ResultLink.Remove(_saveData.ResultLink.Length - 4) + "-InputData.xml";
        
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, list);
        }
        return filePath;
    }
}

[Serializable]
public class SaveData
{
    public string InputDataLink {get; set;}
    public string ResultLink{ get; set; }
    public string ImageLink{ get; set; }
}

[Serializable]
public class InputData
{
    public List<PointData> Points { get; set; }
}

[Serializable]
public class PointData
{
    public int Number { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public PointData() { }
    
    public PointData(int number, float x, float y)
    {
        Number = number;
        X = x;
        Y = y;
    }
}
