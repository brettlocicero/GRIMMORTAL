using UnityEngine;

public class RegionGenerator : MonoBehaviour
{
    [SerializeField] RegionSO testRegion;

    void Start()
    {
        GenerateRegion(testRegion);
    }

    public void GenerateRegion(RegionSO region) 
    {
        region.GenerateRegion();
    }
}
