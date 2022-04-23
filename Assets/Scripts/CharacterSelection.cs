using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour {

    public GameObject[] playerModelOptions;

    public GameObject farFarLeftCharacterPosition;
    public GameObject farLeftCharacterPosition;
    public GameObject leftCharacterPosition;
    public GameObject centerCharacterPosition;
    public GameObject rightCharacterPosition;
    public GameObject farRightCharacterPosition;
    public GameObject farFarRightCharacterPosition;

    public int centerCharacterScale = 240;
    public int sideCharacterScale = 180;
    public int farSideCharacterScale = 120;
    public int farFarSideCharacterScale = 80;

    private GameObject farFarLeftCharacter;
    private GameObject farLeftCharacter;
    private GameObject leftCharacter;
    private GameObject centerCharacter;
    private GameObject rightCharacter;
    private GameObject farRightCharacter;
    private GameObject farFarRightCharacter;

    public int startingCenterCharacterIndex = 0;
    private int centerIndex;

    public float moveDuration = 0.5f;

    [Header("Character Stats Display")]
    public Text nameText;
    public Text inventorySizeText;
    public Text healthText;
    public Text sanityText;
    public GameObject vehicleParent;
    private GameObject currentVehicle;


    private void Start() {
        centerIndex = startingCenterCharacterIndex;

        CreateCharacterAtPosition(playerModelOptions[FarFarLeftIndex()], "far far left");
        CreateCharacterAtPosition(playerModelOptions[FarLeftIndex()], "far left");
        CreateCharacterAtPosition(playerModelOptions[LeftIndex()], "left");
        CreateCharacterAtPosition(playerModelOptions[centerIndex], "center");
        CreateCharacterAtPosition(playerModelOptions[RightIndex()], "right");
        CreateCharacterAtPosition(playerModelOptions[FarRightIndex()], "far right");
        CreateCharacterAtPosition(playerModelOptions[FarFarRightIndex()], "far far right");

        UpdateCharacterStats();
    }
    

    private void CreateCharacterAtPosition(GameObject character, string positionIdentifier) {


        //create the new object
        GameObject newPlayer = GameObject.Instantiate(character);

        //set some position-related values
        GameObject position = centerCharacterPosition;
        int scale = centerCharacterScale;
        switch (positionIdentifier) {
            case ("far far left"):
                position = farFarLeftCharacterPosition;
                scale = farFarSideCharacterScale;
                farFarLeftCharacter = newPlayer;
                break;
            case ("far left"):
                position = farLeftCharacterPosition;
                scale = farSideCharacterScale;
                farLeftCharacter = newPlayer;
                break;
            case ("left"):
                position = leftCharacterPosition;
                scale = sideCharacterScale;
                leftCharacter = newPlayer;
                break;
            case ("center"):
                position = centerCharacterPosition;
                scale = centerCharacterScale;
                centerCharacter = newPlayer;
                break;
            case ("right"):
                position = rightCharacterPosition;
                scale = sideCharacterScale;
                rightCharacter = newPlayer;
                break;
            case ("far right"):
                position = farRightCharacterPosition;
                scale = farSideCharacterScale;
                farRightCharacter = newPlayer;
                break;
            case ("far far right"):
                position = farFarRightCharacterPosition;
                scale = farFarSideCharacterScale;
                farFarRightCharacter = newPlayer;
                break;
        }


        //set the transforms
        newPlayer.transform.SetParent(position.transform.parent);
        newPlayer.transform.position = position.transform.position;
        newPlayer.transform.localScale = new Vector3(scale,scale,scale);
        newPlayer.transform.rotation = Quaternion.Euler(0, 180, 0);

        //remove unnecessary components
        Destroy(newPlayer.GetComponent<Invector.vCharacterController.vThirdPersonController>());
        Destroy(newPlayer.GetComponent<Invector.vCharacterController.vThirdPersonInput>());
        Destroy(newPlayer.GetComponent<Rigidbody>());
        Destroy(newPlayer.GetComponent<CapsuleCollider>());

        //set the layer
        SetLayerRecursively(newPlayer, 5);

        //add the mask script
        PlayerReskinData prd = newPlayer.GetComponent<PlayerReskinData>();
        foreach (GameObject go in prd.objectsThatNeedMaterialsChanged)
            go.AddComponent<MaskObject_AddToObjectsThatGetHidden>();
    }

    private void SetLayerRecursively(GameObject go, int i) {
        go.layer = i;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, i);
    }

    private int FarFarLeftIndex() {
        int i = centerIndex - 3;
        if (i < 0)
            i += playerModelOptions.Length;
        return i;
    }

    private int FarLeftIndex() {
        int i = centerIndex - 2;
        if (i < 0)
            i += playerModelOptions.Length;
        return i;
    }

    private int LeftIndex() {
        int i = centerIndex - 1;
        if (i < 0)
            i += playerModelOptions.Length;
        return i;

    }

    private int RightIndex() {
        int i = centerIndex + 1;
        if (i >= playerModelOptions.Length)
            i -= playerModelOptions.Length;
        return i;
    }

    private int FarRightIndex() {
        int i = centerIndex + 2;
        if (i >= playerModelOptions.Length)
            i -= playerModelOptions.Length;
        return i;
    }

    private int FarFarRightIndex() {
        int i = centerIndex + 3;
        if (i >= playerModelOptions.Length)
            i -= playerModelOptions.Length;
        return i;
    }

    public void PushedLeftArrow() {
        MoveCharactersRight();

        //delete the old character
        Destroy(farFarRightCharacter);

        //reassign the existing characters to their new positioned objects
        farFarRightCharacter = farRightCharacter;
        farRightCharacter = rightCharacter;
        rightCharacter = centerCharacter;
        centerCharacter = leftCharacter;
        leftCharacter = farLeftCharacter;
        farLeftCharacter = farFarLeftCharacter;

        //change the center character index
        centerIndex = LeftIndex();

        CreateCharacterAtPosition(playerModelOptions[FarFarLeftIndex()], "far far left");

        UpdateCharacterStats();
    }

    public void PushedRightArrow() {
        MoveCharactersLeft();

        //delete the old character
        Destroy(farFarLeftCharacter);

        //reassign the existing characters to their new positioned objects
        farFarLeftCharacter = farLeftCharacter;
        farLeftCharacter = leftCharacter;
        leftCharacter = centerCharacter;
        centerCharacter = rightCharacter;
        rightCharacter = farRightCharacter;
        farRightCharacter = farFarRightCharacter;

        //change the center character index
        centerIndex = RightIndex();

        CreateCharacterAtPosition(playerModelOptions[FarFarRightIndex()], "far far right");

        UpdateCharacterStats();
    }

    private void MoveCharactersRight() {
        //change the positions of the characters
        MoveCharacterToFarLeft(farFarLeftCharacter);
        MoveCharacterToLeft(farLeftCharacter);
        MoveCharacterToCenter(leftCharacter);
        MoveCharacterToRight(centerCharacter);
        MoveCharacterToFarRight(rightCharacter);
        MoveCharacterToFarFarRight(farRightCharacter);
    }

    private void MoveCharactersLeft() {
        //change the positions of the characters
        MoveCharacterToFarFarLeft(farLeftCharacter);
        MoveCharacterToFarLeft(leftCharacter);
        MoveCharacterToLeft(centerCharacter);
        MoveCharacterToCenter(rightCharacter);
        MoveCharacterToRight(farRightCharacter);
        MoveCharacterToFarRight(farFarRightCharacter);
    }

    private void MoveCharacterToFarFarLeft(GameObject character) {
        character.transform.DOMove(farFarLeftCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(farFarSideCharacterScale, moveDuration);
    }

    private void MoveCharacterToFarLeft(GameObject character) {
        character.transform.DOMove(farLeftCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(farSideCharacterScale, moveDuration);
    }

    private void MoveCharacterToLeft(GameObject character) {
        character.transform.DOMove(leftCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(sideCharacterScale, moveDuration);
    }

    private void MoveCharacterToCenter(GameObject character) {
        character.transform.DOMove(centerCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(centerCharacterScale, moveDuration);
    }

    private void MoveCharacterToRight(GameObject character) {
        character.transform.DOMove(rightCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(sideCharacterScale, moveDuration);
    }

    private void MoveCharacterToFarRight(GameObject character) {
        character.transform.DOMove(farRightCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(farSideCharacterScale, moveDuration);
    }

    private void MoveCharacterToFarFarRight(GameObject character) {
        character.transform.DOMove(farFarRightCharacterPosition.transform.position, moveDuration);
        character.transform.DOScale(farFarSideCharacterScale, moveDuration);
    }

    private void UpdateCharacterStats() {
        CharacterStats stats = centerCharacter.GetComponent<CharacterStats>();

        nameText.text = stats.characterName;
        inventorySizeText.text = "Inventory Size: " + stats.inventorySize;
        healthText.text = "Health: " + stats.maxHealth;
        sanityText.text = "Sanity: " + stats.maxSanity;


        Destroy(currentVehicle);

        CreateVehicle(stats);

    }

    private void CreateVehicle(CharacterStats stats) {

        //create the new object
        GameObject vehicle = GameObject.Instantiate(stats.vehiclePrefab);

        //set some transform values
        vehicle.transform.parent = vehicleParent.transform;
        vehicle.transform.localPosition = stats.vehiclePosition;
        vehicle.transform.localScale = new Vector3(stats.vehicleScale, stats.vehicleScale, stats.vehicleScale);
        vehicle.transform.rotation = Quaternion.Euler(stats.vehicleRotation);

        //set the layer
        SetLayerRecursively(vehicle, 5);

        currentVehicle = vehicle;
    }

}
