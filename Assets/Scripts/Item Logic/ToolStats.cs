using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolStats : MonoBehaviour
{
    public Tool tool;
    public bool isBroken;
    public double wear;

    public void Awake() {
        if(wear == 0) {
            ResetWear();
        }
        
    }
    public void Start() {
    }

    public double ReturnWear() {
        return wear;
    }

    public void SubtractFromWear(double valueToSubtract) {
        wear -= valueToSubtract;
    }

    public void ResetWear() {
        wear = tool.initialWear;
    }
}
