using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class RayColorChanger : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public LineRenderer lineRenderer;

    public Material defaultMaterial;
    public Material selectedMaterial;

    private void OnEnable()
    {
        rayInteractor.selectEntered.AddListener(OnSelect);
        rayInteractor.selectExited.AddListener(OnDeselect);
    }

    private void OnDisable()
    {
        rayInteractor.selectEntered.RemoveListener(OnSelect);
        rayInteractor.selectExited.RemoveListener(OnDeselect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        if (lineRenderer != null && selectedMaterial != null)
            lineRenderer.material = selectedMaterial;
    }

    private void OnDeselect(SelectExitEventArgs args)
    {
        if (lineRenderer != null && defaultMaterial != null)
            lineRenderer.material = defaultMaterial;
    }
}