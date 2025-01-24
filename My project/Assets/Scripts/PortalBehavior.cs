using UnityEngine;
using System.Collections;

public class PortalBehavior : MonoBehaviour
{
    [Header("Portal Setup")]
    public PortalBehavior otherPortal;

    [Tooltip("Radius within which the capsule is pulled by the portal.")]
    public float attractionRadius = 1f;

    [Tooltip("Strength of the pull towards the portal (acceleration).")]
    public float attractionForce = 30f;

    [Tooltip("When the capsule is this close to the portal, teleport occurs.")]
    public float teleportDistanceThreshold = 0.3f;

    [Tooltip("Small offset from the other portal’s position when you exit.")]
    public float exitOffset = 0.5f;

    [Tooltip("How strongly the capsule is kicked out of the exit portal.")]
    public float exitKickForce = 5f;

    [Tooltip("How long (seconds) after teleport until movement is re-enabled.")]
    public float regainControlDelay = 0.5f;

    [Tooltip("How long (seconds) both portals stop pulling after a teleport.")]
    public float portalAttractionCooldown = 1f;

    [Header("References")]
    public Transform capsuleTransform;
    public CapsuleController capsuleController;

    private Rigidbody capsuleRb;
    private bool isPulling = false;
    private bool portalDisabled = false;  // If true, this portal won't attract right now

    void Start()
    {
        // Cache the capsule’s rigidbody
        if (capsuleTransform != null)
        {
            capsuleRb = capsuleTransform.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogWarning(name + ": capsuleTransform is not assigned!");
        }
    }

    void Update()
    {
        if (!capsuleTransform || !capsuleController || !capsuleRb) return;
        if (portalDisabled) return;  // This portal is temporarily disabled

        float distance = Vector3.Distance(capsuleTransform.position, transform.position);

        if (distance <= attractionRadius)
        {
            // If not already pulling, disable player movement
            if (!isPulling)
            {
                isPulling = true;
                capsuleController.canMove = false;
            }

            // If close enough, teleport
            if (distance <= teleportDistanceThreshold)
            {
                TeleportCapsule();
            }
            else
            {
                // Apply an accelerating force toward the portal
                Vector3 dir = (transform.position - capsuleRb.position).normalized;
                capsuleRb.AddForce(dir * attractionForce, ForceMode.Acceleration);
            }
        }
        else
        {
            // If we left the radius, stop pulling
            if (isPulling)
            {
                isPulling = false;
                capsuleController.canMove = true;
            }
        }
    }

    void TeleportCapsule()
    {
        if (!otherPortal) return;

        // Place the capsule at the other portal, offset behind or in front
        Vector3 exitPos = otherPortal.transform.position
                          - otherPortal.transform.forward * exitOffset;
        capsuleRb.position = exitPos;

        // Zero velocities
        capsuleRb.velocity = Vector3.zero;
        capsuleRb.angularVelocity = Vector3.zero;

        // Kick the capsule away from the other portal
        Vector3 kickDir = -otherPortal.transform.forward;
        capsuleRb.AddForce(kickDir * exitKickForce, ForceMode.VelocityChange);

        // Let the other portal handle re-enabling movement
        otherPortal.StartCoroutine(otherPortal.RegainControlRoutine(capsuleController));

        // Stop pulling on this portal
        isPulling = false;

        // Disable attraction on BOTH portals to avoid immediate re-entrance
        StartCoroutine(DisableAttraction(portalAttractionCooldown));
        otherPortal.StartCoroutine(otherPortal.DisableAttraction(otherPortal.portalAttractionCooldown));
    }

    public IEnumerator RegainControlRoutine(CapsuleController cc)
    {
        yield return new WaitForSeconds(regainControlDelay);
        cc.canMove = true;
    }

    public IEnumerator DisableAttraction(float duration)
    {
        // Turn off attraction
        portalDisabled = true;
        // If we were pulling, reset the state
        if (isPulling)
        {
            isPulling = false;
            capsuleController.canMove = true;
        }

        yield return new WaitForSeconds(duration);

        // Re-enable attraction
        portalDisabled = false;
    }
}
