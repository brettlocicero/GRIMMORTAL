using System.Collections;
using UnityEngine;

public class RoomShrine : MonoBehaviour, IInteractable
{
    enum State { Idle, Active, Cleared }

    [SerializeField] RoomObject room;
    [SerializeField] float delayTime = 4f;
    [SerializeField] Vector3 nextRoomPlayerOffset;
    [SerializeField] ParticleSystem chargeParticles;
    [SerializeField] Animator activateAnimator;

    State state = State.Idle;
    PlayerController player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        room.OnCleared += HandleRoomCleared;
    }

    void OnDestroy()
    {
        room.OnCleared -= HandleRoomCleared;
    }

    public void Interact()
    {
        if (state == State.Idle) Activate();
        else if (state == State.Cleared) GoToNextRoom();
    }

    void Activate()
    {
        state = State.Active;

        PlayActivatedAnimation();
        StartCoroutine(StartRoomSpawning());
    }

    void GoToNextRoom()
    {
        RegionGenerator.Instance.SpawnNextRoom();
    }

    void HandleRoomCleared()
    {
        state = State.Cleared;
    }

    IEnumerator StartRoomSpawning()
    {
        yield return new WaitForSeconds(delayTime);
        room.StartEnemySpawning();
    }

    void PlayActivatedAnimation()
    {
        chargeParticles.Play();
        activateAnimator.SetTrigger("Activate");
    }
}