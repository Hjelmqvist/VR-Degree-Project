using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToggleControllers : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean toggleControllersAction = SteamVR_Input.GetBooleanAction("ToggleControllers");
    Player player;

    bool controllersOn = false;

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (toggleControllersAction.stateDown)
        {
            controllersOn = !controllersOn;
            if (controllersOn)
            {
                foreach (Hand hand in player.hands)
                {
                    hand.ShowController();
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithController);
                }
            }
            else
            {
                foreach (Hand hand in player.hands)
                {
                    hand.HideController();
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithoutController);
                }
            }
        }
    }
}
