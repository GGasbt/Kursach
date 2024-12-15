using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeterminNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text _number;

    public void DetermineNumber(int number)
    {
        _number.text = number.ToString();
    }
    
}
