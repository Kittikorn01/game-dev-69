using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerAmmo ammo;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.2f;

    private float nextFireTime;

    private void OnEnable()
    {
        inputReader.AttackPressed += Shoot;
    }

    private void OnDisable()
    {
        inputReader.AttackPressed -= Shoot;
    }

    private void Shoot()
    {
        if (Time.time < nextFireTime) return;
        if (!ammo.TryConsume()) return;

        nextFireTime = Time.time + fireCooldown;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
