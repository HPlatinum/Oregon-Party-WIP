using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedObjectRemoval : MonoBehaviour {

    //as soon as this script is added to an object, animate it moving and then delete it

    private void Start() {
        StartSquash();
    }

    private void StartSquash() {
        transform.DOScale(new Vector3(transform.localScale.x * 1.2f, transform.localScale.y * 0.8f, transform.localScale.z * 1.2f), 0.2f).OnComplete(StartShrink);
    }

    private void StartShrink() {
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(Remove);
    }

    private void Remove() {
        Destroy(gameObject);
    }
    
}
