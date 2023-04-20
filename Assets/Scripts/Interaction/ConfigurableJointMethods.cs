using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJointMethods : MonoBehaviour
{
    ConfigurableJoint joint;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
    }
    
    public void LockZMotion()
    {
        joint.zMotion = ConfigurableJointMotion.Locked;
    }

    public void UnlockZMotions()
    {
        joint.zMotion = ConfigurableJointMotion.Free;
    }
}