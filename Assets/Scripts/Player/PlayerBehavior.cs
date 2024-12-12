using UnityEngine;

public class PlayerBehavior : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    public int currentHealth = 200;
    public int maxHealth = 200;
    public int change = 10;
    public int score = 0; // Added for the sake of time
    public Canvas gameOverCanvas;

    public UnitHealth _playerHealth;

    void Start()
    {
        _playerHealth = new UnitHealth(currentHealth, maxHealth);
        currentHealth = _playerHealth.Health;
        maxHealth = _playerHealth.MaxHealth;
    }

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
        currentHealth = _playerHealth.Health;
        maxHealth = _playerHealth.MaxHealth;
        if (_playerHealth.Health <= 0)
        {
            Die();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Heal(change);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TakeDamage(change);
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
        Time.timeScale = 0;
        gameOverCanvas.gameObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyHealth>() != null)
        {
            Debug.Log("Player hit");
            TakeDamage(5);
        }
    }
}
