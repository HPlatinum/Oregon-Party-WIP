using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour {

    
    public bool currentlyAttacking = false;
    private bool attackAnimationStarted = false;

    void Start() {
    }

    void Update() {
        bool justStarted = CheckIfAttackAnimationJustStarted();
        if (justStarted)
            ProcessAttackAnimationStarting();

        bool justEnded = CheckIfAttackAnimationJustEnded();
        if (justEnded)
            ProcessAttackAnimationEnding();
    }
    

    private void ProcessAttackAnimationEnding() {
        SetVariablesOnAttackAnimationEnd();
    }

    public void SetVariablesOnAttackAnimationEnd() {
        attackAnimationStarted = false;
        currentlyAttacking = false;
        StaticVariables.controller.lockMovement = false;
        StaticVariables.controller.lockRotation = false;
    }
    
    private bool CanPlayerAttack() {
        if (StaticVariables.interactScript.currentlyInteracting) {
            print("you can't attack, you are currently interacting");
            return false;
        }
        if (currentlyAttacking) {
            print("you can't attack, you are already attacking");
            return false;
        }
        //if (StaticVariables.interactScript.objectInHand == null) {
        //    print("you can't attack, you're not holding anything!");
        //    return false;
        //}
        return true;
    }

    private void PreparePlayerForAttack() {
        currentlyAttacking = true;
        attackAnimationStarted = false;
        StaticVariables.controller.lockMovement = true;
        StaticVariables.controller.lockRotation = true;
        StaticVariables.controller.HaltPlayerVelocity();
    }

    private bool CheckIfAttackAnimationJustStarted() {
        if (currentlyAttacking) {
            if (!attackAnimationStarted) {
                if (StaticVariables.DoesPlayerAnimatorStateHaveAttackTag()) {
                    return true;
                }
            }
        }
        return false;
    }

    private void ProcessAttackAnimationStarting() {
        attackAnimationStarted = true;
    }

    private bool CheckIfAttackAnimationJustEnded() {
        if (currentlyAttacking) {
            if (attackAnimationStarted) {
                if (!StaticVariables.DoesPlayerAnimatorStateHaveAttackTag()) {
                    return true;
                }
            }
        }
        return false;
    }

    public void StartAttack() {

        if (!CanPlayerAttack())
            return;

        currentlyAttacking = true;
        PlayWeaponAttackAnimation();
    }

    private void PlayWeaponAttackAnimation() {
        if (StaticVariables.interactScript.objectInHand == null)
            StaticVariables.PlayAnimation("Right Hook");
        else
            StaticVariables.PlayAnimation("Swing Weapon 1h - Right Hand");
        //StaticVariables.PlayAnimation("Swing Weapon 2h");
    }
} 
