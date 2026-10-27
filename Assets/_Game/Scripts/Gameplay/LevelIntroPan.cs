using System.Collections;
using UnityEngine;

// At level start, locks the player and pans the camera to a target (the exit door) and back
public class LevelIntroPan : MonoBehaviour
{
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform panTarget;
    [SerializeField] private Behaviour[] lockDuringIntro;
    // waits for the ScreenFader fade-in to finish
    [SerializeField, Min(0f)] private float startDelay = 1.5f;
    [SerializeField, Min(0.01f)] private float panDuration = 1.5f;
    [SerializeField, Min(0f)] private float holdDuration = 1f;

    private void Start()
    {
        if (cameraFollow == null && Camera.main != null) cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null || panTarget == null)
        {
            Debug.LogWarning("LevelIntroPan: missing CameraFollow or pan target, skipping intro.", this);
            return;
        }

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        SetLocked(true);
        yield return new WaitForSeconds(startDelay);

        cameraFollow.enabled = false;
        Transform cam = cameraFollow.transform;
        Vector3 home = cam.position;

        yield return Pan(cam, home, AtCameraDepth(panTarget.position, home.z));
        yield return new WaitForSeconds(holdDuration);

        Vector3 back = cameraFollow.Target != null ? AtCameraDepth(cameraFollow.Target.position, home.z) : home;
        yield return Pan(cam, cam.position, back);

        cameraFollow.enabled = true;
        SetLocked(false);
    }

    private IEnumerator Pan(Transform cam, Vector3 from, Vector3 to)
    {
        for (float t = 0f; t < panDuration; t += Time.deltaTime)
        {
            cam.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / panDuration));
            yield return null;
        }
        cam.position = to;
    }

    private void SetLocked(bool locked)
    {
        foreach (Behaviour behaviour in lockDuringIntro)
        {
            if (behaviour != null) behaviour.enabled = !locked;
        }
    }

    private static Vector3 AtCameraDepth(Vector3 point, float z)
    {
        return new Vector3(point.x, point.y, z);
    }
}
