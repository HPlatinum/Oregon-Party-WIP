using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedWoodcutting : MonoBehaviour
{
    Transform woodLeft;
    Transform woodRight;
    Transform log;

    // Start is called before the first frame update
    void Start()
    {
        SetLocalVariables();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate() {
    }

    public void SetLocalVariables() {
        log = transform.Find("Log");
        woodLeft = transform.Find("wood left");
        woodRight = transform.Find("wood right");
        ToggleWoodActive();        
    }

    public void ToggleWoodActive() {
        woodLeft.gameObject.SetActive(!woodRight.gameObject.activeSelf);
        woodRight.gameObject.SetActive(!woodRight.gameObject.activeSelf);
    }
    public void ToggleLogActive() {
        log.gameObject.SetActive(!log.gameObject.activeSelf);
    }

    public void AnimateWoodChop() {
        ToggleLogActive();
        ToggleWoodActive();
        woodLeft.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
        print(woodLeft.gameObject.GetComponent<Rigidbody>());
        woodLeft.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 50);
        print(woodRight.gameObject.GetComponent<Rigidbody>());
        woodRight.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
        woodRight.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 50);
        StaticVariables.WaitTimeThenCallFunction(1f, DestroyWoodObjects);
        StaticVariables.WaitTimeThenCallFunction(1.5f, DestroyLog);
    }

    public void DestroyWoodObjects() {
        StaticVariables.interactScript.DestroyInteractable(woodLeft);
        StaticVariables.interactScript.DestroyInteractable(woodRight);
    }

    private void DestroyLog() {
        Destroy(gameObject);
        print("hwerheklhrelkr");
    }

}
