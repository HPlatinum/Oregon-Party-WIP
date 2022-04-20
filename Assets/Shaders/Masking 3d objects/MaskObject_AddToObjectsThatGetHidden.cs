using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject_AddToObjectsThatGetHidden : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().material.renderQueue = 3002;
    }
    
}
