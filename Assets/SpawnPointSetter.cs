using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnPointSetter : MonoBehaviour
{
    public LayerMask spawnableSurfaces;
    public Transform pointerOrigin; // Left controller
    public float maxDistance = 10f;
    // public KeyCode setKey = KeyCode.JoystickButton1; // Typically Button 1 (e.g., X or A)
    // Input.GetKeyDown(setKey) inside if statement
    public InputActionProperty leftTriggerAction;

    public static Vector3? spawnPoint = null; // Global access

    private void Update()
    {
        if (leftTriggerAction.action.ReadValue<float>() > 0.75f)
        {
            Debug.Log("Left trigger was pulled.");
            Ray ray = new Ray(pointerOrigin.position, pointerOrigin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, spawnableSurfaces))
            {
                spawnPoint = hit.point;
                Debug.Log("Spawn point set to: " + hit.point);
                // Optional: create a visual marker at that point
            }
        }
    }
}

