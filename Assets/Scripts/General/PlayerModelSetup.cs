using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerModelSetup: MonoBehaviour
{
    
    public GameObject[] playerModelOptions;
    public int chosenModelOption;
    public int chosenMaterialOption;

    //option for the chosen player model
    //option for the chosen material?

    private GameObject playerModel;

    public void CreatePlayerModelInstanceInScene() {
        GameObject chosenModel = playerModelOptions[chosenModelOption];

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
            playerModel = GameObject.Instantiate(chosenModel);

            //set the transforms
            playerModel.transform.SetParent(null);
            playerModel.transform.localPosition = playerSpawnPoint.transform.position;

            //set the model material
            playerModel.GetComponent<PlayerReskinData>().UpdateMaterial(chosenMaterialOption);

            GameObject.Destroy(playerSpawnPoint);
        }
        
    }

    public void SetCameraToFollowPlayer() {
        transform.parent.Find("Virtual Follow Camera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerModel.transform;
    }

}