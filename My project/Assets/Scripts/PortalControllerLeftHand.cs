using UnityEngine;

public class PortalControllerLeftHand : MonoBehaviour
{
    [Header("Portal Settings")]
    public GameObject portalPrefab;         // Assign your portal prefab
    public float maxRayDistance = 20f;      // How far the ray can go
    public float portalOffset = 0.05f;      // Offset so the portal doesn't clip into the wall
    public LayerMask wallLayer;             // Layer mask for walls

    [Header("VR References")]
    [Tooltip("Transform of the left controller (from your OVR/Meta rig)")]
    public Transform leftControllerTransform;

    private GameObject activePortal;

    void Start()
    {
        if (portalPrefab != null)
        {
            // Instantiate a single portal, keep disabled until used
            activePortal = Instantiate(portalPrefab);
            activePortal.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Portal prefab not assigned!");
        }
    }

    void Update()
    {
        if (activePortal == null || leftControllerTransform == null) return;

        // 1) Check if the user presses the left trigger this frame
        //    OVRInput.Button.PrimaryIndexTrigger = index trigger on the left controller 
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            // 2) Build a Ray from the left controller's position forward
            Vector3 origin = leftControllerTransform.position;
            Vector3 direction = leftControllerTransform.forward;
            Ray ray = new Ray(origin, direction);

            // 3) Raycast to see if we hit a wall
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, wallLayer))
            {
                // Double-check it's tagged "Wall" if you rely on tags
                if (hit.collider.CompareTag("Wall"))
                {
                    // Enable the portal if not active
                    if (!activePortal.activeSelf)
                        activePortal.SetActive(true);

                    // Position the portal with a small offset
                    Vector3 portalPos = hit.point + hit.normal * portalOffset;
                    activePortal.transform.position = portalPos;

                    // Orient the portal so it faces outward from the wall
                    Quaternion portalRot = Quaternion.LookRotation(-hit.normal, Vector3.up);
                    activePortal.transform.rotation = portalRot;
                }
                else
                {
                    // We hit something that's not a wall -> hide the portal
                    if (activePortal.activeSelf)
                        activePortal.SetActive(false);
                }
            }
            else
            {
                // Ray didn't hit anything -> hide the portal
                if (activePortal.activeSelf)
                    activePortal.SetActive(false);
            }
        }
    }
}
