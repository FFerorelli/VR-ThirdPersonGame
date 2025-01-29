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
    [Tooltip("Assign a LayerMask that includes both walls and ground.")]
    public LayerMask wallLayer;

    // Internal references to the two portals
    private GameObject activePortalLeft;
    private GameObject activePortalRight;

    void Start()
    {
        // Use the existing scene portals instead of instantiating new ones
        if (portalPrefabLeft != null)
        {
            activePortalLeft = portalPrefabLeft;
            activePortalLeft.SetActive(false);
        }
        if (portalPrefabRight != null)
        {
            activePortalRight = portalPrefabRight;
            activePortalRight.SetActive(false);
        }
    }

    void Update()
    {
        // LEFT CONTROLLER
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            TrySpawnPortal(leftControllerTransform, ref activePortalLeft);
        }

        // RIGHT CONTROLLER
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            TrySpawnPortal(rightControllerTransform, ref activePortalRight);
        }
    }

    private void TrySpawnPortal(Transform controllerTransform, ref GameObject portalInstance)
    {
        if (portalInstance == null) return;

        // 1) Ray from the controller’s position & forward
        Vector3 origin = controllerTransform.position;
        Vector3 direction = controllerTransform.forward;
        Ray ray = new Ray(origin, direction);

        // 2) Raycast using the LayerMask that includes walls and ground
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, wallLayer))
        {
            // We hit something in the layer mask (no tag check needed)
            if (!portalInstance.activeSelf)
                portalInstance.SetActive(true);

            // Position with a small offset so it doesn’t clip into the surface
            Vector3 spawnPos = hit.point + (hit.normal * portalOffset);
            portalInstance.transform.position = spawnPos;

            // Orient so the portal faces outward from the surface
            Quaternion rot = Quaternion.LookRotation(-hit.normal, Vector3.up);
            portalInstance.transform.rotation = rot;
        }
        else
        {
            // Ray didn't hit anything in that layer -> disable portal
            if (portalInstance.activeSelf)
                portalInstance.SetActive(false);
        }
    }
}
