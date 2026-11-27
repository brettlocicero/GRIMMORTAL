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

                if (TryAddRoom(region, connector, out RoomObject room))
                {
                    rooms.Add(room);
                    openRooms.Add(room);
                }

                yield return new WaitForSeconds(generationDelay);
            }
        }
    }

    bool TryAddRoom(RegionSO region, Transform connector, out RoomObject room)
    {
        HallwayObject hallway = Instantiate(region.RandomHallway, transform);
        hallway.SnapTo(connector);

        room = Instantiate(region.RandomRoom, transform);
        room.SnapTo(hallway.Exit);

        Physics.SyncTransforms();

        BoxCollider bounds = room.Bounds;

        Collider[] hits = Physics.OverlapBox(
            bounds.transform.TransformPoint(bounds.center),
            Vector3.Scale(bounds.size, bounds.transform.lossyScale) / 2f,
            bounds.transform.rotation,
            layoutMask,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length > 0)
        {
            Destroy(hallway.gameObject);
            Destroy(room.gameObject);

            room = null;
            return false;
        }

        hallway.Init(room);
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
}