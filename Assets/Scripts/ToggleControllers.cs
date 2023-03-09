using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToggleControllers : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean toggleControllersAction = SteamVR_Input.GetBooleanAction("ToggleControllers");
    [SerializeField] RenderModel[] hands;

    bool controllersOn = false;

    private void Update()
    {
        if (toggleControllersAction.stateDown)
        {
            controllersOn = !controllersOn;
            if (controllersOn)
            {
                foreach (var hand in hands)
                {
                    hand.SetControllerVisibility(true);
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithController, 0.1f);
                }
            }
            else
            {
                foreach (var hand in hands)
                {
                    hand.SetControllerVisibility(false);
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithoutController, 0.1f);
                }
            }
        }
    }
}