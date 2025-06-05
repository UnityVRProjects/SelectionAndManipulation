using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SelectableObject : MonoBehaviour
{
    private void Awake()
    {
        //var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        var interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelect);
        interactable.selectExited.AddListener(OnDeselect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("Selected: " + gameObject.name);
        // ✅ New: Create a clone
        Vector3 spawnPos = SpawnPointSetter.spawnPoint ?? transform.position + Vector3.up * 0.2f;
        GameObject clone = Instantiate(gameObject, spawnPos, transform.rotation);

        // ✅ Remove this script from the clone to avoid recursive duplication
        Destroy(clone.GetComponent<SelectableObject>());

        // ✅ Ensure it has a Rigidbody and Grab Interactable
        if (!clone.TryGetComponent<Rigidbody>(out _))
            clone.AddComponent<Rigidbody>();
        if (!clone.TryGetComponent<XRGrabInteractable>(out _))
            clone.AddComponent<XRGrabInteractable>();

        // Optional visual feedback
        GetComponent<Renderer>().material.color = Color.green;
    }

    private void OnDeselect(SelectExitEventArgs args)
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
