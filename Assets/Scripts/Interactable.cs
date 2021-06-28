using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractTypes { Pickup};
    public InteractTypes interactType;

    public Item item;
}
