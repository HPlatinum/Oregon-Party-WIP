using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeFingerTracker : MonoBehaviour {

    bool isTouching = false;
    private Rigidbody2D rb;
    public Camera canvasCamera;
    private CircleCollider2D coll;
    public GameObject trailPrefab;

    private GameObject currentTrail;
    Vector2 prevPos;
    public float minActiveSpeed = 0.1f; //the speed at which the collider activates
    public Vector2 velocity = Vector2.zero;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            isTouching = true;

            currentTrail = Instantiate(trailPrefab, transform);
        }
        else if (Input.GetMouseButtonUp(0)) {
            isTouching = false;
            coll.enabled = false;

            currentTrail.transform.parent = transform.parent;
            Destroy(currentTrail, 2);
        }
        
        UpdatePosition();
    }


    void UpdatePosition() {

        var mousePos = Input.mousePosition;
        mousePos.z = 10; // select distance = 10 units from the camera
        //Debug.Log(camera.ScreenToWorldPoint(mousePos));


        Vector2 newPos = canvasCamera.ScreenToWorldPoint(mousePos);
        //print(newPos);
        rb.position = newPos;
        velocity = (newPos - prevPos) / Time.deltaTime;
        //float speed = (newPos - prevPos).magnitude / Time.deltaTime;

        //velocity = speed * 
        prevPos = newPos;

        if (isTouching) {
            if (velocity.magnitude > minActiveSpeed)
                coll.enabled = true;
            else
                coll.enabled = false;
        }

    }
}
