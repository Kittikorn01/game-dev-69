using UnityEngine;

// Plays one random clip once, then destroys itself.
// Each clip must have an Animator state with the same name.
[RequireComponent(typeof(Animator))]
public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private AnimationClip[] clips;

    private void Start()
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"[SpawnEffect] No clips set: {name}", this);
            Destroy(gameObject);
            return;
        }

        AnimationClip clip = clips[Random.Range(0, clips.Length)];
        GetComponent<Animator>().Play(clip.name, 0, 0f);
        Destroy(gameObject, clip.length);
    }
}
