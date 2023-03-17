using UnityEngine;
using Valve.VR;

namespace Hjelmqvist.VR
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] SteamVR_Input_Sources handType = SteamVR_Input_Sources.LeftHand;
        [SerializeField, Range(0, 1)] float grabStrengthThreshold = 0.1f;
        [SerializeField] SteamVR_Action_Single squeezeAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
        [SerializeField] SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Interact");

        [Header("Finding Interactable"), Space(10)]
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

        [Header("Grabbing"), Space(10)]
        [SerializeField] float timeToReachHand = 0.1f;
        [SerializeField] AnimationCurve grabSpeed;

        bool isGrabbing = false;
        bool wasGrabbing = false;

        float holdStartTime;
        float holdTargetTime;

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
            wasGrabbing = isGrabbing;
            isGrabbing = squeezeAction.GetAxis(handType) > grabStrengthThreshold;

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

                if (wasGrabbing && !isGrabbing)
                {
                    DropInteractable(); // Drop object
                }
            }
        }

        private void FixedUpdate()
        {
            if (isHolding)
            {
                float step = grabSpeed.Evaluate(Mathf.InverseLerp(holdStartTime, holdTargetTime, Time.time));
                heldInteractable.HeldFixedUpdate(step);
            }
        }

        private void TryPickupInteractable()
        {
            if (TryGetInteractable(out Interactable interactable))
            {
                if (!wasGrabbing && isGrabbing)
                {
                    PickupInteractable(interactable);
                }
            }
        }

        public void PickupInteractable(Interactable interactable)
        {
            StopHover();
            interactable.Pickup(this);
            heldInteractable = interactable;
            isHolding = true;
            holdStartTime = Time.time;
            holdTargetTime = holdStartTime + timeToReachHand;
        }

        public void DropInteractable()
        {
            if (heldInteractable)
            {
                heldInteractable.Drop(this);
                heldInteractable = null;
                isHolding = false;
            }
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
                if (overlapping[i].TryGetComponent(out Interactable current) && current.CanBeGrabbed)
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
                if (hits[i].transform.TryGetComponent(out Interactable current) && current.CanBeGrabbed)
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