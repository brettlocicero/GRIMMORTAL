using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionGenerator : MonoBehaviour
{
    public static RegionGenerator Instance;

    [SerializeField] Animator playerTransitionAnimator;
    [SerializeField] GameObject hubObjects;
    // [SerializeField] RoundManager roundManager;

    [Header("Layout")]
    [SerializeField] int roomCount = 8;
    [SerializeField] LayerMask layoutMask;
    [SerializeField] float generationDelay = 0.05f;

    List<RoomObject> rooms = new();
    List<HallwayObject> hallways = new();
    AudioSource audioSource;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void GenerateRegion(RegionSO region)
    {
        StartCoroutine(GenerateRegionWorker(region));
    }

    IEnumerator GenerateRegionWorker(RegionSO region)
    {
        playerTransitionAnimator.SetTrigger("RegionEnter");
        audioSource.Play();

        yield return new WaitForSeconds(1f);

        hubObjects.SetActive(false);
        region.SetRegionAmbience();

        yield return GenerateLayout(region);

        // roundManager.StartRounds();
    }

    IEnumerator GenerateLayout(RegionSO region)
    {
        foreach (RoomObject room in rooms)
            Destroy(room.gameObject);

        foreach (HallwayObject hallway in hallways)
            Destroy(hallway.gameObject);

        rooms.Clear();
        hallways.Clear();

        yield return null;

        RoomObject start = Instantiate(region.RandomRoom, transform.position, transform.rotation, transform);
        start.Unlocked = true;

        rooms.Add(start);

        List<RoomObject> openRooms = new() { start };

        while (rooms.Count < roomCount && openRooms.Count > 0)
        {
            RoomObject parent = openRooms[Random.Range(0, openRooms.Count)];
            openRooms.Remove(parent);

            foreach (Transform connector in parent.GetConnectors())
            {
                if (!connector || rooms.Count >= roomCount)
                    continue;

                if (TryAddRoom(region, parent, connector, out RoomObject room))
                {
                    rooms.Add(room);
                    openRooms.Add(room);
                }

                yield return new WaitForSeconds(generationDelay);
            }
        }
    }

    bool TryAddRoom(RegionSO region, RoomObject parent, Transform connector, out RoomObject room)
    {
        HallwayObject hallway = Instantiate(region.RandomHallway, transform);
        hallway.SnapTo(connector);

        room = Instantiate(region.RandomRoom, transform);

        Transform[] connectors = room.GetConnectors();
        List<Transform> available = new();

        foreach (Transform exit in connectors)
        {
            if (exit)
                available.Add(exit);
        }

        if (available.Count == 0)
        {
            Destroy(hallway.gameObject);
            Destroy(room.gameObject);

            room = null;
            return false;
        }

        Transform entry = available[Random.Range(0, available.Count)];

        room.SnapTo(hallway.Exit, entry);

        Physics.SyncTransforms();

        bool blocked = HasOverlap(hallway.Bounds, parent.transform, hallway.transform, room.transform) ||
            HasOverlap(room.Bounds, parent.transform, room.transform, hallway.transform);

        if (blocked)
        {
            Destroy(hallway.gameObject);
            Destroy(room.gameObject);

            room = null;
            return false;
        }

        hallway.Init(parent, room);
        hallways.Add(hallway);
        return true;
    }

    public Transform GetRandomSpawnPoint()
    {
        List<Transform> points = new();

        foreach (RoomObject room in rooms)
        {
            if (room.Unlocked)
                points.AddRange(room.SpawnPoints);
        }

        return points[Random.Range(0, points.Count)];
    }

    bool HasOverlap(BoxCollider[] bounds, params Transform[] ignore)
    {
        Physics.SyncTransforms();

        int mask = layoutMask.value == 0 ? ~0 : layoutMask.value;

        foreach (BoxCollider bound in bounds)
        {
            Collider[] hits = Physics.OverlapBox(
                bound.transform.TransformPoint(bound.center),
                Vector3.Scale(bound.size, bound.transform.lossyScale) / 2f,
                bound.transform.rotation,
                mask,
                QueryTriggerInteraction.Collide
            );

            foreach (Collider hit in hits)
            {
                bool ignored = false;

                foreach (Transform item in ignore)
                {
                    if (hit.transform == item || hit.transform.IsChildOf(item))
                    {
                        ignored = true;
                        break;
                    }
                }

                if (!ignored && IsLayoutObject(hit))
                    return true;
            }
        }

        return false;
    }

    bool IsLayoutObject(Collider hit)
    {
        RoomObject room = hit.GetComponentInParent<RoomObject>();
        HallwayObject hallway = hit.GetComponentInParent<HallwayObject>();

        return rooms.Contains(room) || hallways.Contains(hallway);
    }
}
