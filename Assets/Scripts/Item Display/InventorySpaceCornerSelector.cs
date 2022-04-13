using UnityEngine;
using UnityEngine.UI;

public class InventorySpaceCornerSelector : MonoBehaviour { 

    //attach to an object that is the parent of many inventory space objects and nothing else
    //shows the images for each slot that fits its position (ex: the top-right slot shows a corner in the top-right)
    
    public int totalInventorySlots = 24;
    public int columnCount = 4;
    
    void Start() {
        int i = 0;

        foreach (Transform t in transform) {
            bool isTopRow = false;
            bool isBottomRow = false;
            bool isLeftEdge = false;
            bool isRightEdge = false;

            if (i < columnCount)
                isTopRow = true;
            if (i >= (totalInventorySlots - columnCount))
                isBottomRow = true;
            if ((i % columnCount) == 0)
                isLeftEdge = true;
            if ((i % columnCount) == (columnCount - 1))
                isRightEdge = true;


            if (isTopRow && isLeftEdge) {
                t.Find("Image Options").Find("top-left").gameObject.SetActive(true);
            }
            else if (isTopRow && isRightEdge) {
                t.Find("Image Options").Find("top-right").gameObject.SetActive(true);
            }
            else if (isTopRow) {
                t.Find("Image Options").Find("top-middle").gameObject.SetActive(true);
            }
            else if (isBottomRow && isLeftEdge) {
                t.Find("Image Options").Find("bottom-left").gameObject.SetActive(true);
            }
            else if (isBottomRow && isRightEdge) {
                t.Find("Image Options").Find("bottom-right").gameObject.SetActive(true);
            }
            else if (isBottomRow) {
                t.Find("Image Options").Find("bottom-middle").gameObject.SetActive(true);
            }
            else if (isLeftEdge) {
                t.Find("Image Options").Find("middle-left").gameObject.SetActive(true);
            }
            else if (isRightEdge) {
                t.Find("Image Options").Find("middle-right").gameObject.SetActive(true);
            }
            else {
                t.Find("Image Options").Find("middle").gameObject.SetActive(true);
            }

            i++;
        }

    }

}
