using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RotateFish : MonoBehaviour {

    public float rotationSpeed = 50f;

    public void Start(){
        transform.Rotate(0, 0, Random.Range(0, 360));
    }
    private void Update() {
        transform.Rotate(0, 0, -(rotationSpeed * Time.unscaledDeltaTime));
    }

}
