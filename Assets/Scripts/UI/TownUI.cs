using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownUI : MonoBehaviour
{
    public GameObject townUI;
    // Start is called before the first frame update
    void Start()
    {
        townUI.SetActive(false);
    }

    void Update()
    {
        townState();
    }

    // currently allows you to open the town menu by pressing T. Functionality to be added later
    public bool townState() {
        if(Input.GetButtonDown("TestOpenTown")){
            townUI.SetActive(!townUI.activeSelf);
            return(townUI.activeSelf);
        }
        return(townUI.activeSelf);
    }
}
