using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
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
    private const float transitionSpeed = 2f; // Speed of the camera transition
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;
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
        Dash();

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
            timeSinceLastMove = 0f; // Reset the timer when there is movement
        }
        else
        {
            // Allow free camera movement
            horizontalRotation += mouseX;
            Vector3 targetPosition = transform.position - Quaternion.Euler(verticalRotation, horizontalRotation, 0) * Vector3.forward * cameraDistance;
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - targetPosition);

            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, Time.deltaTime * transitionSpeed);
            playerCamera.transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * transitionSpeed);

            if (isGrounded && timeSinceLastMove >= freeCamDelay)
            {
                // Free look
                playerCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, Time.deltaTime * transitionSpeed);
                playerCamera.transform.rotation = Quaternion.Lerp(originalCameraRotation, targetRotation, Time.deltaTime * transitionSpeed);
                wasMoving = false;
            }
        }
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dashDirection = transform.forward;
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
        }
    }
}