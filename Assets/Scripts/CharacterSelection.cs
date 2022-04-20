using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterSelection : MonoBehaviour {

    public GameObject[] playerModelOptions;

    public GameObject farLeftCharacterPosition;
    public GameObject leftCharacterPosition;
    public GameObject centerCharacterPosition;
    public GameObject rightCharacterPosition;
    public GameObject farRightCharacterPosition;

    public int centerCharacterScale = 240;
    public int sideCharacterScale = 180;
    public int farSideCharacterScale = 120;

    private GameObject farLeftCharacter;
    private GameObject leftCharacter;
    private GameObject centerCharacter;
    private GameObject rightCharacter;
    private GameObject farRightCharacter;

    public int startingCenterCharacterIndex = 0;
    private int centerIndex;

    public float moveDuration = 0.5f;

    private void Start() {
        centerIndex = startingCenterCharacterIndex;

        CreateCharacterAtPosition(playerModelOptions[FarLeftIndex()], "far left");
        CreateCharacterAtPosition(playerModelOptions[LeftIndex()], "left");
        CreateCharacterAtPosition(playerModelOptions[centerIndex], "center");
        CreateCharacterAtPosition(playerModelOptions[RightIndex()], "right");
        CreateCharacterAtPosition(playerModelOptions[FarRightIndex()], "far right");
    }
    

    private void CreateCharacterAtPosition(GameObject character, string positionIdentifier) {


        //create the new object
        GameObject newPlayer = GameObject.Instantiate(character);

        //set some position-related values
        GameObject position = centerCharacterPosition;
        int scale = centerCharacterScale;
        switch (positionIdentifier) {
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

    public void PushedRightArrow() {
        MoveCharactersRight();

        //delete the old character
        Destroy(farRightCharacter);

        //reassign the existing characters to their new positioned objects
        farRightCharacter = rightCharacter;
        rightCharacter = centerCharacter;
        centerCharacter = leftCharacter;
        leftCharacter = farLeftCharacter;

        //change the center character index
        centerIndex = LeftIndex();

        CreateCharacterAtPosition(playerModelOptions[FarLeftIndex()], "far left");

        //todo update the character stats info
        
    }

    public void PushedLeftArrow() {
        MoveCharactersLeft();

        //delete the old character
        Destroy(farLeftCharacter);

        //reassign the existing characters to their new positioned objects
        farLeftCharacter = leftCharacter;
        leftCharacter = centerCharacter;
        centerCharacter = rightCharacter;
        rightCharacter = farRightCharacter;

        //change the center character index
        centerIndex = RightIndex();

        CreateCharacterAtPosition(playerModelOptions[FarRightIndex()], "far right");

        //todo update the character stats info

    }

    private void MoveCharactersRight() {
        //change the positions of the characters
        MoveCharacterToLeft(farLeftCharacter);
        MoveCharacterToCenter(leftCharacter);
        MoveCharacterToRight(centerCharacter);
        MoveCharacterToFarRight(rightCharacter);
    }

    private void MoveCharactersLeft() {
        //change the positions of the characters
        MoveCharacterToFarLeft(leftCharacter);
        MoveCharacterToLeft(centerCharacter);
        MoveCharacterToCenter(rightCharacter);
        MoveCharacterToRight(farRightCharacter);
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
    


}
