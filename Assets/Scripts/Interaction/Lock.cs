using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] float interactCooldown = 1f;
    [SerializeField] bool relockable = false;
    [SerializeField] Collider[] collidersToIgnore;
    [SerializeField] UnityEvent OnLocked;
    [SerializeField] UnityEvent OnUnlocked;

    float lastInteractTime = float.MinValue;
    Key placedKey;

    bool IsSearching => placedKey == null && Time.time >= lastInteractTime + interactCooldown;
    bool CanExit => placedKey != null && Time.time >= lastInteractTime + interactCooldown;

    private void OnTriggerStay(Collider other)
    {
        if (IsSearching && other.TryGetComponent(out Key key))
        {
            SetIgnoreCollision(key, true);
            lastInteractTime = Time.time;
            placedKey = key;
            bool correctKey = key.Equals(this.key);
            key.PlaceInLock(this, correctKey);
            key.OnPickup.AddListener(Key_OnPickup);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CanExit && other.TryGetComponent(out Key key))
        {
            if (key.Equals(placedKey))
            {
                Key_OnPickup();
            }
        }
    }

    private void Key_OnPickup()
    {
        SetIgnoreCollision(placedKey, false);
        placedKey.OnPickup.RemoveListener(Key_OnPickup);
        if (placedKey.Equals(key))
        {
            ToggleLock(true);
        }
        placedKey = null;
        lastInteractTime = Time.time;
    }

    private void SetIgnoreCollision(Key key, bool ignore)
    {
        for (int i = 0; i < collidersToIgnore.Length; i++)
        {
            Physics.IgnoreCollision(collidersToIgnore[i], key.Collider, ignore);
        }
    }

    public void ToggleLock(bool locked)
    {
        if (locked)
        {
            if (relockable)
            {
                OnLocked.Invoke();
            }
        }
        else
        {
            OnUnlocked.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (key)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, key.transform.position);
        }
    }
}