using UnityEngine;

public class PlayerBehavior : MonoBehaviour, IDataPersistence
{
    public UnitHealth _playerHealth = new UnitHealth(100, 100);

    public void LoadGameData(GameData data)
    {
        _playerHealth.Health = data.currentHealth;
        _playerHealth.MaxHealth = data.maxHealth;
    }

    public void SaveGameData(ref GameData data)
    {
        data.currentHealth = _playerHealth.Health;
        data.maxHealth = _playerHealth.MaxHealth;
    }

    void Update()
    {
        if (_playerHealth.Health <= 0)
        {
            Die();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Heal(10);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            DataPersistenceManager.instance.SaveGameData();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DataPersistenceManager.instance.LoadGameData();
        }
    }

    public void TakeDamage(int damage)
    {
        _playerHealth.TakeDamage(damage);
        Debug.Log("Player took " + damage + " damage");
    }

    public void Heal(int healAmount)
    {
        _playerHealth.Heal(healAmount);
        Debug.Log("Player healed " + healAmount + " health");
    }

    public void Die()
    {
        _playerHealth.Die();
        Debug.Log("Player is dead");
    }

}
