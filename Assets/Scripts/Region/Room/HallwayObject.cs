using UnityEngine;

public class HallwayObject : MonoBehaviour
{
    [SerializeField] Transform entry;
    [SerializeField] Transform exit;
    [SerializeField] DoorObject entryDoor;
    [SerializeField] DoorObject exitDoor;
    [SerializeField] BoxCollider[] bounds;

    public BoxCollider[] Bounds => bounds;
    public Transform Exit => exit;

    public void SnapTo(Transform connector)
    {
        transform.rotation = connector.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(entry.rotation) * transform.rotation;
        transform.position += connector.position - entry.position;
    }
    
    public void Init(RoomObject entryRoom, RoomObject exitRoom)
    {
        entryDoor.Init(entryRoom);
        exitDoor.Init(exitRoom);
    }
}
