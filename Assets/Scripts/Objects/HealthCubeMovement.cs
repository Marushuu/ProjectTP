using UnityEngine;

public class HealthCubeMovement : MonoBehaviour
{
    [SerializeField]
    public Transform[] wayPoints;
    public float movementSpeed = 25f;
    public int healAmount = 5;


    private int currentWayPoint = 0;
    private Rigidbody rb;
    private GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (Vector3.Distance(transform.position, wayPoints[currentWayPoint].position) < .25f)
        {
            currentWayPoint++;
            currentWayPoint = currentWayPoint % wayPoints.Length;
        }
        Vector3 direction = (wayPoints[currentWayPoint].position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletProjectile>() != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerBehavior>().Heal(healAmount);
        }
    }
}
