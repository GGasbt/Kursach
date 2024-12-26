using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
