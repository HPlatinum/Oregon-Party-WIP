using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerModelSetup: MonoBehaviour
{
    public GameObject fistWeaponTrailPrefab;
    private GameObject[] playerModelOptions;
    [Header("Character Override")]
    public bool overrideSelectedCharacterWithThisOption = false;
    public CharacterAppearanceData characterSelectionOverride;

    private GameObject playerModel;

    private CharacterAppearanceData characterAppearanceData;
    //private GameObject modelToUse;
    //private int materialIndex;

    public void CreatePlayerModelInstanceInScene() {
        //SetOverrideCharacterModelReference();
        SetCharacterAppearanceData();

        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
        if (temp.Length != 1) {
            print("There is not exactly one gameobject in the scene witht the Player Tag! The Player Tagged objects are:");
            foreach(GameObject go in temp) {
                print(go.name);
            }
        }
        else {
            GameObject playerSpawnPoint = temp[0];

            //create the new object
            playerModel = GameObject.Instantiate(characterAppearanceData.modelPrefab);

            //set the transforms
            playerModel.transform.SetParent(null);
            playerModel.transform.localPosition = playerSpawnPoint.transform.position;

            //set the model material
            playerModel.GetComponent<PlayerReskinData>().UpdateMaterial(characterAppearanceData.materialIndex);

            //add the punching weapon trail
            AddWeaponTrailToRightFist();

            GameObject.Destroy(playerSpawnPoint);
        }
        
    }

    //private void SetOverrideCharacterModelReference(){
    //    characterSelectionOverride.SetModelFromList(transform.Find("Player Models List").GetComponent<PlayerModelsList>().models);
    //}

    private void SetCharacterAppearanceData(){
        //if we are overriding the player option with this predefined one, then use the predefined option
        if (overrideSelectedCharacterWithThisOption){
            characterAppearanceData = characterSelectionOverride;
            characterAppearanceData.SetModelFromList(transform.Find("Player Models List").GetComponent<PlayerModelsList>().models);
            //modelToUse = characterSelectionOverride.modelPrefab;
            //materialIndex = characterSelectionOverride.materialIndex;
        }

        //if staticvariables does not have a chosen character option, then use the predefined one
        else if (StaticVariables.chosenCharactedAppearanceData == null){
            characterAppearanceData = characterSelectionOverride;
            characterAppearanceData.SetModelFromList(transform.Find("Player Models List").GetComponent<PlayerModelsList>().models);
        }

        //otherwise, use the selected options from staticvariables
        else{
            characterAppearanceData = StaticVariables.chosenCharactedAppearanceData;
            //modelToUse = StaticVariables.chosenCharactedAppearanceData.modelPrefab;
            //materialIndex = StaticVariables.chosenCharactedAppearanceData.materialIndex;
        }
    }



    public void SetCameraToFollowPlayer() {
        transform.parent.Find("Virtual Follow Camera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerModel.transform;
    }

    public int GetMaxHealth() {
        return characterAppearanceData.modelPrefab.GetComponent<CharacterStats>().maxHealth;
    }

    public int GetMaxSanity() {
        return characterAppearanceData.modelPrefab.GetComponent<CharacterStats>().maxSanity;
    }

    private void AddWeaponTrailToRightFist(){
        GameObject trail = GameObject.Instantiate(fistWeaponTrailPrefab);

        //we can't use StaticVariables.interactScript.rightHand because the staticvariables reference has not been declared yet
        Transform rightHand = playerModel.transform.Find("InteractCollider").GetComponent<InteractionManager>().rightHand;
        
        trail.transform.SetParent(rightHand);
        trail.transform.localPosition = Vector3.zero;
        trail.SetActive(false);
}
}