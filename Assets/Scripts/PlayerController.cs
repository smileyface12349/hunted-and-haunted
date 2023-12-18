using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviour
{
    public static float MaxHealth = 100;
    public Vector3 ScreenCenter; // Position of the player on the screen
    public static readonly float MinRadius = 25; // Below this distance, clicks will be rejected
    public static readonly float InvulnerabilityTime = 0f; // Seconds of invulnerability after taking damage
    public static readonly float ShootCooldown = 0.2f; // Seconds between shots if mouse button held
    public static readonly float ClickShootCooldown = 0.06f; // Seconds between shots if mouse button repeatedly clicked
    public static readonly int MaxEffects = 5;
    public static readonly float HauntChance = 0.75f; // Chance of getting a haunt (could be nothing) versus a power up
    public static readonly float HealthRegenInterval = 0.1f; // Seconds between health regen ticks
    public static readonly float HealthRegenAmount = 1; // Health Regen Per Tick
    public static readonly float HealthRegenThreshold = 3f; // Seconds of no damage before health regen
    public static readonly float ShootRotateThreshold = 0.3f; // Seconds since shooting before rotating player back
    public static readonly float GunFlashDuration = 0.1f; // Seconds that the gun flash is visible for

    public GameObject bulletPrefab;
    public float health;
    public float timeSinceDamage;
    public float speed = 1f;
    public float shootAngle;
    public float timeSinceShoot = 10;
    public UIController uiController;
    public SpriteRenderer sprite;
    public SpriteRenderer gunFlash;
    public GameObject gunSound;

    private ActiveEffect[] effects = new ActiveEffect[MaxEffects];

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
        ScreenCenter = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Set effects that have ended to null
        UpdateEffects();
        timeSinceDamage += Time.deltaTime;

        // Moving
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (CheckForEffect(EffectType.InvertControls)) {
            horizontal = -horizontal;
            vertical = -vertical;
        }
        if (CheckForEffect(EffectType.Stun)) {
            horizontal *= 0.7f;
            vertical *= 0.7f;
        }
        // Limit speed to the player speed (so it isn't any faster when diagonally)
        Vector2 velocity = new Vector2(horizontal, vertical);
        if (velocity.magnitude > 1) {
            velocity.Normalize();
            horizontal = velocity.x;
            vertical = velocity.y;
        }
        if (CheckForEffect(EffectType.ExtraSpeed)) {
            horizontal *= 2f;
            vertical *= 2f;
        }
        if (horizontal != 0) {
            float deltaX = speed * LevelController.Speed * horizontal * Time.deltaTime;
            if (gameObject.transform.position.x + deltaX < -LevelController.Bounds.x) {
                Debug.Log("Out of bounds! " + deltaX);
                deltaX = - LevelController.Bounds.x - gameObject.transform.position.x;
            }
            if (gameObject.transform.position.x + deltaX > LevelController.Bounds.x) {
                Debug.Log("Out of bounds! " + deltaX);
                deltaX = LevelController.Bounds.x - gameObject.transform.position.x;
            }
            gameObject.transform.Translate(deltaX, 0, 0);
        }
        if (vertical != 0) {
            float deltaY = speed * LevelController.Speed * vertical * Time.deltaTime;
            if (gameObject.transform.position.y + deltaY < -LevelController.Bounds.y) {
                Debug.Log("Out of bounds! " + deltaY);
                deltaY = - LevelController.Bounds.y - gameObject.transform.position.y;
            }
            if (gameObject.transform.position.y + deltaY > LevelController.Bounds.y) {
                Debug.Log("Out of bounds! " + deltaY);
                deltaY = LevelController.Bounds.y - gameObject.transform.position.y;
            }
            gameObject.transform.Translate(0, deltaY, 0);
        }
        
        // Shooting
        timeSinceShoot += Time.deltaTime;
        if (Input.GetMouseButton(0) && !CheckForEffect(EffectType.NoShoot)) {
            if (timeSinceShoot > ShootCooldown || (timeSinceShoot > ClickShootCooldown && Input.GetMouseButtonDown(0))) {
                timeSinceShoot = 0;
                Debug.Log(Input.mousePosition);
                Vector3 diff = Input.mousePosition - ScreenCenter;
                if (diff.magnitude > MinRadius) {
                    Instantiate(gunSound);
                    gunFlash.enabled = true;
                    shootAngle = Mathf.Atan2(diff.y, diff.x);
                    if (CheckForEffect(EffectType.InvertAim)) {
                        shootAngle += Mathf.PI;
                    }
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = gameObject.transform.position;
                    bullet.GetComponent<BulletController>().angle = shootAngle;
                    if (CheckForEffect(EffectType.ExtraDamage)) {
                        bullet.GetComponent<BulletController>().damage = 100;
                    } else if (CheckForEffect(EffectType.ReducedDamage)) {
                        bullet.GetComponent<BulletController>().damage = 25;
                    } else {
                        bullet.GetComponent<BulletController>().damage = 50;
                    }
                    bullet.GetComponent<BulletController>().speed = 8;
                    bullet.GetComponent<BulletController>().isPlayer = true;
                    if (CheckForEffect(EffectType.PiercingBullets)) {
                        bullet.GetComponent<BulletController>().piercing = true;
                    }
                }
            }
        }
        
        // Turn off gun flash
        if (gunFlash.enabled && timeSinceShoot > GunFlashDuration) {
            gunFlash.enabled = false;
        }
        
        // Rotating player
        if (timeSinceShoot < ShootRotateThreshold) {
            // Rotate player in direction of shot
            float angle = shootAngle * Mathf.Rad2Deg;
            sprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } else if (velocity.magnitude > 0) {
            float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            sprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Health regen
        if (health < MaxHealth && timeSinceDamage > HealthRegenThreshold && Math.Floor(Time.time / HealthRegenInterval) != Math.Floor((Time.time - Time.deltaTime) / HealthRegenInterval)) {
            health += HealthRegenAmount;
            if (health > MaxHealth) {
                health = MaxHealth;
            }
            uiController.UpdateHealth(health);
        }
    }
    
    public bool CheckForEffect(EffectType effectType) {
        // note: we assume that they are removed frequently enough when expiring
        foreach (ActiveEffect effect in effects)
        {
            if (effect == null) break; // we assume all the non-null values are at the start
            if (effect.Type == effectType) {
                return true;
            }
        }
        return false;
    }
    
    private void UpdateEffects() {
        Debug.Log(effects);
        bool changed = false;
        for (int i = 0; i < effects.Length; i++) {
            if (effects[i] != null) {
                if (effects[i].HasEnded()) {
                    effects[i] = null;
                    changed = true;
                    // Shift the rest forwards
                    for (int j = i; j < effects.Length - 1; j++) {
                        effects[j] = effects[j+1];
                    }
                    effects[effects.Length - 1] = null;
                    // Do this one again
                    i--;
                }
            }
        }
        if (changed)
            uiController.SetEffects(effects);
    }
    
    public void TakeDamage(float damage) {
        if (timeSinceDamage <= InvulnerabilityTime) {
            return;
        }
        if (CheckForEffect(EffectType.Invincible)) {
            return;
        }
        
        if (CheckForEffect(EffectType.OneHealth)) {
            damage *= 1.25f; // +25% damage taken
        }
        
        health -= damage;
        timeSinceDamage = 0;
        
        GameObject.FindWithTag("HUD").GetComponent<UIController>().UpdateHealth(health);
        
        if (health <= 0) {
            GameOver();
        }
    }
    
    public void GameOver() {
        Destroy(gameObject);
        uiController.GameOver();
    }
    
    public void OnTriggerEnter2D(Collider2D bullet) {
        if (bullet.CompareTag("Bullet")) {
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (!bulletController.isPlayer) {
                TakeDamage(bulletController.damage);
                if (!bulletController.piercing) {
                    Destroy(bullet.gameObject);
                }
            }
        }
    }
    
    public void Haunt() {
        ActiveEffect effect = ChooseEffect();
        if (effect == null) {
            uiController.SetTopText("No Effect", "Have a nice day!", 1f);
            return;
        }
        AddEffect(effect);
        uiController.SetTopText(effect.IsHaunt ? "Haunted!" : "POWERED UP!", effect.Text + "!", 1f);
    }
    
    public ActiveEffect ChooseEffect() {
        
        // Haunts
        if (Random.value <= HauntChance) {
            float r = Random.value;
            if (r <= 0.075f) {
                return new InvertedControls();
            } if (r <= 0.25f) {
                return new InvertedAiming();
            }
            // Disabled due to points not being implemented
            // if (r <= 0.4f) {
            //     return new NoPoints();
            // }
            if (r <= 0.4f) {
                return new ReducedDamage();
            }
             if (r <= 0.55f) {
                return new NoShoot();
            }
             if (r <= 0.65f) {
                return new OneHealth();
            }
             if (r <= 0.75f) {
                return new Stun();
            }
        }
        
        // Power Ups
        else {
            float r = Random.value;
            if (r <= 0.2f) {
                return new ExtraSpeed();
            } if (r <= 0.4f) {
                return new Invincibility();
            } if (r <= 0.6f) {
                return new ReducedEnemySpeed();
            } if (r <= 1f) {
                return new PiercingBullets();
            }
        }
        
        return null;
    }
    
    public void AddEffect(ActiveEffect effect) {
        for (int i = 0; i < effects.Length; i++) {
            if (effects[i] == null) {
                effects[i] = effect;
                uiController.SetEffects(effects);
                return;
            }
        }
    }
}
