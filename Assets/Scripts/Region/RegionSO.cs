using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegionSO", menuName = "Scriptable Objects/RegionSO")]
public class RegionSO : ScriptableObject
{
    [SerializeField] string regionName;
    [SerializeField] RoomObject[] rooms;
    [SerializeField] HallwayObject[] hallways;

    [Header("Ambience")]
    [SerializeField] Material skybox;
    [SerializeField] Color fogColor;
    [SerializeField] Color sunColor;

    public string RegionName => regionName;

    public RoomObject RandomRoom => rooms[Random.Range(0, rooms.Length)];
    public HallwayObject RandomHallway => hallways[Random.Range(0, hallways.Length)];


    public void SetRegionAmbience()
    {
        RenderSettings.skybox = skybox;
        RenderSettings.fogColor = fogColor;
        RenderSettings.sun.color = sunColor;
    }
}