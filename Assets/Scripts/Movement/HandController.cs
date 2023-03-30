using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandController : MonoBehaviour
{
    [SerializeField] Transform handInput;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] LayerMask blockingLayers;
    [SerializeField] float maxVelocity = 10f;

    Rigidbody rb;
    SkinnedMeshRenderer meshRenderer;

    public bool IsControlling = true;

    public Transform Input => handInput;
    public Rigidbody RB => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void FixedUpdate()
    {
        UpdateCollider();

        if (IsControlling)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 positionDelta = handInput.position - cameraPosition;
            if (Physics.Raycast(cameraPosition, positionDelta, out RaycastHit hit, positionDelta.magnitude, blockingLayers))
            {
                rb.SetVelocity(hit.point);
            }
            else
            {
                rb.SetVelocity(handInput.position);
            }

            rb.angularVelocity = Vector3.zero;
            transform.rotation = handInput.rotation;
        }
    }

    private void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        meshCollider.sharedMesh = colliderMesh;
    }
}