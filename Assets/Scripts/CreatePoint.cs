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
    
    [SerializeField] Transform _lineObj;
    [SerializeField] GameObject _linePrefab;
    
    [SerializeField] Transform _disatanceValueGameObject;
    [SerializeField] GameObject _textPrefab;
    
    private List<Transform> _pointList = new List<Transform>();

    private bool _canCreatePoint = true;
    
    private bool isDragging = false; // Флаг для отслеживания состояния перетаскивания
    private Vector3 offset; 
    
    
    public string draggableLayer = "Draggable"; 
    private Transform selectedObject = null;   

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
            
            if (Physics.Raycast(ray, out hit) && _canCreatePoint && hit.transform.gameObject.layer != 6)
            {
                Debug.Log("fff");
                // Создаем сферу на месте удара луча
                SpawnSphere(hit.point);
            }
        }

        if (Input.GetMouseButton(0))
        {
            /*
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Блокируем выполнение Raycast
            }
            */
            if (isDragging)
            {
                selectedObject.position = GetMouseWorldPos();
                for (int i = 0; i < _pointList.Count; i++)
                {
                    if (_pointList[i].position != selectedObject.position)
                    {

                        _lineObj.GetChild(i).GetComponent<LineRenderer>()
                            .SetPosition(1, selectedObject.position + new Vector3(0, 1, 0));
                        _disatanceValueGameObject.GetChild(i).transform.position = (selectedObject.position + _pointList[i].position)/2 + new Vector3(0, 2, 0);
                        _disatanceValueGameObject.GetChild(i).GetComponent<DefinedDistance>()
                        .DefineValue(Vector3.Distance(selectedObject.position, _pointList[i].position));
                    }
                }
            }
            else
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Проверяем попадание луча в объект
                if (Physics.Raycast(ray, out hit) &&
                    hit.transform.gameObject.layer == LayerMask.NameToLayer(draggableLayer))
                {
                    isDragging = true;
                    selectedObject = hit.transform;
                    selectedObject.position = GetMouseWorldPos();
                    Debug.Log(Time.time);
                    for (int i = 0; i < _pointList.Count; i++)
                    {
                        DrawLine(_pointList[i].position, selectedObject.position);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            for (int i = 0; i < _lineObj.transform.childCount; i++)
            {
                Destroy(_lineObj.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < _disatanceValueGameObject.childCount; i++)
            {
                Destroy(_disatanceValueGameObject.GetChild(i).gameObject);
            }
        }
    }
    
    private void DrawLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject line = Instantiate(_linePrefab, _lineObj);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

        // Настраиваем точки для линии
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition + new Vector3(0, 1, 0));
        lineRenderer.SetPosition(1, endPosition + new Vector3(0, 1, 0));
        
        Vector3 center = (endPosition + new Vector3(0, 1, 0) + new Vector3(0, 1, 0) + startPosition) / 2;
        var v = Instantiate(_textPrefab, center, Quaternion.Euler(0,0,0), _disatanceValueGameObject);
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
    
    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(selectedObject.position).z; // Глубина для корректного преобразования
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
