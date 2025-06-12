using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandRayScalerRotator : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable

{
    private List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor> grabInteractors = new List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>();
    private float initialDistance;
    private Vector3 initialScale;
    private Quaternion initialRotationOffset;

    [SerializeField] private Transform scaleTarget;

    private Transform firstInteractorTransform => grabInteractors[0].transform;
    private Transform secondInteractorTransform => grabInteractors[1].transform;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Does this work???");
        base.OnSelectEntered(args);
        grabInteractors.Add(args.interactorObject);

        if (grabInteractors.Count == 2)
        {
            Debug.Log("Understands theres 2 interactors");
            initialScale = scaleTarget.localScale;

            initialDistance = Vector3.Distance(firstInteractorTransform.position, secondInteractorTransform.position);

            Vector3 direction = secondInteractorTransform.position - firstInteractorTransform.position;
            initialRotationOffset = Quaternion.Inverse(Quaternion.LookRotation(direction)) * transform.rotation;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        grabInteractors.Remove(args.interactorObject);
    }

    void Update()
    {
        if (grabInteractors.Count == 2)
        {
            float currentDistance = Vector3.Distance(firstInteractorTransform.position, secondInteractorTransform.position);
            float scaleFactor = Mathf.Clamp(currentDistance / initialDistance, 0.1f, 5f);
            //scaleTarget.localScale = initialScale * scaleFactor;
            scaleTarget.localScale = initialScale * scaleFactor;
            Debug.Log($"Scaling with factor {scaleFactor}");
            Debug.Log($"New Scale: {scaleTarget.localScale}");

        }
    }

    public override bool IsSelectableBy(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        // Allow up to 2 interactors (even if they are both ray-based)
        return base.IsSelectableBy(interactor) &&
           (grabInteractors.Count < 2 || grabInteractors.Contains(interactor));
    }

    void Start() {
        if (scaleTarget == null) scaleTarget = transform; // fallback if not assigned
    }   
}
