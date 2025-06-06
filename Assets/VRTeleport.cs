using UnityEngine;
using UnityEngine.InputSystem;

public class VRTeleport : MonoBehaviour
{
    public Transform xrOrigin;             
    public Transform headCamera;        
    public Transform controller;           
    public LayerMask teleportLayer;        
    public GameObject teleportMarker;    
    public float maxDistance = 10f;     
    public InputActionProperty teleportAction;

    private Vector3 destination;
    private bool validTeleport;

    void Start()
    {
        teleportAction.action.Enable();
        Debug.Log("Started");

    }
    
    void OnEnable()
    {
        teleportAction.action.Enable();
    }

    void OnDisable()
    {
        teleportAction.action.Disable();
    }

    void Update()
    {

        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        validTeleport = false;

        if (Physics.Raycast(ray, out hit, maxDistance, teleportLayer))
        {
            destination = hit.point;

            Debug.Log($"destination: {destination}");

            validTeleport = true;

            

                teleportMarker.SetActive(true);
                teleportMarker.transform.position = destination;


            if (teleportAction.action.WasPressedThisFrame())
            {
                Debug.Log("pressed");
                Teleport();
                
            }
        }
        else
        {
            if (teleportMarker != null)
            {
                teleportMarker.SetActive(false);
            }
        }
    }

    void Teleport()
    {
        Vector3 offset = xrOrigin.position - headCamera.position;
        offset.y = 0f; 
        xrOrigin.position = destination + offset;
        Debug.Log("Teleport Complete");
    }
}