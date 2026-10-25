using UnityEngine;
using TMPro;

// Shows collected / total generators, e.g. 0/2 then 1/2, 2/2.
// It counts the generators itself and listens to each one's Activated event,
// so it needs no GameManager and leaves Generator.cs untouched.
public class GeneratorCounterUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to use the TMP_Text on this object.")]
    [SerializeField] private TMP_Text label;

    private Generator[] generators;

    private void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        generators = Object.FindObjectsByType<Generator>(FindObjectsSortMode.None);
        foreach (Generator g in generators)
        {
            if (g != null) g.Activated += OnGeneratorActivated;
        }
        UpdateLabel();
    }

    private void OnDisable()
    {
        if (generators == null) return;
        foreach (Generator g in generators)
        {
            if (g != null) g.Activated -= OnGeneratorActivated;
        }
    }

    private void Start()
    {
        // Start runs after every Awake, so the first count shown is always correct.
        UpdateLabel();
    }

    private void OnGeneratorActivated(Generator g)
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label == null || generators == null) return;

        int collected = 0;
        foreach (Generator g in generators)
        {
            if (g != null && g.IsActivated) collected++;
        }
        label.text = collected + "/" + generators.Length;
    }
}
