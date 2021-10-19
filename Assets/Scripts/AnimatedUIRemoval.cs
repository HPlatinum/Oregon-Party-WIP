using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimatedUIRemoval : MonoBehaviour {

    private Vector3 originalScale;
    private float squashTime = 0.2f;
    private float shrinkTime = 0.2f;

    //an external script will call StartSquash when the object should start shrinking
    //after the object is shrunked, delete this script from it

    private void Start() {
        SetOriginalScale();
    }

    public void StartSquash() {
        transform.DOScale(CalculateNewScale(1.2f, 1.2f, 1.2f), squashTime).OnComplete(StartShrink);
    }

    private void StartShrink() {
        transform.DOScale(Vector3.zero, shrinkTime).OnComplete(FinishShrinking);
    }

    private void FinishShrinking() {
        transform.localScale = originalScale;
        gameObject.SetActive(false);
        Destroy(this);
    }

    private Vector3 CalculateNewScale(float x, float y, float z) {
        return new Vector3(transform.localScale.x * x, transform.localScale.y * y, transform.localScale.z * z);
    }

    private void SetOriginalScale() {
        originalScale = transform.localScale;
    }

    public float GetTotalAnimationTime() {
        return squashTime + shrinkTime;
    }

}
