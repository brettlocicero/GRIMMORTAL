using UnityEngine;
using UnityEngine.InputSystem;

public class StationObject : MonoBehaviour, IInteractable
{
    public string Prompt => "Forge Weapons | Upgrade the Forge";

    [SerializeField] GameObject stationUI;

    PlayerController player;
    bool isOpen;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        stationUI.SetActive(false);
    }

    void Update()
    {
        if (isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void Interact()
    {
        if (!isOpen)
            SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }

    void SetOpen(bool open)
    {
        isOpen = open;
        stationUI.SetActive(open);

        player.MovementLocked = open;
        player.SensitivityLocked = open;

        if (open)
            CursorManager.UnlockCursor();
        else
            CursorManager.LockCursor();
    }
}