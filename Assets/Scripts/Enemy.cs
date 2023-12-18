using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int contactDamage;
    public float health;
    public Vector2 velocity;
    public bool followPlayer;
    public float speed; // used to follow player with certain speed
    public float slowFactor = 3f; // higher = more slowing when close to player
    public GameObject spawnOnDeath;
    public bool dieOnContact;
    
    protected GameObject player;
    

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Hi, I'm a new enemy!");
        player = GameObject.FindWithTag("Player");
    }
    
    public static float Sigmoid(float value) {
        return 1.0f / (1.0f + (float) Math.Exp(-value));
    }

    // Update is called once per frame
    public void Update()
    {
        if (player == null) return;
        
        // Update velocity using speed (point towards player)
        if (followPlayer) {
            Vector2 delta = player.transform.position - transform.position;
            float distance = delta.magnitude;
            velocity = delta.normalized * (speed * Sigmoid(distance / slowFactor));
        }
        
        // Handle effects
        if (player.GetComponent<PlayerController>().CheckForEffect(EffectType.ReducedEnemySpeed)) {
            velocity *= 0.2f;
        }
        
        // Rotate player
        if (velocity.magnitude > 0) {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
        
        // Move using velocity
        transform.Translate(velocity * (Time.deltaTime * LevelController.Speed));
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Contact damage
        if (other.gameObject.CompareTag("Player"))
        {
            // Contact damage
            if (contactDamage > 0) {
                Debug.Log("Dealing contact damage");
                other.gameObject.GetComponent<PlayerController>().TakeDamage(contactDamage);
            }
            
            // Die on contact
            if (dieOnContact) {
                Die(true);
            }
        }
        // TODO: Keep doing contact damage if the player stays within radius
        
        // Damage from player bullets
        if (other.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = other.gameObject.GetComponent<BulletController>();
            if (bullet.isPlayer) { // Note: Could allow friendly fire between enemies
                TakeDamage(bullet.damage);
                if (!bullet.piercing) {
                    Destroy(other.gameObject);
                }
            }
            
        }

    }
    
    public void TakeDamage(float damage) {
        health -= damage;
        Debug.Log("Health: " + health);
        
        if (health <= 0) {
            Die();
        }
    }
    
    public void Die(bool noHaunt = false) {
        Destroy(gameObject);
        if (!noHaunt) player.GetComponent<PlayerController>().Haunt();
        
        // Explosion
        if (spawnOnDeath != null) {
            Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
        }
        
    }
}
    