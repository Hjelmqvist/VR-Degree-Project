using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] float delay = 0f;
    [SerializeField] float distanceThreshold = 0.1f;
    [SerializeField] float searchDelayAfterPickup = 0.2f;
    [SerializeField] UnityEvent OnLocked;
    [SerializeField] UnityEvent OnUnlocked;
    Key placedKey;

    bool isSearching = true;

    private void Update()
    {
        if (isSearching)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, distanceThreshold);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out Key key))
                {
                    EnterKey(key);
                    break;
                }
            }
        }
        else if (Vector3.Distance(transform.position, placedKey.transform.position) > distanceThreshold * 2)
        {
            isSearching = true;
            placedKey = null;
        }
    }

    private void EnterKey(Key key)
    {
        isSearching = false;
        placedKey = key;
        bool correctKey = key.Equals(this.key);
        key.PlaceInLock(this, correctKey);
        key.OnPickup.AddListener(Key_OnPickup);
    }

    private void Key_OnPickup()
    {
        placedKey.OnPickup.RemoveListener(Key_OnPickup);

        if (placedKey.Equals(key))
        {
            ToggleLock(true);
        }

        StartCoroutine(SearchDelayCoroutine());

        IEnumerator SearchDelayCoroutine()
        {
            yield return new WaitForSeconds(searchDelayAfterPickup);
            isSearching = true;
            placedKey = null;
        }
    }

    public void ToggleLock(bool locked)
    {
        StartCoroutine(LockCoroutine());

        IEnumerator LockCoroutine()
        {
            yield return new WaitForSeconds(delay);
            if (locked)
            {
                OnLocked.Invoke();
            }
            else
            {
                OnUnlocked.Invoke();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceThreshold);
    }
}