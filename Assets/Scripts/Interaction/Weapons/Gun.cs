using Hjelmqvist.VR;
using UnityEngine;
using UnityEngine.Events;

public class Gun : Interactable
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform projectileSpawn;
    [Tooltip("Number of shots per second")]
    [SerializeField] float fireRate = 1f;

    float lastShotTime = float.MinValue;

    public UnityEvent OnShoot;

    protected virtual bool CanShoot => projectilePrefab && Time.time >= lastShotTime + fireRate;

    public override void StartInteract()
    {
        base.StartInteract();
        if (CanShoot)
        {
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
        IgnoreCollision(colliders, projectile.GetComponent<Collider>(), true);
        lastShotTime = Time.time;
        OnShoot.Invoke();
    }
}