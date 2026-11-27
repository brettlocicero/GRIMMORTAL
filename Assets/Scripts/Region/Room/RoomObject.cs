using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [Header("Connectors")]
    [SerializeField] Transform north;
    [SerializeField] Transform south;
    [SerializeField] Transform east;
    [SerializeField] Transform west;

    [Header("Spawning")]
    [SerializeField] Transform[] spawnPoints;

    [SerializeField] BoxCollider[] bounds;

    public BoxCollider[] Bounds => bounds;
    public bool Unlocked { get; set; }
    public Transform[] SpawnPoints => spawnPoints;

    public Transform[] GetConnectors()
    {
        return new Transform[] { north, south, east, west };
    }

    public void SnapTo(Transform connector)
    {
        Transform[] connectors = GetConnectors();
        List<Transform> available = new();

        foreach (Transform exit in connectors)
        {
            if (exit)
                available.Add(exit);
        }

        Transform entry = available[Random.Range(0, available.Count)];

        transform.rotation = connector.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(entry.rotation) * transform.rotation;
        transform.position += connector.position - entry.position;
    }

    public void SnapTo(Transform connector, Transform entry)
    {
        transform.rotation = connector.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(entry.rotation) * transform.rotation;
        transform.position += connector.position - entry.position;
    }
}