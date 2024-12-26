using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLine : MonoBehaviour
{
    private void Start()
    {
        if(transform.parent == null)
            Destroy(gameObject);
    }
}
