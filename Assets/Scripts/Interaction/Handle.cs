using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] Vector3 handOffset; 

    HandController controller;
    Rigidbody handRigidbody;
    Transform handTransform;

    const float BreakDistance = 0.8f;

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        controller = hand.Controller;
        controller.IsControlling = false;
        handRigidbody = controller.RB;
        handTransform = hand.transform;
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.useGravity = false;
        controller.IsControlling = true;
        controller = null;
        handRigidbody = null;
        handTransform = null;
    }

    public override void HeldFixedUpdate(float step)
    {
        if (controller)
        {
            handRigidbody.velocity = Vector3.zero;
            handRigidbody.angularVelocity = Vector3.zero;

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

        handTransform.position = transform.TransformPoint(-snapshot.position + offset);
        handTransform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);

        //TODO: Add velocity/movement/rotation to parent object?
    }
}