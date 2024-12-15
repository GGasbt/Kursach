using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DropdownMenu : MonoBehaviour
{
    private Dropdown _dd;

    private void Start()
    {
        _dd = GetComponent<Dropdown>();
    }
    
    public void SwitchDropDownValue(int value)
    {
        switch (value)
        {
            case 1:
                LoadData.StartPanelActive = false;
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                //StartPanelController.SetActiveStartPanel(false);
                break;
            case 3:
                SaveSystem.saveResult?.Invoke();
                break;
            case 2:
                LoadData.LoadSolution();
                break;
            default:
                break;
        }

        _dd.value = 0;
    }
}
