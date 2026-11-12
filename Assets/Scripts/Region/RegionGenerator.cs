using System.Collections;
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

    RegionSO currentRegion;
    RoomObject currentRoom;
    int roomIndex = 0;
    
    AudioSource audioSource;

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
        PlayTransitionFX();

        yield return new WaitForSeconds(1f);

        hubObjects.SetActive(false);

        currentRegion = region;
        roomIndex = 0;

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

    void SpawnRoom()
    {
        if (currentRoom)
            Destroy(currentRoom.gameObject);

        currentRoom = currentRegion.SpawnRoom(roomIndex, transform.position);
    }

    void PlayTransitionFX()
    {
        playerTransitionAnimator.SetTrigger("RegionEnter");
        audioSource.Play();
    }
}