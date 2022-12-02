using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [Header("Detect Objects")]
    //Detection Point
    public Transform _interactionPoint;
    //Detection Radius
    const float _interactionPointRadius = 0.5f;
    //Detection Layer
    public LayerMask _interactableMask;
    //Cached Trigger Object
    public GameObject detectedObject;

    [Header("Grab Drop Object")]
    [SerializeField] bool isGrabbing;
    [SerializeField] float grabbedObjectyYValue;
    public GameObject grabbedObject;
    public Transform grabPoint;

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (DetectObject())
        {
          if (context.started)
          {
                //If grabbedObject != null. no interaction. drop item first
                if (isGrabbing)
                {
                    GrabDrop();
                    return;
                }

                detectedObject.GetComponent<Item>().Interact();
          }
        }
    }

    bool DetectObject()
    { 
        Collider2D obj = Physics2D.OverlapCircle(_interactionPoint.position, _interactionPointRadius, _interactableMask);

        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public void GrabDrop()
    {
        // check if grabbedObject = !null => drop object
        if (isGrabbing)
        {
            isGrabbing = false;

            grabbedObject.GetComponent<Collider2D>().isTrigger = false;
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;

            grabbedObject.transform.parent = null;

            //grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x, grabbedObjectyYValue, grabbedObject.transform.position.z);

            grabbedObject = null;

        }
        // Check if grabbedObject = null => grab object
        else
        {
            isGrabbing = true;

            grabbedObject = detectedObject;

            grabbedObject.GetComponent<Collider2D>().isTrigger = true;
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;

            grabbedObject.transform.parent = transform;

            //grabbedObjectyYValue = grabbedObject.transform.position.y;

            grabbedObject.transform.localPosition = grabPoint.localPosition;
        }
    }
}
