using UnityEngine;

public static class RigidbodyExtensions
{
    const float MaxVelocityChange = 20f;
    const float MaxAngularVelocityChange = 10f;
    const float AngularVelocitySpeed = 50f;

    public static void SetVelocity(this Rigidbody rb, Vector3 targetPosition, float step = 1f)
    {
        Vector3 distance = targetPosition - rb.position;
        Vector3 targetVelocity = distance / Time.fixedDeltaTime;
        Vector3 velocity = targetVelocity * step;
        rb.velocity = Vector3.MoveTowards(rb.velocity, velocity, MaxVelocityChange);
    }

    public static void SetAngularVelocity(this Rigidbody rb, Quaternion targetRotation, float step = 1f)
    {
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(rb.rotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180)
            angle -= 360;

        if (angle != 0 && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            Vector3 angularTarget = angle * axis * step * AngularVelocitySpeed * Time.fixedDeltaTime;
            rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, MaxAngularVelocityChange);
        }
    }
}