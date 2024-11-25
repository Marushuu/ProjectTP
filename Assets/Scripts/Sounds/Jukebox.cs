using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AudioSource audioSource;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            audioSource.Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            audioSource.Stop();
        }
    }
}
