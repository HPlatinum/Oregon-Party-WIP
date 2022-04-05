using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedObjectRemoval : MonoBehaviour {

    public GameObject dustPrefab;

    //as soon as this script is added to an object, animate it moving and then delete it

    private void Start() {
        StartSquash();
    }

    private void StartSquash() {
        transform.DOScale(CalculateNewScale(1.2f, 0.8f, 1.2f), 0.2f).OnComplete(StartShrink);
    }

    private void StartShrink() {
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(Remove);
    }

    private void Remove() {
        GameObject dust = Instantiate(dustPrefab);
        //dust.transform.SetParent(transform.parent);
        dust.transform.position = transform.position; // positions object in hand

        Destroy(gameObject);
    }

    private Vector3 CalculateNewScale(float x, float y, float z) {
        return new Vector3(transform.localScale.x * x, transform.localScale.y * y, transform.localScale.z * z);
    }
    
}
