using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerObject : MonoBehaviour{

    public enum DamageType { health, sanity };
    public DamageType damageType = DamageType.health;
    public int damageAmount;

    private void OnTriggerEnter(Collider coll) {
        if (coll.tag == "Damage Detection") {
            StaticVariables.healthAndSanityTracker.EnterDangerZone(this);
            //print("entered danger area");
        }
    }

    private void OnTriggerExit(Collider coll) {
        if (coll.tag == "Damage Detection") {
            StaticVariables.healthAndSanityTracker.LeaveDangerZone(this);
            //print("exited danger area");
        }
    }
}
