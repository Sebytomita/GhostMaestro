using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportOnGrab : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The XR Rig or XR Origin transform that should be teleported.")]
    public Transform xrRig;

    [Tooltip("Where the XR Rig will be moved to (place an empty GameObject at desired seat position).")]
    public Transform teleportTarget;

    [Header("Options")]
    [Tooltip("Teleport when the chair is selected (grabbed).")]
    public bool teleportOnSelect = true;

    [Tooltip("Teleport when the interactable receives an Activate event (primary button).")]
    public bool teleportOnActivate = false;

    [Tooltip("Match the teleport target rotation to the XR rig's rotation.")]
    public bool matchTargetRotation = true;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (_interactable == null)
            Debug.LogError($"TeleportOnGrab requires an XRBaseInteractable on the same GameObject ({name}).");
    }

    void OnEnable()
    {
        if (_interactable == null) return;
        if (teleportOnSelect) _interactable.selectEntered.AddListener(OnSelectEntered);
        if (teleportOnActivate) _interactable.activated.AddListener(OnActivated);
    }

    void OnDisable()
    {
        if (_interactable == null) return;
        if (teleportOnSelect) _interactable.selectEntered.RemoveListener(OnSelectEntered);
        if (teleportOnActivate) _interactable.activated.RemoveListener(OnActivated);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        TeleportRig();
    }

    void OnActivated(ActivateEventArgs args)
    {
        TeleportRig();
    }

    // Public method so you can also call it from the XRGrabInteractable's UnityEvent in the inspector
    public void TeleportRig()
    {
        if (xrRig == null || teleportTarget == null)
        {
            Debug.LogWarning("TeleportOnGrab: xrRig or teleportTarget not assigned.");
            return;
        }

        // Move the rig
        xrRig.position = teleportTarget.position;

        if (matchTargetRotation)
            xrRig.rotation = teleportTarget.rotation;
    }
}
