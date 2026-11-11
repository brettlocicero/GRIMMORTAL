using UnityEngine;
using UnityEngine.SceneManagement;

public class RegionDoor : MonoBehaviour, IInteractable
{
    [SerializeField] RegionSO region;

    public void Interact()
    {
        RegionGenerator.Instance.GenerateRegion(region);
    }
}