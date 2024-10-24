using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;  // The camera transform that the UI follows

    void LateUpdate()
    {
        // Set the UI's position to the camera's position (if needed)
        transform.position = cameraTransform.position;

        // Follow the camera's rotation but lock the Z axis to prevent tilting
        Vector3 cameraRotation = cameraTransform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);
    }
}
