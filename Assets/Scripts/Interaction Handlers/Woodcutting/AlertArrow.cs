using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertArrow : MonoBehaviour
{
    public bool ShowAlert;
    public Camera cam;
    public Transform deposit;
    public Transform player;
    public Image alert;
    public Canvas canvas;
    private Vector3 viewPos;
    private float angle;
    float zValueForAngle;
    float xValueForAngle;
    private float xValue;
    private float yValue;
    private int MaxXBorder;
    private int MinXBorder;
    private int MaxYBorder;
    private int MinYBorder;
    
    void Start() {
        print("test");
        MaxXBorder = 1600;
        MinXBorder = 320;
        MaxYBorder = 800;
        MinYBorder = 280;
        ShowAlert = true;
        viewPos = new Vector3 (0,0,0);
        zValueForAngle = 0;
        xValueForAngle = 0;
        xValue = 0;
        yValue = 0;
    }
    void Update() {
        if(player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(ShowAlert) {
            zValueForAngle = player.position.z - deposit.position.z;
            xValueForAngle = player.position.x - deposit.position.x;
            if(xValueForAngle < 0){
                angle = 180 + Mathf.Rad2Deg*Mathf.Atan(zValueForAngle/xValueForAngle);
            }
            else{
                angle = Mathf.Rad2Deg*Mathf.Atan(zValueForAngle/xValueForAngle);
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
    }
}