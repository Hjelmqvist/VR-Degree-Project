using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void HoverStart()
    {
        Debug.Log("Start hovering");
    }

    public virtual void HoverEnd()
    {
        Debug.Log("Stop hovering");
    }

    public virtual void Pickup()
    {
        Debug.Log("Pickup");
        rb.isKinematic = true;
    }

    public virtual void Drop()
    {
        Debug.Log("Drop");
        rb.isKinematic = false;
    }

    public virtual void Interact()
    {

    }
}