using UnityEngine;
using UnityEngine.InputSystem;

public class InteractKeyboardToggle : MonoBehaviour
{
    [Header("Target")]
    public ObjectInteractUI target;           
    public Transform playerHead;              
    [Header("Rules")]
    public float maxDistance = 3f;    

    private void Awake()
    {
        if (!target) target = GetComponent<ObjectInteractUI>();
        if (!playerHead && Camera.main) playerHead = Camera.main.transform;
    }

    private void Update()
    {
        
        if (Keyboard.current == null) return;
        if (!Keyboard.current.xKey.wasPressedThisFrame) return;
        if (!target) return;

        if (playerHead && Vector3.Distance(playerHead.position, target.transform.position) > maxDistance)
            return;

        target.ToggleDetail();
    }
}
