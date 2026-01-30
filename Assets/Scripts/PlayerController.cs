using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Rigidbody of the player.
    private Rigidbody rb;

    // Variable to keep track of collected "PickUp" objects.
    private int count;

    // Total pickups in the scene (auto-detected)
    private int totalPickups;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Speed at which the player moves.
    public float speed = 0;

    // UI text component to display count of "PickUp" objects collected.
    public TextMeshProUGUI countText;

    // UI object to display winning/losing text.
    public GameObject winTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;

        // Prefab-safe UI hookup: find UI in the scene if not assigned
        if (countText == null)
            countText = GameObject.Find("CountText")?.GetComponent<TextMeshProUGUI>();

        if (winTextObject == null)
            winTextObject = GameObject.Find("WinText");

        if (countText == null || winTextObject == null)
        {
            Debug.LogError("UI not found. Ensure scene has objects named 'CountText' and 'WinText'.");
            enabled = false;
            return;
        }

        // Auto-detect how many pickups exist
        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;

        winTextObject.SetActive(false);
        SetCountText();
    }

    // This function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        if (rb != null)
            rb.AddForce(movement * speed);
    }

    // Called by DistanceShrinkRespawn when a pickup is collected
    public void AddPickup()
    {
        count++;
        SetCountText();
    }

    void SetCountText()
    {
        if (countText != null)
            countText.text = $"Count: {count}/{totalPickups}";

        if (count >= totalPickups && totalPickups > 0)
        {
            if (winTextObject != null)
            {
                winTextObject.SetActive(true);

                // If WinText is a TMP text component directly
                TextMeshProUGUI tmp = winTextObject.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                    tmp.text = "You Win!";
            }

            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
                Destroy(enemy);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

            if (winTextObject != null)
            {
                winTextObject.SetActive(true);

                TextMeshProUGUI tmp = winTextObject.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                    tmp.text = "You Lose!";
            }
        }
    }
}
