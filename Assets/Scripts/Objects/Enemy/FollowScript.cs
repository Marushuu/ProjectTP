using UnityEngine;
using UnityEngine.AI;

public class FollowScript : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        enemy.SetDestination(player.position);
    }
}
