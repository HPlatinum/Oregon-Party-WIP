using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public string characterName;
    public int maxHealth;
    public int maxSanity;
    //other stat ideas: movement speed, weapon speed, shop price discount

    public GameObject vehiclePrefab;
    [Header("Character Selection")]
    public Vector3 vehiclePosition;
    public Vector3 vehicleRotation;
    public float vehicleScale;

}
