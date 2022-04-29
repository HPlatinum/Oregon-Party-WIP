using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DisplayItem : MonoBehaviour {

    public float rotationSpeed = 50f;
    public bool shouldRotate = false;

    private void Update() {
        if (shouldRotate) 
            transform.Rotate(0, (rotationSpeed * Time.unscaledDeltaTime), 0);
    }

    public void AddItemAsChild(Item item, bool makeItemMaskable = false, float scale = -1) {
        if (scale <= 0)
            scale = 1;
        SetTransform(scale);
        GameObject newModel = GameObject.Instantiate(item.model, transform);
        SetLayerRecursively(newModel, 5); //assumes UI layer is #5
        SetModelTransform(newModel.transform, item, scale);

        if (makeItemMaskable) {//add the mask script
            AddMaskRecursively(newModel);
        }
    }

    private void AddMaskRecursively(GameObject go) {
        if (go.GetComponent<Renderer>() != null)
            go.AddComponent<MaskObject_AddToObjectsThatGetHidden>();
        foreach (Transform t in go.transform)
            AddMaskRecursively(t.gameObject);
    }

    private void SetTransform(float scale) {
        transform.localScale = Vector3.one * scale; //scale based on the container's scaling requirements
        transform.localPosition = new Vector3(0, 0, -500); //prevent clipping througn UI elements
    }

    private void SetModelTransform(Transform transform, Item item, float scale) {
        transform.localPosition = Vector3.zero;
        transform.localScale = transform.localScale * item.modelScale; //scale based on the item's scaling requirements
        transform.Rotate(item.modelRotation);
        transform.localPosition = new Vector3(item.modelPosition.x, item.modelPosition.y, 0);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer) {
        //sets the object and all children to be in the specified layer
        if (obj == null) 
            return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) {
            if (child == null)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public void ClearDisplay() {
        shouldRotate = false;
        foreach (Transform t in transform)
            GameObject.Destroy(t.gameObject);
    }
}
