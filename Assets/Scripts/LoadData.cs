using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadData
{
    public static string CurrentSolutionPath;
    public static bool StartPanelActive = true;
    private static string SolutionFolderPath = Application.dataPath + "\\Solution";

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
        string path = EditorUtility.OpenFilePanel("Open solution", "", "xml");    

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
