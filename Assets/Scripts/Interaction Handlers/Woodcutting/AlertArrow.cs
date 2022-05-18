using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertArrow : MonoBehaviour
{
    public float displayTime;
    public bool ShowAlert;
    public Camera cam;
    public Transform deposit;
    public Transform player;
    public Image alert;
    private Vector3 viewPos;
    private float angle;
    private Timer timer;
    float zValueForAngle;
    float xValueForAngle;
    float hypotenuse;
    private float xValue;
    private float yValue;
    private int MaxXBorder;
    private int MinXBorder;
    private int MaxYBorder;
    private int MinYBorder;
    
    void Start() {
        CreateLocalVariables();
    }
    void Update() {
        if(!timer.TimerIsRunning()){
            timer.StartGameTimer(displayTime);
            print("Started timer");
        }
        if(timer.TimerWasStartedAndIsNowStopped()){
            Destroy(this.gameObject);
        }
        zValueForAngle = player.position.z - deposit.position.z;
        xValueForAngle = player.position.x - deposit.position.x;
        hypotenuse = Mathf.Sqrt((zValueForAngle*zValueForAngle) + (xValueForAngle*xValueForAngle));
        if(xValueForAngle < 0){
            angle = 180 + Mathf.Rad2Deg*Mathf.Atan(zValueForAngle/xValueForAngle);
        }
        else{
            angle = Mathf.Rad2Deg*Mathf.Atan(zValueForAngle/xValueForAngle);
        }

        if(hypotenuse <= 5 && hypotenuse > 2) {
            float scalePct = hypotenuse/5;
            alert.rectTransform.localScale = new Vector3(scalePct,scalePct,scalePct);
            Color opacity = alert.GetComponent<Image>().color;
            opacity.a = scalePct;
            alert.GetComponent<Image>().color = opacity;
        }
        if(hypotenuse <= 2) {
            Color opacity = alert.GetComponent<Image>().color;
            opacity.a = 0;
            alert.GetComponent<Image>().color = opacity;
        }

        viewPos = cam.WorldToViewportPoint(deposit.position);
        xValue = viewPos.x * 1920;
        yValue = viewPos.y * 1080;


        if(xValue > MaxXBorder) {
            xValue = MaxXBorder;
        }
        if(xValue < MinXBorder) {
            xValue = MinXBorder;
        }
        if(yValue > MaxYBorder) {
            yValue = MaxYBorder;
        }
        if(yValue < MinYBorder) {
            yValue = MinYBorder;
        }

        alert.rectTransform.anchoredPosition = new Vector3(xValue, yValue, 0);
        alert.rectTransform.rotation = Quaternion.Euler(0,0,angle);
    }
    
    private void CreateLocalVariables() {
        MaxXBorder = 1600;
        MinXBorder = 320;
        MaxYBorder = 800;
        MinYBorder = 280;
        ShowAlert = false;
        viewPos = new Vector3 (0,0,0);
        zValueForAngle = 0;
        xValueForAngle = 0;
        xValue = 0;
        yValue = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        alert = this.gameObject.GetComponent<Image>();
        cam = GameObject.Find("vThirdPersonCamera").GetComponent<Camera>();
        timer = this.gameObject.GetComponent<Timer>();
    }
}