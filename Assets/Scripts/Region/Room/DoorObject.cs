using UnityEngine;

public class DoorObject : MonoBehaviour, IInteractable
{
    RoomObject room;

    public void Init(RoomObject room)
    {
        this.room = room;
    }

    public void Interact()
    {
        room.Unlocked = true;
        Destroy(gameObject);
    }
}