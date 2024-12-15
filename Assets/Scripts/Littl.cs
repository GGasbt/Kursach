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
            SolveTSP();
        };
    }

    public void SolveTSP()
    {
        ComputeDistanceMatrix();
        bestPath = SolveLittleAlgorithm(distanceMatrix);
        DrawPath();
        FinishAlgoritm?.Invoke(bestPath);
    }

    void ComputeDistanceMatrix()
    {
        int n = points.Count;
        distanceMatrix = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    distanceMatrix[i, j] = int.MaxValue; // Исключаем путь из точки в саму себя
                }
                else
                {
                    distanceMatrix[i, j] = Mathf.RoundToInt(Vector3.Distance(points[i].position, points[j].position));
                }
            }
        }
    }

    List<int> SolveLittleAlgorithm(float[,] matrix)
    {
        int n = matrix.GetLength(0);
        float minCost = int.MaxValue;
        List<int> bestPath = new List<int>();

        LittleAlgorithm(matrix, new List<int> { 0 }, 0, 0, ref minCost, ref bestPath);

        return bestPath;
    }

    void LittleAlgorithm(float[,] matrix, List<int> currentPath, float currentCost, int level, ref float minCost, ref List<int> bestPath)
    {
        if (level == matrix.GetLength(0) - 1)
        {
            float returnCost = matrix[currentPath[level], 0];
            float totalCost = currentCost + returnCost;

            if (totalCost < minCost)
            {
                minCost = totalCost;
                bestPath.Clear();
                bestPath.AddRange(currentPath);
                bestPath.Add(0);
            }
            return;
        }

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            if (!currentPath.Contains(i))
            {
                currentPath.Add(i);
                LittleAlgorithm(matrix, currentPath, currentCost + matrix[currentPath[level], i], level + 1, ref minCost, ref bestPath);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }

    string FormatPath(List<int> path)
    {
        List<string> formatted = new List<string>();
        foreach (int index in path)
        {
            formatted.Add(points[index].name);
        }
        return string.Join(" -> ", formatted);
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
}
