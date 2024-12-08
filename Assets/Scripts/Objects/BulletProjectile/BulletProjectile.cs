using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 5;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyHealth>() != null)
        {
            other.GetComponent<EnemyHealth>().health.TakeDamage(damage);
        }

        if (other.GetComponent<PlayerController>() != null)
        {
            other.GetComponent<PlayerBehavior>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
