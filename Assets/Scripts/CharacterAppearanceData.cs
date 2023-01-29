using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterAppearanceData{
    public int modelIndex;
    public int materialIndex;

    [HideInInspector]
    public GameObject modelPrefab;

    public void SetModelFromList(GameObject[] modelsList){
        modelPrefab = modelsList[modelIndex];
    }
}
