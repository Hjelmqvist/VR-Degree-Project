using UnityEngine;
using Valve.VR;

public class MyHand : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources handType = SteamVR_Input_Sources.LeftHand;
    [SerializeField] SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

    [Space(10)]
    [Header("Pickup/Hover")]
    [SerializeField] Transform overlapPosition;
    [SerializeField] float overlapRadius;
    [SerializeField] LayerMask overlapLayers;

    [Space(10)]
    [SerializeField] Transform castTransform;
    [SerializeField] float castDistance = 5;
    [Tooltip("Direction in relation to the Cast Transform")]
    [SerializeField] Vector3 castDirection;
    [SerializeField] LineRenderer castLine;

    Interactable hoveredInteractable = null;
    bool isHovering = false;

    Interactable heldInteractable = null;
    bool isHolding = false;

    void Update()
    {
        if (grabGripAction.stateUp)
        {
            if (isHovering)
            {
                hoveredInteractable.HoverEnd();
                isHovering = false;
            }

            if (isHolding)
            {
                heldInteractable.Drop();
                heldInteractable = null;
                isHolding = false;
            }
        }

        if (!isHolding) // Search for object
        {
            if (TryGetInteractable(out Interactable interactable))
            {
                if (grabGripAction.stateDown)
                {
                    heldInteractable.Pickup();
                    heldInteractable = interactable;
                    isHolding = true;
                }
                else
                {
                    interactable.HoverStart();
                    isHovering = true;
                }
            }
        }
    }

    private bool TryGetInteractable(out Interactable interactable)
    {
        interactable = null;
        if (TryFindClosestOverlappingInteractable(out interactable))
            return true;
        return TryFindFindInteractableWithCast(out interactable);
    }

    private bool TryFindClosestOverlappingInteractable(out Interactable interactable)
    {
        Collider[] overlapping = Physics.OverlapSphere(overlapPosition.position, overlapRadius, overlapLayers);

        float closestDistance = float.MaxValue;
        interactable = null;

        for (int i = 0; i < overlapping.Length; i++)
        {
            if (overlapping[i].TryGetComponent(out Interactable current))
            {
                float distance = Vector3.Distance(current.transform.position, overlapPosition.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    interactable = current;
                }
            }
        }

        return interactable != null;
    }

    private bool TryFindFindInteractableWithCast(out Interactable interactable)
    {
        interactable = null;

        RaycastHit[] hits = Physics.SphereCastAll(castTransform.position, castDistance, castTransform.TransformDirection(castDirection));

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            // Return first found interactable
            if (hit.transform.TryGetComponent(out interactable))
            {
                return true;
            }
        }


        return false;
    }

    void AttachInteractable(Interactable interactable)
    {

    }

    void DetachInteractable()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(overlapPosition.position, overlapRadius);
        Gizmos.DrawLine(castTransform.position, castTransform.position + castTransform.TransformDirection(castDirection) * castDistance);
    }
}
