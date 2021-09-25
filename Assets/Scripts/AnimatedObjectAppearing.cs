using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedObjectAppearing : MonoBehaviour {

    private Vector3 originalScale;

    //as soon as this script is added to an object, animate it growing and then delete this script from the object

    private void Start() {
        SetOriginalScale();
        StartGrowth();
    }

    private void StartGrowth() {
        transform.localScale = Vector3.zero;
        transform.DOScale(CalculateNewScale(0.6f, 1.4f, 0.6f), 0.2f).OnComplete(StartScalingTo1);
    }

    private void StartScalingTo1() {
        transform.DOScale(originalScale, 0.2f).OnComplete(DeleteThisCompontent);
    }

    private void DeleteThisCompontent() {
        Destroy(this);
    }

    private Vector3 CalculateNewScale(float x, float y, float z) {
        return new Vector3(originalScale.x * x, originalScale.y * y, originalScale.z * z);
    }

    private void SetOriginalScale() {
        originalScale = transform.localScale;
    }
    
}
