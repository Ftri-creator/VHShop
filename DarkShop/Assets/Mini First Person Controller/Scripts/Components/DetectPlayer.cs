using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public AudioSource ScarryAudio;
    public GameObject detected;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScarryAudio.Play();
            detected.SetActive(false);
        }
    }
}
