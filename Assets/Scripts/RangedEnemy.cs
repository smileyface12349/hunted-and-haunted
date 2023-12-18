using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public float projectileDamage = 35;
    public float projectileSpeed = 4;
    public float speedWhenShooting = 1f;
    public float timeBetweenShots = 5f;
    public float timeToShoot; // Redundant unless stop to shoot is enabled
    public float minDistance; // Below this distance, it will not shoot
    public float maxDistance; // Beyond this distance, it will not shoot
    public float aimRandomness; // Max radians to vary the angle by in each direction. Pi = fully random.
    public bool dontAimAtPlayer; // If enabled, 'aimRandomness' is the step to rotate each time.
    public float projectileSize = 1f;

    private float timeSinceLastShot;
    private float timeSinceStartingShot;
    private bool isShooting;
    private float currentAngle = 0f;
    
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (player == null) return;
        
        base.Update();
        
        if (!isShooting) {
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= timeBetweenShots) {
                float distanceFromPlayer = (player.transform.position - transform.position).magnitude;
                if (distanceFromPlayer < maxDistance && distanceFromPlayer > minDistance) {
                    isShooting = true;
                    timeSinceStartingShot = 0;
                    timeSinceLastShot = 0;
                    if (speedWhenShooting < 1) {
                        speed *= speedWhenShooting;
                    }
                }
            }
        } else {
            timeSinceStartingShot += Time.deltaTime;
        }
        
        if (isShooting && (timeToShoot == 0 || timeSinceStartingShot > timeToShoot)) {
            if (dontAimAtPlayer) {
                Shoot(currentAngle);
                currentAngle += aimRandomness;
            } else {
                ShootAtPlayer();
            }
            if (speedWhenShooting < 1) {
                speed /= speedWhenShooting;
            }
            isShooting = false;
        }
        
    }
    
    public new void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
    }
    
    public void ShootAtPlayer() {
        Vector3 diff = player.transform.position - transform.position;
        if (diff.magnitude > minDistance) {
            float angle = Mathf.Atan2(diff.y, diff.x);
            if (aimRandomness > 0) {
                angle += Random.Range(-aimRandomness, aimRandomness);
            }
            Shoot(angle);
        }
    }
    
    public void Shoot(float angle) {
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg - 90f, Vector3.forward);
        GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
        if (projectileSize != 1) {
            bullet.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
        bullet.GetComponent<BulletController>().angle = angle;
        bullet.GetComponent<BulletController>().speed = projectileSpeed;
        bullet.GetComponent<BulletController>().damage = projectileDamage;
    }
}

