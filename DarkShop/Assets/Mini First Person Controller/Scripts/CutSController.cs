using System.Collections;
using UnityEngine;

public class CutSController : MonoBehaviour
{
    public GameObject Aim, text8, Player, skipCutSText;
    private bool canSkip = false;
    [SerializeField] private Animator animFade;
    private void Awake()
    {
        animFade = GameObject.FindGameObjectWithTag("ImageFade").GetComponent<Animator>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(spawnPlayer());
        StartCoroutine(hideText8());
        StartCoroutine(skipCutScene());
        Invoke("startTransition", 10.5f);
        Aim.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canSkip) { Player.SetActive(true); Aim.SetActive(true); }
    }
    IEnumerator spawnPlayer()
    {
        yield return new WaitForSeconds(12.5f);
        Aim.SetActive(true);
        Player.SetActive(true);
    }
    IEnumerator hideText8()
    {
        text8.SetActive(true);
        yield return new WaitForSeconds(5.5f);
        text8.SetActive(false);
    }
    IEnumerator skipCutScene()
    {
        yield return new WaitForSeconds(3f);
        skipCutSText.SetActive(true);
        canSkip = true;
    }
    private void startTransition()
    {
        animFade.SetTrigger("SetToDark");
    }
}