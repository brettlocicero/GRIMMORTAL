using System.Collections;
using UnityEngine;

public class RoomShrine : MonoBehaviour, IInteractable
{
    [SerializeField] RoomObject room;
    [SerializeField] float delayTime = 4f;
    [SerializeField] ParticleSystem chargeParticles;
    [SerializeField] Animator activateAnimator;

    public void Interact()
    {
        PlayActivatedAnimation();
        StartCoroutine(StartRoomSpawning());
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
