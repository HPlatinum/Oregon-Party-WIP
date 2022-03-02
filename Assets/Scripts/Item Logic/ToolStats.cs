using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolStats : MonoBehaviour
{
    public Tool tool;
    public bool isBroken;
    public double wear;

    public void Start() {
    }

    private void Update() {
    }

    public double ReturnWear() {
        return wear;
    }

    public void SubtractFromWear(double valueToSubtract) {
        wear -= valueToSubtract;
    }

    public void SetToolToBroken() {
        isBroken = true;
    }

    public void RepairTool() {
        isBroken = false;
        wear = 100;
    }
}
