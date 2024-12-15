using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreatePoint : MonoBehaviour
{
    [SerializeField] GameObject spherePrefab;
    [SerializeField] ParticleSystem _effectPrefab;
    private List<Transform> _pointList = new List<Transform>();

    private bool _canCreatePoint = true;

    private void Start()
    {
        if (LoadData.CurrentSolutionPath == null) return;
        
        List<Transform> list = new List<Transform>();
        List<PointData> pointData = GetInputDataWithPath<PointData>(LoadData.CurrentSolutionPath);
        
        for (int i = 0; i < pointData.Count; i++)
        {
            for(int j = 0; j < pointData.Count; j++)
                if(i == j)
                    SpawnSphere(new Vector3(pointData[j].X, 0, pointData[j].Y));
        }
        
        LoadData.CurrentSolutionPath = null;
    }

    void Update()
    {
        // Проверяем нажатие левой кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {
        
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Блокируем выполнение Raycast
            }
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем попадание луча в объект
            if (Physics.Raycast(ray, out hit) && _canCreatePoint)
            {
                // Создаем сферу на месте удара луча
                SpawnSphere(hit.point);
            }
        }
    }

    public void GetPointList()
    {
        if(_pointList.Count <= 1) return;
        
        _canCreatePoint = false;
        Debug.Log("Point list: " + _pointList.Count);
        Littl.StartAlgoritm?.Invoke(_pointList);
    }

    private void SpawnSphere(Vector3 position)
    {
        // Создаем сферу и задаем ее размер
        GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
        if (_pointList.Count == 0)
            sphere.GetComponent<Renderer>().material.color = Color.red;
        _pointList.Add(sphere.transform);
        sphere.GetComponent<DeterminNumber>().DetermineNumber(_pointList.Count);
        if (_effectPrefab != null)
        {
            var effect = Instantiate(_effectPrefab, sphere.transform.position, Quaternion.Euler(-90, 0, 0));
            StartCoroutine(DestroyAfterTime(effect.gameObject));
        }
    }
    
    [CanBeNull]
    private List<T> GetInputDataWithPath<T>(string path)
    {
        if (File.Exists(path))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (StreamReader reader = new StreamReader(path))
            {
                return (List<T>)serializer.Deserialize(reader);
            }
        }
        else
        {
            Debug.LogError($"Файл не найден: {path}");
            return null;
        } 
    }

    private IEnumerator DestroyAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(obj);
    }
}
