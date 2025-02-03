using UnityEngine;

public class FreeFlyCameraController : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float rotationX = 0f; // For camera pitch (up/down)
    private float rotationY = 0f; // For camera yaw (left/right)

    void Start() {
        // Lock and hide the cursor so it never leaves the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        // Optional: Toggle cursor lock when pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // --- 1. Handle Movement (WASD + Space/Shift) ---

        float horizontalInput = Input.GetAxis("Horizontal");  // A/D
        float verticalInput = Input.GetAxis("Vertical");    // W/S

        float upDownInput = 0f;
        if (Input.GetKey(KeyCode.Space)) {
            upDownInput += 1f; // Move up
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            upDownInput -= 1f; // Move down
        }

        Vector3 direction = (transform.forward * verticalInput) +
                            (transform.right * horizontalInput) +
                            (transform.up * upDownInput);

        direction.Normalize();

        transform.position += direction * (moveSpeed * Time.deltaTime);

        // --- 2. Handle Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * mouseSensitivity;    // Yaw
        rotationX -= mouseY * mouseSensitivity;    // Pitch

        rotationX = Mathf.Clamp(rotationX, minPitch, maxPitch);

        transform.eulerAngles = new Vector3(rotationX, rotationY, 0f);
    }
}
