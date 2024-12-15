using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefinedDistance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI number;

    public void DefineValue(float value)
    {
        number.text = Math.Round(value, 2).ToString();
    }
}
