using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Hjelmqvist.VR
{
    [RequireComponent(typeof(Rigidbody), typeof(SteamVR_Skeleton_Poser))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] Material hoverMaterial;
        [SerializeField] float timeToReachHand = 0.1f;
        [SerializeField] float travelSpeed = 0.2f;
        [SerializeField] float holdMagnitudeThreshold = 0.1f;
         
        [Space(10)]
        [SerializeField] bool usePoser = true;
        [SerializeField] float poseBlendTime = 0.1f;
        [SerializeField] float skeletonBlendTime = 0f;

        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;

        List<Hand> hoveringHands = new List<Hand>();
        Hand holdingHand;

        Rigidbody rb;
        SteamVR_Skeleton_Poser skeletonPoser;

        public bool IsHeld => holdingHand != null;

        protected virtual void Awake()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            skeletonPoser = GetComponent<SteamVR_Skeleton_Poser>();
        }

        private void UpdateHighlight()
        {
            if (hoveringHands.Count == 0) // Remove highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] currentMaterials = meshRenderers[i].materials;
                    Material[] materials = new Material[currentMaterials.Length - 1];
                    for (int j = 0; j < materials.Length; j++)
                    {
                        materials[j] = currentMaterials[j];
                    }
                    meshRenderers[i].materials = materials;
                }
                isHighlighting = false;
            }
            else if (!isHighlighting) // Add highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] currentMaterials = meshRenderers[i].materials;
                    Material[] materials = new Material[currentMaterials.Length + 1];
                    for (int j = 0; j < currentMaterials.Length; j++)
                    {
                        materials[j] = currentMaterials[j];
                    }
                    materials[materials.Length - 1] = hoverMaterial;
                    meshRenderers[i].materials = materials;
                }
                isHighlighting = true;
            }
        }

        public virtual void StartHover(Hand hand)
        {
            hoveringHands.Add(hand);
            UpdateHighlight();
        }

        public virtual void StopHover(Hand hand)
        {
            hoveringHands.Remove(hand);
            UpdateHighlight();
        }

        public virtual void Pickup(Hand hand)
        {
            if (usePoser)
            {
                hand.Skeleton.BlendToPoser(skeletonPoser, poseBlendTime);
            }

            rb.useGravity = false;
            holdingHand = hand;
        }

        public virtual void HeldFixedUpdate()
        {
            Vector3 targetPosition = TargetItemPosition();
            Vector3 distance = targetPosition - transform.position;
            Vector3 targetVelocity = distance / Time.fixedDeltaTime;

            if (distance.magnitude > holdMagnitudeThreshold)
            {
                targetVelocity *= travelSpeed;
            }

            rb.velocity = Vector3.MoveTowards(rb.velocity, targetVelocity, MaxVelocityChange);

            Quaternion targetRotation = Quaternion.RotateTowards(rb.rotation, TargetItemRotation(), MaxAngularVelocityChange);

            //if (GetUpdatedAttachedVelocities(out Vector3 velocityTarget, out Vector3 angularTarget))
            //{
            //    //rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, MaxVelocityChange);
            //    rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, MaxAngularVelocityChange);
                
            //}
        }

        public virtual void Drop(Hand hand)
        {
            if (usePoser)
            {
                hand.Skeleton.BlendToSkeleton(skeletonBlendTime);
            }
            
            rb.useGravity = true;
            holdingHand = null;
        }

        public virtual void Interact()
        {

        }


        // FROM STEAMVR HAND

        protected const float MaxVelocityChange = 20f;
        protected const float VelocityMagic = 10000f;
        protected const float MaxAngularVelocityChange = 10f;
        protected const float AngularVelocityMagic = 50f;

        public Vector3 TargetItemPosition()
        {
            if (!usePoser)
                return holdingHand.transform.position;
            Vector3 tp = holdingHand.transform.InverseTransformPoint(holdingHand.transform.TransformPoint(skeletonPoser.GetBlendedPose(holdingHand.Skeleton).position));
            return holdingHand.transform.TransformPoint(tp);
        }

        protected Quaternion TargetItemRotation()
        {
            if (!usePoser)
                return holdingHand.transform.rotation;
            Quaternion tr = Quaternion.Inverse(holdingHand.transform.rotation) * (holdingHand.transform.rotation * skeletonPoser.GetBlendedPose(holdingHand.Skeleton).rotation);
            return holdingHand.transform.rotation * tr;
        }

        protected bool GetUpdatedAttachedVelocities(out Vector3 velocityTarget, out Vector3 angularTarget)
        {
            bool realNumbers = false;

            float velocityMagic = VelocityMagic;
            float angularVelocityMagic = AngularVelocityMagic;

            Vector3 targetItemPosition = TargetItemPosition();
            Vector3 positionDelta = (targetItemPosition - rb.position);
            velocityTarget = (positionDelta * velocityMagic * Time.deltaTime);

            if (float.IsNaN(velocityTarget.x) == false && float.IsInfinity(velocityTarget.x) == false)
            {
                realNumbers = true;
            }
            else
                velocityTarget = Vector3.zero;

            Quaternion targetItemRotation = TargetItemRotation();
            Quaternion rotationDelta = targetItemRotation * Quaternion.Inverse(transform.rotation);

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0 && float.IsNaN(axis.x) == false && float.IsInfinity(axis.x) == false)
            {
                angularTarget = angle * axis * angularVelocityMagic * Time.deltaTime;
                realNumbers &= true;
            }
            else
                angularTarget = Vector3.zero;

            return realNumbers;
        }
    }
}