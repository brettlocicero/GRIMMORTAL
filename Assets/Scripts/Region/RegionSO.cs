using UnityEngine;

[CreateAssetMenu(fileName = "RegionSO", menuName = "Scriptable Objects/RegionSO")]
public class RegionSO : ScriptableObject
{
    [SerializeField] string regionName;
    [SerializeField] GameObject[] props;
    
    public string RegionName => RegionName;
    
    public void GenerateRegion()
    {
        foreach (GameObject prop in props) 
        {
            
        }
    }
}