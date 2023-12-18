using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject objectToFollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToFollow != null) {
            gameObject.transform.position = new Vector3(
                objectToFollow.transform.position.x,
                objectToFollow.transform.position.y,
                -5
            );
        }
    }
}
