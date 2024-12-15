using System;
using System.Collections.Generic;
using System.IO;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class Littl : MonoBehaviour
{
    [SerializeField] private GameObject _linePrefab;
    private float[,] distanceMatrix;
    private int numPoints;
    private float bestCost = float.MaxValue;
    private List<int> bestPath;

    // Список точек типа Transform
    public List<Transform> points;

    public static Action<List<Transform>> StartAlgoritm;
    public static Action<List<int>> FinishAlgoritm;


    private void OnEnable()
    {
        StartAlgoritm += delegate(List<Transform> list) {
            points = list;
            //SaveSystem.saveData.Points = ConvertListInArray(points);
            StartExecution();
        };
    }

    private async void StartExecution()
    {
        numPoints = points.Count;
        distanceMatrix = new float[numPoints, numPoints];
        bestPath = new List<int>();

        // Заполнение матрицы расстояний
        for (int i = 0; i < numPoints; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                if (i != j)
                {
                    distanceMatrix[i, j] = Vector3.Distance(points[i].position, points[j].position);
                }
                else
                {
                    distanceMatrix[i, j] = float.MaxValue; // Исключение путей из точки в саму себя
                }
            }
        }

        await Solve();
        DrawPath();
        FinishAlgoritm?.Invoke(bestPath);
    }

    public Task Solve()
    {
        List<int> initialPath = new List<int> { 0 };
        BranchAndBound(initialPath, 0);
        return Task.CompletedTask;
    }

    private void BranchAndBound(List<int> path, float currentCost)
    {
        if (path.Count == numPoints)
        {
            // Замыкаем цикл (возвращаемся в начальную точку)
            currentCost += distanceMatrix[path[^1], path[0]];
            if (currentCost < bestCost)
            {
                bestCost = currentCost;
                bestPath = new List<int>(path);
            }
            return;
        }

        // Перебор возможных точек
        for (int i = 0; i < numPoints; i++) 
        {
            if (!path.Contains(i))
            {
                int last = path[^1];
                float newCost = currentCost + distanceMatrix[last, i];

                if (newCost < bestCost)
                {
                    path.Add(i);
                    BranchAndBound(path, newCost);
                    path.RemoveAt(path.Count - 1);
                }
            }
        }
    }
    
    private void DrawPath()
    {
        // Добавляем последний переход для замыкания маршрута
        try
        {
            for (int i = 0; i < Math.Max(bestPath.Count - 1, 0); i++)
            {
                int startIdx = Math.Abs(bestPath[i] % bestPath.Count);
                int endIdx = Math.Abs(bestPath[i + 1] % bestPath.Count);

                // Создаем линию между двумя точками
                DrawLine(points[startIdx].position, points[endIdx].position);
            }

            DrawLine(points[0].position, points[bestPath[^1]].position);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void DrawLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject line = Instantiate(_linePrefab);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

        // Настраиваем точки для линии
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition + new Vector3(0, 1, 0));
        lineRenderer.SetPosition(1, endPosition + new Vector3(0, 1, 0));
    }
    
    public string PrintBestPath()
    {
        string pathStr = "Оптимальный путь: ";
        foreach (var point in bestPath)
        {
            pathStr += point+1 + " ";
        }
        Debug.Log(pathStr + $"\nМинимальная стоимость пути: {bestCost:F2}");
        
        
        
        return pathStr;
    }
}
