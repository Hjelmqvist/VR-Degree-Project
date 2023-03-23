using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] Vector3 handOffset; 
    HandController controller;

    const float BreakDistance = 0.8f;

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        Physics.IgnoreCollision(GetComponent<Collider>(), hand.GetComponent<Collider>());
        if (hand.TryGetComponent(out controller))
        {
            controller.IsControlling = false;
        }
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.useGravity = false;
        Physics.IgnoreCollision(GetComponent<Collider>(), hand.GetComponent<Collider>(), false);
        if (controller)
        {
            controller.IsControlling = true;
            controller = null;
        }
    }

    public override void HeldFixedUpdate(float step)
    {
        if (controller)
        {
            controller.RB.velocity = Vector3.zero;
            controller.RB.angularVelocity = Vector3.zero;

            if (Vector3.Distance(holdingHand.transform.position, controller.Input.position) > BreakDistance)
            {
                holdingHand.DropInteractable();
                return;
            }
        }

        //TODO: Figure out the math to not need the offset?

        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
        Vector3 offset = handOffset;
        if (holdingHand.HandType == SteamVR_Input_Sources.LeftHand)
        {
            offset.x = -offset.x;
        }
        Vector3 position = transform.TransformPoint(-snapshot.position + offset);
        holdingHand.transform.position = position;
        holdingHand.transform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);
    }
}