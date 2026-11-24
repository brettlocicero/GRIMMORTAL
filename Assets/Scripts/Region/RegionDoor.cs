using UnityEngine;
using UnityEngine.SceneManagement;

public class RegionDoor : MonoBehaviour, IInteractable
{
    public string Prompt => "Enter " + region.RegionName;

    [SerializeField] RegionSO region;

    public void Interact()
    {
        RegionGenerator.Instance.GenerateRegion(region);
    }
}