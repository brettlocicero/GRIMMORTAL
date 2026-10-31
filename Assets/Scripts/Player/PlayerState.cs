using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;

    int health;

    public int Health => health;

    void Awake()
    {
        health = maxHealth;
    }

    public void Damage(int amount)
    {
        health = Mathf.Max(0, health - amount);
    }
}