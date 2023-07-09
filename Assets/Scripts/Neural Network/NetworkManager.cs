using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public GameObject genericVehicle;
    GameObject[] vehicleGeneration;
    float timeTaken;


    // Start is called before the first frame update
    void Start()
    {
        CreateGeneration(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeTaken += Time.deltaTime;
        bool finished = true;
        foreach (GameObject network in vehicleGeneration){
            if (!network.GetComponent<NetworkControl>().hasCrashed){
                finished = false;
                break;
            }
        }

        if (finished || timeTaken > 10){
            CreateGeneration(true);
        }
    }

    void DeleteGeneration(){
        foreach (GameObject vehicle in vehicleGeneration){
            Destroy(vehicle);
        }
    }

    void CreateGeneration(bool learn){
        if (learn){
            List<float[][][]> newWeights;
            List<float[][]> newBiases;
            Learn(out newWeights, out newBiases);
            for (int i = 0; i < vehicleGeneration.Length; i++){
                vehicleGeneration[i].GetComponent<NetworkControl>().Controller.overrideValues(newWeights[i], newBiases[i]);
                
                vehicleGeneration[i].transform.position = new Vector3(0f, 1f, 2f);
                vehicleGeneration[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            for (int i = 0; i < vehicleGeneration.Length; i++){
                vehicleGeneration[i].GetComponent<NetworkControl>().hasCrashed = false;
            }
        }
        else{
            vehicleGeneration = new GameObject[100];
            for (int i = 0; i < vehicleGeneration.Length; i++){
                vehicleGeneration[i] = Instantiate(genericVehicle, new Vector3(0f, 1f, 2f), Quaternion.Euler(0, 0, 0));
            }
        }

        timeTaken = 0.0f;
    }

    void Learn(out List<float[][][]> newWeights, out List<float[][]> newBiases){
        newWeights = new List<float[][][]>();
        newBiases = new List<float[][]>();

        GameObject[] newNetworks = new GameObject[vehicleGeneration.Length];
        GameObject[] TwoFittestVehicles = new GameObject[2]{vehicleGeneration[0], vehicleGeneration[1]};
        float[] fitnesses = new float[2];

        foreach (GameObject network in vehicleGeneration){
            NetworkControl netController = network.GetComponent<NetworkControl>();

            if (netController.GetFitness() > fitnesses[0] && (fitnesses[0] <= fitnesses[1])){
                TwoFittestVehicles[0] = network;
                fitnesses[0] = netController.GetFitness();
            }
            else if (netController.GetFitness() > fitnesses[1]){
                TwoFittestVehicles[1] = network;
                fitnesses[1] = netController.GetFitness();
            }
        }

        newWeights.Add(TwoFittestVehicles[1].GetComponent<NetworkControl>().Controller.weights);
        newBiases.Add(TwoFittestVehicles[1].GetComponent<NetworkControl>().Controller.biases);
        newWeights.Add(TwoFittestVehicles[1].GetComponent<NetworkControl>().Controller.weights);
        newBiases.Add(TwoFittestVehicles[1].GetComponent<NetworkControl>().Controller.biases);

        NeuralNet[] fittestCrossover = TwoFittestVehicles[0].GetComponent<NetworkControl>().Controller.Crossover(TwoFittestVehicles[1].GetComponent<NetworkControl>().Controller);

        for (int i = 2; i < vehicleGeneration.Length / 2; i++){
            newWeights.Add(vehicleGeneration[i].GetComponent<NetworkControl>().Controller.weights);
            newBiases.Add(vehicleGeneration[i].GetComponent<NetworkControl>().Controller.biases);
        }

        for (int i = vehicleGeneration.Length / 2; i < vehicleGeneration.Length; i++){
            newWeights.Add(fittestCrossover[1].Mutate().weights);
            newBiases.Add(fittestCrossover[1].Mutate().biases);
        }

        Debug.Log($"{fitnesses[0]} {fitnesses[1]}");
    }
}
