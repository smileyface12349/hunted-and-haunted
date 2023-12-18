using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 8;
    public float angle;
    public float damage;
    public bool piercing;
    public bool isPlayer;
    
    public static float BoundsDespawnRadius = 10; // Despawns if out of bounds by this much

    // Start is called before the first frame update
    void Start()
    {
        // Rotate
        gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    // Update is called once per frame
    void Update()
    {
        // move bullet in direction of angle
        // gameObject.transform.Translate(speed * Mathf.Cos(angle) * Time.deltaTime * LevelController.Speed, speed * Mathf.Sin(angle) * Time.deltaTime * LevelController.Speed, 0);
        
        // move forwards
        gameObject.transform.Translate(speed * Time.deltaTime * LevelController.Speed, 0, 0);
        
        // Despawn if out of bounds
        if (gameObject.transform.position.x < -(LevelController.Bounds.x+BoundsDespawnRadius)) {
            Destroy(gameObject);
        }
        if (gameObject.transform.position.x > LevelController.Bounds.x+BoundsDespawnRadius) {
            Destroy(gameObject);
        }
        if (gameObject.transform.position.y < -(LevelController.Bounds.y+BoundsDespawnRadius)) {
            Destroy(gameObject);
        }
        if (gameObject.transform.position.y > LevelController.Bounds.y+BoundsDespawnRadius) {
            Destroy(gameObject);
        }
        
        
    }
}
