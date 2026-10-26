using System;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerAmmo ammo;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.2f;

    private float nextFireTime;

    public event Action Fired;

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
        // TransformDirection, not firePoint.rotation: the gun is mirrored with negative scale, which rotation ignores
        Vector3 direction = firePoint.TransformDirection(Vector3.up);
        Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
        Fired?.Invoke();
    }
}
