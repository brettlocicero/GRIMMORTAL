using UnityEngine;

public class RegionGenerator : MonoBehaviour
{
    [SerializeField] RegionSO testRegion;

    RegionSO currentRegion;
    RoomObject currentRoom;
    int roomIndex = 0;

    void Start()
    {
        GenerateRegion(testRegion);
    }

    public void GenerateRegion(RegionSO region)
    {
        currentRegion = region;
        roomIndex = 0;

        SpawnRoom();
    }

    public void SpawnNextRoom()
    {
        roomIndex++;

        if (roomIndex < currentRegion.RoomCount)
            SpawnRoom();
    }

    void SpawnRoom()
    {
        if (currentRoom)
            Destroy(currentRoom.gameObject);

        currentRoom = currentRegion.SpawnRoom(roomIndex, transform.position);
    }
}