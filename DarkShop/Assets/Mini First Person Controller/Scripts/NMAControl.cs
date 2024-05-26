using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NMAControl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepSound;
    private float maxVolume = 0.5f;
    private float minVolume = 0.2f;
    private float visionRange = 7f;
    private int currentTargetIndex = 0;
    public float hearingDistance = 10f;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Animator animDoorway;
    private Animator animDoor;
    private bool canSeePlayer;
    private int i;
    private Animator animImageEndFade;
    private bool canSetDestination = false;
    private float stoppingDistance = 1f;
    public GameObject crosshair, loseMessage;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    private float distanceToPlayer;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animDoorway = GameObject.FindGameObjectWithTag("Doorway").GetComponent<Animator>();
        animDoor = GameObject.FindGameObjectWithTag("Door").GetComponent<Animator>();
        animImageEndFade = GameObject.FindGameObjectWithTag("Image").GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNextRandomTarget();
    }
    private void Update()
    {
        // Проверяем расстояние до игрока
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Проверяем, находится ли игрок в зоне видимости
        if (distanceToPlayer <= hearingDistance)
        {
            // Рассчитываем громкость на основе расстояния
            float volume = 1f - Mathf.Clamp01(distanceToPlayer / hearingDistance);
            volume = Mathf.Lerp(minVolume, maxVolume, volume);

            // Устанавливаем громкость звука
            audioSource.volume = volume;

            // Воспроизводим звук, если он не играет
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Выключаем звук, если игрок вне области видимости
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            SetNextRandomTarget();
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && Data.isCashout)
        {
            canSetDestination = true;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetInteger("Speed", 0);
        }
        else
        {
            animator.SetInteger("Speed", 1);
        }

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= visionRange)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, player.position, out hit))
                {
                    canSeePlayer = hit.transform.CompareTag("Player");
                }
            }
            else
            {
                canSeePlayer = false;
            }

            if (canSeePlayer)
            {
                agent.SetDestination(player.position);
                float enemyPlayerDistance = Vector3.Distance(transform.position, player.position);
                if (enemyPlayerDistance <= stoppingDistance)
                {
                    agent.isStopped = true; // Останавливаем движение агента
                    audioSource.Stop();
                    animImageEndFade.SetTrigger("EndFadeBlack");
                    StartCoroutine(showMessage());
                    Data.isDie = true;
                    animator.SetInteger("Speed", 0);
                }
                else
                {
                    agent.isStopped = false; // Возобновляем движение агента
                    animator.SetInteger("Speed", 1);
                }
            }
            else
            {
                if (canSetDestination)
                {
                    agent.SetDestination(targets[i].position);
                }
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Door")) { animDoor.Play("OpenDoor"); Data.isOpenDoor = true; }
        if (collision.gameObject.CompareTag("Doorway")) { animDoorway.Play("OpenDoorway"); Data.isOpenDoorway = true; }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
    IEnumerator showMessage()
    {
        yield return new WaitForSeconds(2);
        crosshair.SetActive(false);
        loseMessage.SetActive(true);
    }
    void SetNextRandomTarget()
    {
        currentTargetIndex = Random.Range(0, targets.Count);
        agent.SetDestination(targets[currentTargetIndex].position);
    }
}
