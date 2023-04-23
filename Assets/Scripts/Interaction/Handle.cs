using Hjelmqvist.VR;
using UnityEngine;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] bool canBreakGrab = true;
    [SerializeField] Rigidbody bodyToMove;

    Quaternion startRotation;

    const float MoveDistanceThreshold = 0.01f;
    const float BreakDistanceThreshold = 0.8f;
    const float BreakDotRotationThreshold = 0f;
    const float DragMultiplier = 0.05f;
    const float RotateMultiplier = 2;

    public Transform HandInput => handInput;

    protected override void Awake()
    {
        base.Awake();
        startRotation = transform.rotation;
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.useGravity = false;
    }

    public void ResetRotation()
    {
        transform.rotation = startRotation;
    }

    public override void HeldFixedUpdate(float step)
    {
        float handDistance = Vector3.Distance(handTransform.position, handInput.position);
        ApplyForcesToMovingBody(handDistance);
        TryBreakGrab(handDistance);
    }

    protected void TryBreakGrab(float handDistance)
    {
        // Break hold if distance is too big or rotation is too different
        if (canBreakGrab)
        {
            bool distanceBreak = handDistance > BreakDistanceThreshold;
            bool rotationBreak = Vector3.Dot(handTransform.forward, handInput.forward) < BreakDotRotationThreshold;
            if (distanceBreak || rotationBreak)
            {
                holdingHand.DropInteractable();
                return;
            }
        }
    }

    protected void ApplyForcesToMovingBody(float handDistance)
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