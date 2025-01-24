using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Header("Portal Prefabs")]
    public GameObject portalPrefabLeft;
    public GameObject portalPrefabRight;

    [Header("Controller References")]
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    [Header("Settings")]
    public float maxRayDistance = 20f;
    public float portalOffset = 0.05f;
    public LayerMask wallLayer;

    // Internal references to the two portals
    private GameObject activePortalLeft;
    private GameObject activePortalRight;

    void Start()
    {
        // Instantiate both portals once, keep them disabled
        if (portalPrefabLeft != null)
        {
            activePortalLeft = Instantiate(portalPrefabLeft);
            activePortalLeft.SetActive(false);
        }
        if (portalPrefabRight != null)
        {
            activePortalRight = Instantiate(portalPrefabRight);
            activePortalRight.SetActive(false);
        }
    }

    void Update()
    {
        // --- LEFT CONTROLLER PORTAL ---
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            TrySpawnPortal(
                leftControllerTransform,
                ref activePortalLeft  // pass by reference so we can modify it 
            );
        }

        // --- RIGHT CONTROLLER PORTAL ---
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            TrySpawnPortal(
                rightControllerTransform,
                ref activePortalRight
            );
        }
    }

    // This method does the raycast logic; "ref" allows it to modify the passed-in GameObject reference if needed
    private void TrySpawnPortal(Transform controllerTransform, ref GameObject portalInstance)
    {
        // 1) Ray from the controller’s position & forward
        Vector3 origin = controllerTransform.position;
        Vector3 direction = controllerTransform.forward;
        Ray ray = new Ray(origin, direction);

        // 2) Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, wallLayer))
        {
            // Check if we hit a wall
            if (hit.collider.CompareTag("Wall"))
            {
                // Enable portal if not active
                if (!portalInstance.activeSelf)
                    portalInstance.SetActive(true);

                // Position with offset
                portalInstance.transform.position = hit.point + hit.normal * portalOffset;

                // Orient so it faces outward
                Quaternion rot = Quaternion.LookRotation(-hit.normal, Vector3.up);
                portalInstance.transform.rotation = rot;
            }
            else
            {
                // Hit something else -> disable portal
                if (portalInstance.activeSelf)
                    portalInstance.SetActive(false);
            }
        }
        else
        {
            // No hit -> disable portal
            if (portalInstance.activeSelf)
                portalInstance.SetActive(false);
        }
    }
}

