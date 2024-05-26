using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float bobSpeed = 1f; // Скорость покачивания камеры
    public float bobAmount = 0.05f; // Размер покачивания камеры

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobAmount;
            transform.localPosition = new Vector3(startPos.x, startPos.y + translateChange, startPos.z);
        }
        else
        {
            transform.localPosition = startPos;
        }
    }
}
