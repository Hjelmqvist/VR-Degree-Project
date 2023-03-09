using UnityEngine;
using Valve.VR;

namespace Hjelmqvist.VR
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] SteamVR_Input_Sources handType = SteamVR_Input_Sources.LeftHand;
        [SerializeField] SteamVR_Action_Single squeezeAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
        [SerializeField] SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Interact");

        [SerializeField, Range(0, 1)] float grabStrength = 0.5f;

        [Space(10)]
        [Header("Pickup/Hover")]
        [SerializeField] Transform overlapPosition;
        [SerializeField] float overlapRadius;
        [SerializeField] LayerMask overlapLayers;

        [Space(10)]
        [SerializeField] Transform castTransform;
        [SerializeField] float castRadius = 0.1f;
        [SerializeField] float castDistance = 5;
        [SerializeField] LineRenderer castLine;

        Interactable hoveredInteractable = null;
        Interactable heldInteractable = null;
        bool isHolding = false;

        SteamVR_Behaviour_Skeleton skeleton;

        public SteamVR_Behaviour_Skeleton Skeleton
        {
            get
            {
                if (skeleton == null)
                    skeleton = GetComponentInChildren<SteamVR_Behaviour_Skeleton>();
                return skeleton;
            }
        }

        public SteamVR_Input_Sources HandType => handType;

        void Update()
        {
            if (!isHolding) // Search for object
            {
                TryPickupInteractable();
            }
            else
            {
                if (interactAction.stateDown)
                {
                    heldInteractable.Interact();
                }

                if (squeezeAction.GetAxis(handType) < grabStrength)
                {
                    DropInteractable(); // Drop object
                }
            }
        }

        private void FixedUpdate()
        {
            if (isHolding)
            {
                heldInteractable.HeldFixedUpdate();
            }
        }

        private void TryPickupInteractable()
        {
            if (TryGetInteractable(out Interactable interactable))
            {
                if (squeezeAction.GetAxis(handType) > grabStrength)
                {
                    interactable.Pickup(this);
                    heldInteractable = interactable;
                    isHolding = true;
                    StopHover();
                }
            }
        }

        private void DropInteractable()
        {
            heldInteractable.Drop(this);
            heldInteractable = null;
            isHolding = false;
        }

        /// <summary>
        /// Looks for overlapping interactable and with cast.
        /// Manages hover effects.
        /// </summary>
        private bool TryGetInteractable(out Interactable interactable)
        {
            if (TryFindClosestOverlappingInteractable(out interactable))
            {
                StartHover(interactable);
            }
            else if (TryFindFindInteractableWithCast(out interactable))
            {
                StartHover(interactable, true);
            }
            else
            {
                StopHover();
            }
            return interactable != null;
        }

        private bool TryFindClosestOverlappingInteractable(out Interactable interactable)
        {
            float closestDistance = float.MaxValue;
            interactable = null;

            Collider[] overlapping = Physics.OverlapSphere(overlapPosition.position, overlapRadius, overlapLayers);
            for (int i = 0; i < overlapping.Length; i++)
            {
                if (overlapping[i].TryGetComponent(out Interactable current) && !current.IsHeld)
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
            RaycastHit[] hits = Physics.SphereCastAll(castTransform.position, castRadius, castTransform.forward, castDistance);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.TryGetComponent(out Interactable current) && !current.IsHeld)
                {
                    interactable = current;
                    return true;
                }
            }

            return false;
        }

        private void StartHover(Interactable interactable, bool showLine = false)
        {
            if (hoveredInteractable)
            {
                hoveredInteractable.StopHover(this);   
            }
            interactable.StartHover(this);
            hoveredInteractable = interactable;

            if (showLine)
            {
                UpdateLine(castTransform.position, interactable.transform.position);
            }
            else
            {
                UpdateLine(Vector3.zero, Vector3.zero);
            }
        }

        private void StopHover()
        {
            if (hoveredInteractable)
            {
                hoveredInteractable.StopHover(this);
            }
            hoveredInteractable = null;
            UpdateLine(Vector3.zero, Vector3.zero);
        }

        private void UpdateLine(Vector3 startPos, Vector3 endPos)
        {
            castLine.SetPosition(0, startPos);
            castLine.SetPosition(1, endPos);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(overlapPosition.position, overlapRadius);
            Gizmos.DrawLine(castTransform.position, castTransform.position + castTransform.forward * castDistance);
        }
    }
}