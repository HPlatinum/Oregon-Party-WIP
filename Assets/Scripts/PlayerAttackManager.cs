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
        TurnOffWeaponTrail();
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
        TurnOnWeaponTrail();
        PlayWeaponAttackAnimation();
    }

    private void PlayWeaponAttackAnimation() {
        if (StaticVariables.interactScript.objectInHand == null) {
            StaticVariables.PlayAnimation("Right Hook");
            return;
        }
        
        switch (StaticVariables.interactScript.itemInHand.weaponType) {
            case (WeaponType.Punch):
                StaticVariables.PlayAnimation("Right Hook");
                return;
            case (WeaponType.OneHandRight):
                StaticVariables.PlayAnimation("Swing Weapon 1h - Right Hand");
                return;
            case (WeaponType.TwoHand):
                StaticVariables.PlayAnimation("Swing Weapon 2h");
                return;

        }
    }

    private void TurnOnWeaponTrail(){
        if (StaticVariables.interactScript.objectInHand == null)
            TurnOnWeaponTrailForFist();
        else if (StaticVariables.interactScript.itemInHand.weaponType == WeaponType.Punch)
            TurnOnWeaponTrailForFist();
        else
            TurnOnWeaponTrailForWeapon();
    }    
    
    private void TurnOffWeaponTrail(){
        if (StaticVariables.interactScript.objectInHand == null)
            TurnOffWeaponTrailForFist();
        else if (StaticVariables.interactScript.itemInHand.weaponType == WeaponType.Punch)
            TurnOffWeaponTrailForFist();
        else
            TurnOffWeaponTrailForWeapon();
    }

    private void TurnOnWeaponTrailForFist(){
        Transform weaponTrailTrans = StaticVariables.interactScript.rightHand.transform.Find("Weapon Trail - Hand(Clone)");
        if (weaponTrailTrans == null){
            print("can't turn on the fist weapon trail, the 'Weapon Trail' object doesn't exist!");
            return;
        }
        weaponTrailTrans.gameObject.SetActive(true);
    }

    private void TurnOnWeaponTrailForWeapon(){
        if (StaticVariables.interactScript.objectInHand == null){
            print("can't turn on a weapon trail... you don't have a weapon!");
            return;
        }
        Transform weaponTrailTrans = StaticVariables.interactScript.objectInHand.transform.Find("Weapon Trail");
        if (weaponTrailTrans == null){
            print("can't turn on weapon trail, the 'Weapon Trail' object doesn't exist! Weapon is " + StaticVariables.interactScript.objectInHand.name);
            return;
        }
        weaponTrailTrans.gameObject.SetActive(true);
    }

    private void TurnOffWeaponTrailForFist(){
        Transform weaponTrailTrans = StaticVariables.interactScript.rightHand.transform.Find("Weapon Trail - Hand(Clone)");
        if (weaponTrailTrans == null){
            print("can't turn off the fist weapon trail, the 'Weapon Trail' object doesn't exist!");
            return;
        }
        weaponTrailTrans.gameObject.SetActive(false);
    }

    private void TurnOffWeaponTrailForWeapon(){
        if (StaticVariables.interactScript.objectInHand == null){
            print("can't turn off a weapon trail... you don't have a weapon!");
            return;
        }
        Transform weaponTrailTrans = StaticVariables.interactScript.objectInHand.transform.Find("Weapon Trail");
        if (weaponTrailTrans == null){
            print("can't turn off weapon trail, the 'Weapon Trail' object doesn't exist! Weapon is " + StaticVariables.interactScript.objectInHand.name);
            return;
        }
        weaponTrailTrans.gameObject.SetActive(false);

    }
    
} 
