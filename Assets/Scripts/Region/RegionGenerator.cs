using System.Collections;
using TMPro;
using UnityEngine;

public class RegionGenerator : MonoBehaviour
{
    public static RegionGenerator Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Animator playerTransitionAnimator;
    [SerializeField] GameObject hubObjects;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI regionInfoText;

    RegionSO currentRegion;
    RoomObject currentRoom;
    Transform landmarkParent;
    System.Random rng;
    int roomIndex = 0;
    int seed;

    AudioSource audioSource;

    public int Seed => seed;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void GenerateRegion(RegionSO region)
    {
        GenerateRegion(region, System.Guid.NewGuid().GetHashCode());
    }

    public void GenerateRegion(RegionSO region, int regionSeed)
    {
        seed = regionSeed;
        rng = new System.Random(seed);

        Debug.Log($"Generating {region.RegionName} with seed {seed}");

        StartCoroutine(GenerateRegionWorker(region));
    }

    IEnumerator GenerateRegionWorker(RegionSO region)
    {
        PlayTransitionFX();

        yield return new WaitForSeconds(1f);

        hubObjects.SetActive(false);

        currentRegion = region;
        roomIndex = 0;

        SpawnLandmarks();
        SpawnRoom();
        region.SetRegionAmbience();
    }

    public void SpawnNextRoom()
    {
        StartCoroutine(SpawnNextRoomWorker());
    }

    IEnumerator SpawnNextRoomWorker()
    {
        PlayTransitionFX();

        yield return new WaitForSeconds(1f);

        roomIndex++;
        SpawnRoom();
    }

    void SpawnLandmarks()
    {
        if (landmarkParent)
            Destroy(landmarkParent.gameObject);

        landmarkParent = new GameObject("Landmarks").transform;
        currentRegion.SpawnLandmarks(transform.position, landmarkParent, rng);
    }

    void SpawnRoom()
    {
        if (currentRoom)
            Destroy(currentRoom.gameObject);

        currentRoom = currentRegion.SpawnRoom(roomIndex, transform.position, rng);
    }

    void PlayTransitionFX()
    {
        playerTransitionAnimator.SetTrigger("RegionEnter");
        audioSource.Play();
    }

    public void UpdateRegionInfoText(string text)
    {
        regionInfoText.text = text;
    }
}