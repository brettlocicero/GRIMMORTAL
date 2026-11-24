using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InteractionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float interactionRange = 4f;
    [SerializeField] LayerMask interactionLayerMask;

    [Header("UI")]
    [SerializeField] TMP_Text hoverText;

    Transform cam;
    IDisposable buttonListener;
    bool usingGamepad;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void OnEnable()
    {
        buttonListener = InputSystem.onAnyButtonPress.Call(control => usingGamepad = control.device is Gamepad);
    }

    void OnDisable()
    {
        buttonListener.Dispose();
    }

    void Update()
    {
        IInteractable interactable = GetInteractable();

        hoverText.gameObject.SetActive(interactable != null);

        if (interactable != null)
        {
            string group = usingGamepad ? "Gamepad" : "Keyboard&Mouse";
            string key = InputManager.Actions.Player.Interact.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions, group: group);
            hoverText.text = $"<b><color=red>[{key}]</color></b> {interactable.Prompt}";

            if (InputManager.Actions.Player.Interact.WasPressedThisFrame())
                interactable.Interact();
        }
    }

    IInteractable GetInteractable()
    {
        IInteractable interactable = null;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactionRange, interactionLayerMask))
            hit.collider.TryGetComponent(out interactable);

        return interactable;
    }
}