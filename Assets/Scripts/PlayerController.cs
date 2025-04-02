using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Joystick joystick;
    public Joystick rotateJoystick;
    public float moveSpeed = 5f;
    [SerializeField] public float rotateSpeed = 1.5f;
    public Transform cameraTransform;

    private CharacterController _controller;
    private float _verticalRotation = 0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 move = transform.forward * joystick.GetInput().y + transform.right * joystick.GetInput().x;
        _controller.Move(move * moveSpeed * Time.deltaTime);
    }
    private void Rotate()
    {
        float rotateX = rotateJoystick.GetInput().x * rotateSpeed;
        float rotateY = rotateJoystick.GetInput().y * rotateSpeed;

        transform.Rotate(Vector3.up * rotateX);
        _verticalRotation -= rotateY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }


}