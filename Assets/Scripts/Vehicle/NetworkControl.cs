using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkControl : MonoBehaviour
{
    private float[] distances;
    private int numCheckpoints;
    public bool hasCrashed;
    public NeuralNet Controller;
    private float speedConstant = 0.05f;
    private float turnConstant = 3.0f;
    private int nextCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        distances = new float[5];    
        hasCrashed = false;
        numCheckpoints = 0;
        nextCheckpoint = 1;

        Controller = new NeuralNet(new int[]{5, 10, 3});
    }

    // Update is called once per frame
    void Update()
    {
        // Handles the input to the network
        RaycastHit hit;
        for (int i = -2; i < 3; i++){
            Physics.Raycast(this.transform.position, Quaternion.Euler(0, i * 30, 0) * transform.forward, out hit);
            distances[i + 2] = hit.distance;
            // Debug.DrawRay(transform.position, Quaternion.Euler(0, i * 30, 0) * transform.forward * hit.distance, Color.red);
        }

        if(hasCrashed){
            Controller.setFitness(numCheckpoints);
            // Terminate this AI

        }
        else{
            float[] controlValues = Controller.forwardPropagation(distances);

            this.transform.Translate(Vector3.forward * controlValues[0] * speedConstant, Space.Self);

            if (controlValues[0] != 0){
                if (controlValues[2] > 0){
                    this.transform.Rotate(Vector3.up * Mathf.Abs(controlValues[1]) * turnConstant, Space.World);
                }
                else{
                    this.transform.Rotate(Vector3.up * -Mathf.Abs(controlValues[1]) * turnConstant, Space.World);
                }
            }
        }

    }

    // Fitness not implemented yet
    public int GetFitness(){
        return this.numCheckpoints;
    }

    void OnTriggerEnter(Collider other){
        if (other.name == "Wall"){
            this.hasCrashed = true;
        }
        else if (other.name == $"Checkpoint {nextCheckpoint}"){
            this.numCheckpoints ++;
            this.nextCheckpoint ++;
        }
        //Debug.Log(other.name);
    }
}
