using UnityEngine;

[CreateAssetMenu(fileName = "RegionSO", menuName = "Scriptable Objects/RegionSO")]
public class RegionSO : ScriptableObject
{
    [SerializeField] string regionName;
    [SerializeField] RoomObject[] rooms;

    [Header("Ambience")]
    [SerializeField] Material skybox;
    [SerializeField] Color fogColor;
    [SerializeField] Color sunColor;

    public string RegionName => regionName;
    public int RoomCount => rooms.Length;

    public RoomObject SpawnRoom(int roomIndex, Vector3 position)
    {
        RoomObject prefab = rooms[roomIndex];
        return Instantiate(prefab, position, Quaternion.identity);
    }

    public void SetRegionAmbience()
    {
        RenderSettings.skybox = skybox;
        RenderSettings.fogColor = fogColor;
        RenderSettings.sun.color = sunColor;
    }
}