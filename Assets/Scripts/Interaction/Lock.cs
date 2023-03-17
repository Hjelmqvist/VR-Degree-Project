using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] float delay = 0f;
    [SerializeField] float distanceThreshold = 0.1f;
    [SerializeField] float interactDelay = 2f;
    [SerializeField] bool relockable = false;
    [SerializeField] UnityEvent OnLocked;
    [SerializeField] UnityEvent OnUnlocked;

    float lastInteractTime = float.MinValue;
    Key placedKey;

    bool IsSearching => placedKey == null && Time.time >= lastInteractTime + interactDelay;
    bool CanExit => placedKey != null && Time.time >= lastInteractTime + interactDelay;

    private void OnTriggerStay(Collider other)
    {
        if (IsSearching && other.TryGetComponent(out Key key))
        {
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
        placedKey.OnPickup.RemoveListener(Key_OnPickup);
        if (placedKey.Equals(key))
        {
            ToggleLock(true);
        }
        placedKey = null;
        lastInteractTime = Time.time;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceThreshold);
    }
}