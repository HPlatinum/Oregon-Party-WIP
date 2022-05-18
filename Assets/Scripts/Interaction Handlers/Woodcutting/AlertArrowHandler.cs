using UnityEngine;

public class AlertArrowHandler : MonoBehaviour
{
    public Transform testObj;
    public GameObject[] arrows;

    public void Start() {
    }
    public void CreateWaypointer(int imageType, float displayTime, Transform targetObject) {
        GameObject arrow = Instantiate(arrows[imageType]);
        arrow.transform.SetParent(this.gameObject.transform, false);
        arrow.GetComponent<AlertArrow>().displayTime = displayTime;
        arrow.GetComponent<AlertArrow>().deposit = targetObject;
    }
}
