using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] Vector3 handOffset;
    [SerializeField] Rigidbody bodyToMove;
    [SerializeField] Collider[] collidersToIgnore;

    HandController controller;
    Rigidbody handRigidbody;
    Transform handTransform;
    Transform handInput;

    Quaternion startRotation;

    const float MoveDistanceThreshold = 0.01f;
    const float BreakDistanceThreshold = 0.8f;
    const float BreakDotRotationThreshold = 0f;
    const float DragMultiplier = 0.05f;
    const float RotateMultiplier = 2;

    protected override void Awake()
    {
        base.Awake();
        startRotation = transform.rotation;
    }

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        ToggleCollisions(hand, true);
        controller = hand.Controller;
        controller.IsControlling = false;
        handRigidbody = controller.RB;
        handTransform = hand.transform;
        handInput = controller.Input;
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        ToggleCollisions(hand, false);
        controller.IsControlling = true;
        rb.useGravity = false;
    }

    private void ToggleCollisions(Hand hand, bool ignore)
    {
        for (int i = 0; i < collidersToIgnore.Length; i++)
        {
            Physics.IgnoreCollision(collidersToIgnore[i], hand.Collider, ignore);
        }
    }

    public void ResetRotation()
    {
        transform.rotation = startRotation;
    }

    public override void HeldFixedUpdate(float step)
    {
        MoveHandToHandle();

        float handDistance = Vector3.Distance(handTransform.position, handInput.position);

        ApplyForcesToMovingBody(handDistance);

        // Break hold if distance is too big or rotation is too different
        if (handDistance > BreakDistanceThreshold ||
            Vector3.Dot(handTransform.forward, handInput.forward) < BreakDotRotationThreshold)
        {
            holdingHand.DropInteractable();
            return;
        }
    }

    private void MoveHandToHandle()
    {
        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);

        Vector3 offset = handOffset;
        if (holdingHand.HandType == SteamVR_Input_Sources.LeftHand)
        {
            offset.x = -offset.x;
        }

        handRigidbody.velocity = Vector3.zero;
        handRigidbody.angularVelocity = Vector3.zero;
        handTransform.position = transform.TransformPoint(-snapshot.position + offset);
        handTransform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);
    }

    private void ApplyForcesToMovingBody(float handDistance)
    {
        if (bodyToMove && handDistance > MoveDistanceThreshold)
        {
            Vector3 direction = handInput.position - handTransform.position;
            Vector3 targetVelocity = direction / Time.fixedDeltaTime * DragMultiplier;
            bodyToMove.velocity = targetVelocity;

            Quaternion rotationDifference = handInput.rotation * Quaternion.Inverse(handTransform.rotation);
            rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0 && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
            {
                Vector3 angularTarget = angle * axis * RotateMultiplier * Time.fixedDeltaTime;
                bodyToMove.angularVelocity = angularTarget;
            }
        }
    }
}