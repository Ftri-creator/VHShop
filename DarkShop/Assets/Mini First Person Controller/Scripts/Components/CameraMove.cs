using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    bool rotateBackwards = false;
    Vector2 velocity;
    Vector2 frameVelocity;
    public Transform enemy; // ������ �� ��������� �����
    public float rotationSpeed = 5f; // �������� �������� ������
    public Transform player; // ������ �� ��������� ������
    public float closeDistance = 2f; // ����������, ��� ������� ������ �������������� � ������� �����

    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<PlController>().transform;
    }

    void LateUpdate()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
    public void cameraRotate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ��������� ������� ������������ ������� (� ������ ������ - ������)
        {
            rotateBackwards = !rotateBackwards; // ����������� �������� ���������� rotateBackwards
        }

        // ���� rotateBackwards ������, �� ������������ ������ ����� (��������� ���� 180 ��������)
        float rotateAngleY = rotateBackwards ? 180f : 0f;

        // ������������ ������ ������ ��� X (�����-����) � Y (�����-������) �� ������ �������� �� ������� velocity
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x + rotateAngleY, Vector3.up);
    }
}
