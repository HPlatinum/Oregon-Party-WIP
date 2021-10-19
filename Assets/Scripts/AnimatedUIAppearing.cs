using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedUIAppearing : MonoBehaviour {

    private Vector3 originalScale;
    private float growTime = 0.2f;
    private float shrinkTime = 0.2f;

    //as soon as this script is added to an object:
    //set the object's size to zero
    //an external script will call StartGrowth when the object should start expanding
    //after the object is full size, delete this script from it

    private void Start() {
        SetOriginalScale();
        StartShrunk();
    }

    private void StartShrunk() {
        transform.localScale = Vector3.zero;
    }

    public void StartGrowth() {
        
        transform.DOScale(CalculateNewScale(1.2f, 1.2f, 1.2f), growTime).OnComplete(StartScalingTo1);
    }

    private void StartScalingTo1() {
        transform.DOScale(originalScale, shrinkTime).OnComplete(DeleteThisCompontent);
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
    
    public float GetTotalAnimationTime() {
        return growTime + shrinkTime;
    }
    
}
