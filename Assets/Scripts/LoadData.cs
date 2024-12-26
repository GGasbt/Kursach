using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;

public static class LoadData
{
    public static string CurrentSolutionPath;
    public static bool StartPanelActive = true;
    private static string SolutionFolderPath = Application.dataPath + "\\Solution";
    
    private static FileBrowser.OnSuccess _onSuccess = paths =>
    {
        if (string.IsNullOrEmpty(paths[0]))
        {
        }
        else
        {
            CurrentSolutionPath = paths[0];
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    };
    private static FileBrowser.OnCancel _onCancel;

    public static string GetSolutionFolderPath()
    {
        if (!Directory.Exists(SolutionFolderPath))
        {
            // Создание папки, если её нет
            Directory.CreateDirectory(SolutionFolderPath);
        }
        
        return SolutionFolderPath;
    }
    
    public static bool LoadSolution()
    {
        //string path = EditorUtility.OpenFilePanel("Open solution", "", "xml");    
        // Проверяем, указан ли путь
        //FileBrowser.SetDefaultFilter( ".xml" );
        FileBrowser.SetFilters( false, new FileBrowser.Filter( "Images", ".xml") );
        return FileBrowser.ShowLoadDialog(_onSuccess, _onCancel, FileBrowser.PickMode.Files, false, "", "", "Load", "Select");
    }
    public static bool LoadSolution(string path)
    {
        // Проверяем, указан ли путь
        if (string.IsNullOrEmpty(path))
        {
            return false;
            Debug.Log("Сохранение отменено пользователем.");
        }
        else
        {
            CurrentSolutionPath = path;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
            return true;
        }
    }
}
