using System.Collections;
using UnityEngine;

// Starts the boss fight the first time the player enters this trigger after all generators are on:
// seals the room, freezes the player, shows the boss appearing in smoke, then hands control back.
// Needs a Collider2D with "Is Trigger" checked on the same GameObject.
public class BossEncounter : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    // invisible walls turned on so the player can't leave the room
    [SerializeField] private GameObject[] roomBlockers;
    // disabled while the camera shows the boss, e.g. movement, aim and shooting
    [SerializeField] private Behaviour[] playerControls;

    [Header("Camera")]
    // found in the scene when empty
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform focusPoint;
    // seconds for the camera to reach the focus point before the smoke
    [SerializeField, Min(0f)] private float panDuration = 1f;
    // seconds the camera stays on the boss after it appears
    [SerializeField, Min(0f)] private float holdDuration = 1.5f;

    [Header("Smoke (optional)")]
    [SerializeField] private SpawnEffect smokePrefab;
    // seconds between the smoke and the boss, 0.33 = smoke at its biggest
    [SerializeField, Min(0f)] private float smokeDelay = 0.33f;

    private bool started;

    private void Awake()
    {
        if (cameraFollow == null) cameraFollow = FindFirstObjectByType<CameraFollow>();
        if (boss == null) Debug.LogWarning($"[BossEncounter] Boss not set: {name}", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (started || !IsPlayer(other)) return;
        if (GameManager.Instance == null || !GameManager.Instance.AllGeneratorsOn) return;

        started = true;
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        foreach (GameObject blocker in roomBlockers) blocker.SetActive(true);
        SetPlayerControls(false);

        Transform previousTarget = null;
        if (cameraFollow != null)
        {
            previousTarget = cameraFollow.Target;
            if (focusPoint != null) cameraFollow.Target = focusPoint;
        }
        yield return new WaitForSeconds(panDuration);

        if (boss != null)
        {
            if (smokePrefab != null) Instantiate(smokePrefab, boss.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(smokeDelay);
            boss.SetActive(true);
        }
        Debug.Log("[BossEncounter] Boss fight started");
        yield return new WaitForSeconds(holdDuration);

        if (cameraFollow != null) cameraFollow.Target = previousTarget;
        SetPlayerControls(true);
    }

    private void SetPlayerControls(bool on)
    {
        foreach (Behaviour control in playerControls) control.enabled = on;
    }

    // the player may have extra child colliders, so also check the Rigidbody2D
    private static bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player")) return true;
        return other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player");
    }
}
