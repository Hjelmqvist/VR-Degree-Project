using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJointMethods : MonoBehaviour
{
    ConfigurableJoint joint;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
    }

    public void LockXMotion()
    {
        joint.xMotion = ConfigurableJointMotion.Locked;
    }

    public void LimitXMotion()
    {
        joint.xMotion = ConfigurableJointMotion.Limited;
    }

    public void LockYMotion()
    {
        joint.yMotion = ConfigurableJointMotion.Locked;
    }

    public void LimitYMotion()
    {
        joint.yMotion = ConfigurableJointMotion.Limited;
    }

    public void LockZMotion()
    {
        joint.zMotion = ConfigurableJointMotion.Locked;
    }

    public void LimitZMotion()
    {
        joint.zMotion = ConfigurableJointMotion.Limited;
    }
}