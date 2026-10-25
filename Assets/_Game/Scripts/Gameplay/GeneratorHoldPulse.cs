using UnityEngine;

// Makes the generator's body breathe (scale up and down) while the player holds Interact.
// Target should be a child, not the root, so the trigger collider does not change size.
public class GeneratorHoldPulse : MonoBehaviour
{
    [SerializeField] private Generator generator;
    [SerializeField] private Transform target;
    // 0.05 = scale goes 5% above and below its normal size
    [SerializeField, Min(0f)] private float amount = 0.05f;
    // breaths per second
    [SerializeField, Min(0.01f)] private float speed = 2f;
    // how fast it returns to normal size after releasing
    [SerializeField, Min(0.01f)] private float settleSpeed = 10f;

    private Vector3 baseScale;
    private float holdStartTime;
    private bool wasHolding;

    private void Awake()
    {
        if (generator == null) generator = GetComponent<Generator>();
        if (generator == null || target == null)
        {
            Debug.LogWarning($"[GeneratorHoldPulse] Generator or Target not set: {name}", this);
            enabled = false;
            return;
        }
        baseScale = target.localScale;
    }

    private void Update()
    {
        bool holding = generator.IsHolding;
        if (holding && !wasHolding) holdStartTime = Time.time;
        wasHolding = holding;

        if (holding)
        {
            float wave = Mathf.Sin((Time.time - holdStartTime) * speed * 2f * Mathf.PI);
            target.localScale = baseScale * (1f + wave * amount);
        }
        else
        {
            target.localScale = Vector3.Lerp(target.localScale, baseScale, settleSpeed * Time.deltaTime);
        }
    }
}
