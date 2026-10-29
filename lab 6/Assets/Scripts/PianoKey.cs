using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PianoKey : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip keySound;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    void Start()
    {
        // Use existing AudioSource, not creating a new one
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = keySound;

        // XR Interactable
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

        if (interactable != null)
        {
            // For Poke Interactor
            interactable.hoverEntered.AddListener(OnSelect);
        }
    }

    public void OnSelect(HoverEnterEventArgs args)
    {
        Debug.Log("KEY PRESSED: " + gameObject.name);
        PlaySound();
    }

    public void PlaySound()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogError("Missing AudioSource or AudioClip!");
            return;
        }
        audioSource.Play();
    }
}
