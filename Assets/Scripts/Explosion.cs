using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float time = 1.0f;
    public float radius = 4.0f;
    public float playerRadius = 2.0f;
    public float enemyDamage = 250f;
    public float playerDamage = 50f;
    
    public static float EffectTime = 0.1f;
    
    private float timeAlive = 0f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(radius*2, radius*2, 1);
        
        // Detect all colliding objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, radius);
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject.CompareTag("Player")) {
                if ((collider.gameObject.transform.position - gameObject.transform.position).magnitude < playerRadius) {
                    collider.gameObject.GetComponent<PlayerController>().TakeDamage(playerDamage);
                }
            }
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(enemyDamage);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= time) {
            Destroy(gameObject);
        }
    }
}
