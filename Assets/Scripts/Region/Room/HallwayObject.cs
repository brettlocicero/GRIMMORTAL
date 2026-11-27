using UnityEngine;

public class HallwayObject : MonoBehaviour
{
    [SerializeField] Transform entry;
    [SerializeField] Transform exit;
    [SerializeField] DoorObject entryDoor;
    [SerializeField] DoorObject exitDoor;

    public Transform Exit => exit;

    public void SnapTo(Transform connector)
    {
        transform.rotation = connector.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(entry.rotation) * transform.rotation;
        transform.position += connector.position - entry.position;
    }

    public void Init(RoomObject room)
    {
        entryDoor.Init(room);
        exitDoor.Init(room);
    }
}