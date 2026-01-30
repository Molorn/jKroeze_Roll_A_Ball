using Valve.VR.InteractionSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceShrinkRespawn : MonoBehaviour
{
    // Auto-filled reference to PlayerController on the same object (prefab-safe)
    private PlayerController player;

    public float shrinkStartDistance = 10f;
    public float shrinkSpeed = 0.05f;
    public float minScale = 0.01f;

    private Vector3 spawnPosition;
    private Vector3 originalScale;

    private Interactable interactable;
    private bool splatted = false;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        player = GetComponent<PlayerController>(); // <-- key fix (no manual assignment)
    }

    void Start()
    {
        spawnPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, spawnPosition);

        if (distance >= shrinkStartDistance)
        {
            Shrink();
        }
        else if (splatted)
        {
            Shrink();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            splatted = true;

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Set pancake scale
            transform.localScale = new Vector3(0.45f, 0.05f, 0.45f);

            // Align pancake to surface
            ContactPoint contact = collision.contacts[0];
            Vector3 surfaceNormal = contact.normal;

            transform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        }
    }

    void Shrink()
    {
        Vector3 scale = transform.localScale;
        scale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        transform.localScale = scale;

        if (scale.y <= minScale)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        TriggerUngrab();
        transform.position = spawnPosition;
        transform.localScale = originalScale;

        splatted = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void TriggerUngrab(float disableTime = 1f)
    {
        StartCoroutine(UngrabRoutine(disableTime));
    }

    private IEnumerator UngrabRoutine(float disableTime)
    {
        // If the object is currently held, force release
        if (interactable != null && interactable.attachedToHand)
        {
            Hand hand = interactable.attachedToHand;
            hand.DetachObject(gameObject, restoreOriginalParent: true);
        }

        // Disable grabbing
        if (interactable != null)
            interactable.enabled = false;

        yield return new WaitForSeconds(disableTime);

        // Re-enable grabbing
        if (interactable != null)
            interactable.enabled = true;
    }

private void OnTriggerEnter(Collider other)
{
    if (!other.CompareTag("PickUp")) return;

    other.gameObject.SetActive(false);

    if (GameManager.Instance != null)
        GameManager.Instance.AddPickup();
    else
        Debug.LogError("No GameManager in scene!");
}

}
