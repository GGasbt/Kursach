using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPanelController : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private GameObject _errorPanel;
    
    private void Start()
    {
        if (LoadData.CurrentSolutionPath != null || !LoadData.StartPanelActive)
        {
            LoadData.StartPanelActive = false;
            startPanel.SetActive(false);   
        }
    }
    
    public void CreateNewSolution()
    {
        if(startPanel.activeInHierarchy)
            startPanel.SetActive(false);
    }

    public void OpenSolution()
    {
        if (LoadData.LoadSolution())
        {
            LoadData.StartPanelActive = false;
            startPanel.SetActive(false);
        }
    }

    public void OpenStartPanel()
    {
        LoadData.StartPanelActive = true;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void OpenInfoPanel()
    {
        _infoPanel.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        _infoPanel.SetActive(false);
    }

    public void CloseErrorPanel()
    {
        _errorPanel.SetActive(false);
    }
}
