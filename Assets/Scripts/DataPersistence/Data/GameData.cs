using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentHealth;
    public int maxHealth;

    public GameData()
    {
        currentHealth = 100;
        maxHealth = 100;
    }
}
