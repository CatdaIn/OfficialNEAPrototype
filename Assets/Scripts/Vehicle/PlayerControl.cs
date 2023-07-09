using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float speedConstant = 0.05f;
    private float turnConstant = 3.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        this.transform.Translate(Vector3.forward * forwardInput * speedConstant, Space.Self);

        if (forwardInput != 0){
            this.transform.Rotate(Vector3.up * horizontalInput * turnConstant, Space.World);
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.name == "Wall"){
            this.speedConstant = 0.0f;
        }
        Debug.Log(other.name);
    }
}
