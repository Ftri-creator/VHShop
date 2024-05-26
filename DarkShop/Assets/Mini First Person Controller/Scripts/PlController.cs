using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlController: MonoBehaviour
{
    [Header("Other")]
    public CameraControl CameraControl;
    public Image crosshairImage;
    private float rayLength = 2.5f;
    public Transform playerCamera;
    private bool isCloseDoor = true;
    private bool isCloseDoorway = true;
    private bool hasKey;
    private bool hasFakeKey = false;
    private bool canPickup = false;
    private bool canGoOut = false;
    private bool canRestart = false;
    public TMP_Text escapeTimeText;
    private float timer = 0f;
    public GameObject Aim, Player, text1, text2, text3, text4, text5, text6, text7, warningFakeKey, endText, flashText, escapeTime, helpmessageOpenDoor,FakeKey, Key, Tape, crosshair,
    helpmessageCloseDoor, helpmessageCloseDoorway, helpmessageOpenDoorway, helpmessageTakeKey, helpmessageCashOut, helpmessageTakeTape, PCollision, Light, Enemy, escapeObject;
    [Header("Animator")]
    private Animator animDoorway, animDoor, animImageEndFade, animentranceDoor;
    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioSource atmospericAudio;
    [SerializeField] private AudioSource flashAudio, flashAudio1;
    private float speed = 2.75f; // Скорость перемещения игрока
    private Rigidbody rb; // Ссылка на Rigidbody компонент
    public KeyCode runningKey = KeyCode.LeftShift;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();
    public bool canRun = false;
    public bool IsRunning { get; private set; }
    public float runSpeed = 0;

    private void Start()
    {
        StartCoroutine(hideText(text4, 3f));
        Key.SetActive(false);
    }
    // Вызывается при запуске игры
    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Получаем ссылку на Rigidbody компонент\
        rb.freezeRotation = true;
        // Get the rigidbody on this.
        rb = GetComponent<Rigidbody>();
        animDoorway = GameObject.FindGameObjectWithTag("Doorway").GetComponent<Animator>();
        animDoor = GameObject.FindGameObjectWithTag("Door").GetComponent<Animator>();
        animImageEndFade = GameObject.FindGameObjectWithTag("Image").GetComponent<Animator>();
        animentranceDoor = GameObject.FindGameObjectWithTag("EntranceDoor").GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        atmospericAudio = GetComponent<AudioSource>();
    }

    // Вызывается при обновлении сцены
    void FixedUpdate()
    {
        float targetMovingSpeed = speed; // Используем обычную скорость, без возможности бега

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Применение движения
        if (Data.canMove)
        {
            rb.velocity = transform.rotation * new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.y);
        }
        else
        {
            return;
        }
        timer += Time.deltaTime;
        RaycastHit hit;
        // Рассчитываем позицию прицела в мире из позиции на Canvas
        Vector2 screenPosition = crosshairImage.rectTransform.position;
        Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(screenPosition);

        // Отрисовываем луч для отладки (убрать в релизной сборке)
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        // Проверяем столкновение луча с объектами
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if (hit.collider.CompareTag("Key") || hit.collider.CompareTag("FakeKey"))
            {
                helpmessageTakeKey.SetActive(true);
                if (Input.GetKey(KeyCode.E) && hit.collider.CompareTag("Key")) { hasKey = true; hasFakeKey = false; Key.SetActive(false); }
                if (Input.GetKey(KeyCode.E) && hit.collider.CompareTag("FakeKey")) { hasKey = false; hasFakeKey = true; FakeKey.SetActive(false); StartCoroutine(hideText(text3, 3f)); }
            }
            else { helpmessageTakeKey.SetActive(false); }
            if (hit.collider.CompareTag("Tape"))
            {
                if (canPickup)
                {
                    helpmessageTakeTape.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        canGoOut = true;
                        Tape.SetActive(false);
                    }
                }
            }
            else { helpmessageTakeTape.SetActive(false); }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TakeKey"))
        {
            StartCoroutine(hideText(text1, 3f));
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Escape"))
        {
            canRestart = true;
            if (canGoOut)
            {
                animImageEndFade.SetTrigger("EndFadeBlack"); endText.SetActive(true); escapeTime.SetActive(true);
                escapeTimeText.text = "Время: " + timer.ToString();
                crosshair.SetActive(false);
                StartCoroutine(pauseGame(2.5f));
                atmospericAudio.Stop();
            }
            else { StartCoroutine(hideText(text7, 3f)); }
        }
        if (other.CompareTag("Detect"))
        {
            StartCoroutine(hideText(text2, 3f));
        }
    }
    private void offLights()
    {
        Light.SetActive(false);
    }

    private void Update()
    {
        if (Data.isDie)
        {
            atmospericAudio.Stop();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene("Menu"); }
        if (canRestart) { if(Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene("Demo"); } }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Computer"))
        {
            helpmessageCashOut.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F) && hasFakeKey)
            {
                StartCoroutine(offCollision(10f));
                helpmessageCashOut.SetActive(false);
                StartCoroutine(hideText(warningFakeKey, 3f));
                Enemy.SetActive(true);
                Key.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.F) && hasKey)
            {
                Data.isCashout = true;
                PCollision.GetComponent<Collider>().enabled = false;
                animentranceDoor.Play("CloseEntrance");
                StartCoroutine(hideText(text5, 3f));
                StartCoroutine(hideTextBeforeTime(text6, 3.5f, 3f));
                StartCoroutine(dontMove(6.75f));
                offLights();
                helpmessageCashOut.SetActive(false);
                escapeObject.SetActive(true);
                m_audioSource.Play();
                canPickup = true;
            }
        }
        else { helpmessageCashOut.SetActive(false); }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            canRestart = true;
            if (Input.GetKeyDown(KeyCode.R) && canRestart)
            {
                SceneManager.LoadScene(1);
            }
        }
        // Door Open / Close Logic
        if (collision.gameObject.CompareTag("Door") && isCloseDoor)
        {
            helpmessageOpenDoor.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) { animDoor.Play("OpenDoor"); isCloseDoor = false; }
        }
        else { helpmessageOpenDoor.SetActive(false); }
        if (collision.gameObject.CompareTag("Door") && isCloseDoor == false || collision.gameObject.CompareTag("Door") && Data.isOpenDoor == true)
        {
            helpmessageCloseDoor.SetActive(true);
            helpmessageOpenDoor.SetActive(false);
            if (Input.GetKeyDown(KeyCode.C)) { animDoor.Play("CloseDoor"); isCloseDoor = true; Data.isOpenDoor = false; }
        }
        else { helpmessageCloseDoor.SetActive(false); }
        // Doorway Open / Close Logic
        if (collision.gameObject.CompareTag("Doorway") && isCloseDoorway)
        {
            helpmessageOpenDoorway.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) { animDoorway.Play("OpenDoorway"); isCloseDoorway = false; }
        }
        else { helpmessageOpenDoorway.SetActive(false); }
        if (collision.gameObject.CompareTag("Doorway") && isCloseDoorway == false || collision.gameObject.CompareTag("Doorway") && Data.isOpenDoorway == true)
        {
            helpmessageCloseDoorway.SetActive(true);
            helpmessageOpenDoorway.SetActive(false);
            if (Input.GetKeyDown(KeyCode.C)) { animDoorway.Play("CloseDoorway"); isCloseDoorway = true; Data.isOpenDoorway = false; }
        }
        else { helpmessageCloseDoorway.SetActive(false); }
        if (collision.gameObject.CompareTag("Toilet"))
        {
            flashText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) { flashAudio.Play(); }
        }
        else { flashText.SetActive(false); }
    }
    IEnumerator hideText(GameObject text, float duration)
    {
        text.SetActive(true);
        yield return new WaitForSeconds(duration);
        text.SetActive(false);
    }
    IEnumerator hideTextBeforeTime(GameObject text, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        text.SetActive(true);
        yield return new WaitForSeconds(duration);
        text.SetActive(false);
    }
    IEnumerator pauseGame(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 0f;
    }
    IEnumerator offCollision(float offTime)
    {
        PCollision.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(offTime);
        PCollision.GetComponent<Collider>().enabled = true;
    }
    IEnumerator dontMove(float timeNotMove)
    {
        rb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        CameraControl.enabled = false;
        Data.canMove = false;
        yield return new WaitForSeconds(timeNotMove);
        Data.canMove = true;
        CameraControl.enabled = true;
        rb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        rb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }
}
