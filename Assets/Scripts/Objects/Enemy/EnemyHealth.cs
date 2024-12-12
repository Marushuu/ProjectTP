using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    public int curHealth = 20;
    public int maxHealth = 20;

    private GameObject player;
    public UnitHealth health;

    void Start()
    {
        health = new UnitHealth(curHealth, maxHealth);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (health.Health <= 0)
        {
            Destroy(gameObject);
            player.GetComponent<PlayerBehavior>().score += 1;
        }
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletProjectile>() != null)
        {
            TakeDamage(other.GetComponent<BulletProjectile>().damage);
        }
    }
}
