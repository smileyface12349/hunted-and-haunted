using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    
    public void Set(float health) {
        gameObject.GetComponent<TextMeshProUGUI>().text = "Health: " + health;
    }
    

}