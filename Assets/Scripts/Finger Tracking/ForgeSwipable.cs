using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeSwipable : MonoBehaviour {

    public ForgeHandler forgeHandler;
    public bool isMetal;
    public float swipeForceMultiplier = 5f;
    public float fallAcceleration;
    public float maxFallSpeed;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        //make the object fall, unless it is at the max fall speed
        Vector2 force = new Vector2(0, -fallAcceleration);
        rb.AddForce(force);
        Vector2 v = rb.velocity;
        if (v.y < -maxFallSpeed) {
            v.y = -maxFallSpeed;
            rb.velocity = v;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll) {
        if (coll.tag == "Finger Tracker") {
            Vector2 force = swipeForceMultiplier * coll.GetComponent<ForgeFingerTracker>().velocity;
            rb.AddForce(force);
            //print("bam");
        }
        else if (coll.tag == "Forge Collection Area - Dirt") {
            forgeHandler.ItemEnteredCollectionArea(isMetal, false, gameObject);
        }
        else if (coll.tag == "Forge Collection Area - Metal") {
            forgeHandler.ItemEnteredCollectionArea(isMetal, true, gameObject);
        }
    }

}
