using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public UnitHealth health = new UnitHealth(20, 20);

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
        if (health.Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
