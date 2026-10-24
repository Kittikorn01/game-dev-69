using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private Behaviour[] disableOnDeath;
    [SerializeField] private GameObject[] hideOnDeath;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void OnDied()
    {
        foreach (Behaviour behaviour in disableOnDeath) behaviour.enabled = false;
        foreach (GameObject target in hideOnDeath) target.SetActive(false);
    }
}
