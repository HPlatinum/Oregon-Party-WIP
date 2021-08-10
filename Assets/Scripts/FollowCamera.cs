using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCamera : MonoBehaviour {
    //this script only exists to make sure the camera follows the player.
    //As long as there is exactly one "Player"-tagged gameobject in the scene, the camera will automatically follow them.
    //This script exists to make it easier and quicker to build levels
    
    void Start() {
        GetComponent<CinemachineVirtualCamera>().Follow = GameObject.FindGameObjectsWithTag("Player")[0].transform;
    }

}
