using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE, GrabDrop, Toggle }
    public InteractionType type;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.layer = 13;
    }

    public void Interact()
    {
        switch (type)
        {
            case InteractionType.GrabDrop:
                //Grab interaction
                FindObjectOfType<InteractionSystem>().GrabDrop();
                Debug.Log("GRAB DROP");
                break;
            case InteractionType.Toggle:
                //Toggle interaction
                FindObjectOfType<UseableSwitch>().Use();
                Debug.Log("TOGGLE");
                break;
            default:
                Debug.Log("NULL ITEM");
                break;
        }
    }
}
