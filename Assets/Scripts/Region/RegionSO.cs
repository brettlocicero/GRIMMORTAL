using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegionSO", menuName = "Scriptable Objects/RegionSO")]
public class RegionSO : ScriptableObject
{
    [SerializeField] string regionName;
    [SerializeField] RoomObject[] rooms;
    [SerializeField] HallwayObject[] hallways;

    public RoomObject RandomRoom => rooms[Random.Range(0, rooms.Length)];
    public HallwayObject RandomHallway => hallways[Random.Range(0, hallways.Length)];

    [Header("Landmarks")]
    [SerializeField] bool useLandmarks = true;
    [SerializeField] Landmark[] landmarks;

    [Header("Ambience")]
    [SerializeField] Material skybox;
    [SerializeField] Color fogColor;
    [SerializeField] Color sunColor;

    public string RegionName => regionName;
    public int RoomCount => rooms.Length;

    public RoomObject SpawnRoom(int roomIndex, Vector3 position, System.Random rng)
    {
        RoomObject prefab = rooms[rng.Next(rooms.Length)];
        return Instantiate(prefab, position, Quaternion.identity);
    }

    public void SpawnLandmarks(Vector3 center, Transform parent, System.Random rng)
    {
        if (!useLandmarks) return;

        List<(Vector3 position, float radius)> placed = new();

        foreach (Landmark landmark in landmarks)
            landmark.Spawn(center, parent, rng, placed);
    }

    float RandomRange(System.Random rng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }

    public void SetRegionAmbience()
    {
        RenderSettings.skybox = skybox;
        RenderSettings.fogColor = fogColor;
        RenderSettings.sun.color = sunColor;
    }
}