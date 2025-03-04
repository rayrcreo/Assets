using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Camera playerCamera; // Reference to the camera object
    public float cameraDistance = 5f; // Distance of the camera from the player
    public float jumpHeight = 2f; // Height of the jump
    private CharacterController characterController;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private bool wasMoving = false;
    private Vector3 velocity;
    private bool isGrounded;
    private float timeSinceLastMove = 0f;
    private const float freeCamDelay = 2.5f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        MovePlayer();
        RotateCamera();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void MovePlayer()
    {
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (move.magnitude > 0)
        {
            timeSinceLastMove = 0f;
        }
        else
        {
            timeSinceLastMove += Time.deltaTime;
        }
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        if (move.magnitude > 0)
        {
            if (!wasMoving)
            {
                // Reset freecam to default position
                horizontalRotation = 0f;
                playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
                playerCamera.transform.position = transform.position - playerCamera.transform.forward * cameraDistance;
            }

            // Align camera with player's direction
            transform.Rotate(Vector3.up * mouseX);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            playerCamera.transform.position = transform.position - playerCamera.transform.forward * cameraDistance;
            wasMoving = true;
        }
        else if (isGrounded && timeSinceLastMove >= freeCamDelay)
        {
            // Free look
            horizontalRotation += mouseX;
            Vector3 direction = new Vector3(0, 0, -cameraDistance);
            Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            playerCamera.transform.position = transform.position + rotation * direction;
            playerCamera.transform.LookAt(transform.position);
            wasMoving = false;
        }
    }
}
