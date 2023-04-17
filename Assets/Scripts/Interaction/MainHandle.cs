using Hjelmqvist.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for interactables where two hands can be used.
/// This acts as the main hand and a Handle is used for the secondary hand.
/// </summary>
public class MainHandle : Interactable
{
    [SerializeField] Handle secondaryHandle;

    public override void HeldFixedUpdate(float step)
    {
        if (secondaryHandle.IsGrabbed)
        {

        }
        else
        {

        }
    }
}
