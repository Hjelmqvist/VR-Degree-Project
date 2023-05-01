using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class PoolCue : Interactable
{
    [SerializeField] PoolLine poolLine;
    [SerializeField] PoolCueTip cueTip;
    [SerializeField] Rigidbody secondaryHandleRB;
    Vector3 startPosition;
    Quaternion startRotation;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        secondaryHandle.gameObject.SetActive(true);
        poolLine.gameObject.SetActive(true);
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        secondaryHandle.gameObject.SetActive(false);
        poolLine.gameObject.SetActive(false);
        rb.freezeRotation = false;
        secondaryHandle.ApplyForces = true;
    }

    public override void HeldFixedUpdate(float step)
    {
        base.HeldFixedUpdate(step);

        bool interacting = IsInteracting || secondaryHandle.IsInteracting;
        rb.freezeRotation = interacting;

        bool sleeping = secondaryHandleRB.IsSleeping();

        if (IsInteracting && !sleeping)
        {
            secondaryHandleRB.Sleep();
        }
        else if (!IsInteracting && sleeping)
        {
            secondaryHandleRB.WakeUp();
        }

        secondaryHandle.ApplyForces = !interacting;
    }

    protected override void Move(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
    {
        Vector3 targetPosition;

        if (IsInteracting)
        {
            Vector3 direction = handInput.TransformPoint(snapshot.position) - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            targetPosition = transform.position + transform.forward * localDirection.z;
        }
        else
        {
            targetPosition = handInput.TransformPoint(snapshot.position);
        }
        rb.SetVelocity(targetPosition, step);
    }

    public override void StartInteract()
    {
        base.StartInteract();
        cueTip.gameObject.SetActive(true);
    }

    public override void StopInteract()
    {
        base.StopInteract();
        cueTip.gameObject.SetActive(false);
    }
}