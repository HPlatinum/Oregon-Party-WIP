using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReskinData : MonoBehaviour
{
    public List<GameObject> objectsThatNeedMaterialsChanged;
    public List<Material> materialOptions;

    public void UpdateMaterial(Material mat) {
        foreach(GameObject go in objectsThatNeedMaterialsChanged) {
            Material[] newMaterials = go.GetComponent<MeshRenderer>().materials;
            newMaterials[1] = mat;
            go.GetComponent<MeshRenderer>().materials = newMaterials;
        }

    }
    
}