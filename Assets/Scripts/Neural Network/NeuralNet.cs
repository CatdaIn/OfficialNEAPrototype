using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet
{
    private int[] layers;
    private float[][] neurons;
    public float[][] biases;
    public float[][][] weights;
    private float fitness;

    public NeuralNet(int[] inLayers){
        this.layers = inLayers;

        initNeurons();
        initBiases();
        initWeights();
    }

    private void initNeurons(){

        neurons = new float[layers.Length][]; // Layers.length is the number of layers of neurons

        for (int i = 0; i < layers.Length; i++){

            neurons[i] = new float[layers[i]]; // layers[i] is the number of neurons in a given layer

            for (int j = 0; j < layers[i]; j++){
                neurons[i][j] = 0; // All neurons default value will be 0, before the network is run
            }
        }
    }

    // Almost identical to initNeurons, except the values are random
    private void initBiases(){
        biases = new float[layers.Length][]; // Layers.length is the number of layers of neurons

        for (int i = 0; i < layers.Length; i++){

            biases[i] = new float[layers[i]]; // layers[i] is the number of neurons in a given layer

            for (int j = 0; j < layers[i]; j++){
                biases[i][j] = UnityEngine.Random.Range(-0.5f, 0.5f); // Biases will start with a random value between -1/2 and 1/2
            }
        }
    }

    private void initWeights(){
        weights = new float[layers.Length - 1][][]; // Layers.length is the number of layers of neurons, so number of layers of edges is layers.length - 1

        for (int i = 1; i < layers.Length; i++){
            weights[i - 1] = new float[layers[i]][]; // It is easier to do all the weights into each node when calculating, so the end node is used as 2nd index

            for (int j = 0; j < layers[i]; j++){
                weights[i-1][j] = new float[layers[i-1]]; // Finally, the start node of an edge is used as the final index

                for (int k = 0; k < layers[i-1]; k++){
                    weights[i-1][j][k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
            }
        }
    }

    public float[] forwardPropagation(float[] inputs){
        neurons[0] = inputs;

        for (int i = 1; i < layers.Length; i++){ // We skip layers[0], as that is just the input layer
            for (int j = 0; j < neurons[i].Length; j++){
                for (int k = 0; k < neurons[i-1].Length; k++){
                    neurons[i][j] += neurons[i - 1][k] * weights[i-1][j][k];
                }
                neurons[i][j] += biases[i][j];
                neurons[i][j] = tanhActivation(neurons[i][j]);
            }
        }

        return neurons[neurons.Length - 1];
    }

    private float tanhActivation(float value){
        return (float)System.Math.Tanh(value);
    }

    public void setFitness(float value){
        this.fitness = value;
    }

    public NeuralNet[] Crossover(NeuralNet otherNetwork){
        NeuralNet child1 = otherNetwork.DuplicateNetwork();
        NeuralNet child2 = this.DuplicateNetwork();
        float randNum;
        float temp;
        
        // Randomly swap weights
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    randNum = UnityEngine.Random.Range(0f, 1f);
                    if (randNum < 0.5f){
                        temp = child2.weights[i][j][k];
                        child2.weights[i][j][k] = child1.weights[i][j][k];
                        child1.weights[i][j][k] = temp;
                    }
                }
            }
        }

        // Randomly swap biases
        for (int i = 0; i < biases.Length; i++){
            for (int j = 0; j < biases[i].Length; j++){
                randNum = UnityEngine.Random.Range(0f, 1f);
                if (randNum < 0.5f){
                    temp = child2.biases[i][j];
                    child2.biases[i][j] = child1.biases[i][j];
                    child1.biases[i][j] = temp;
                }
            }
        }

        return new NeuralNet[]{child1, child2};
    }

    public void overrideValues(float[][][] weights, float[][] biases){
        this.weights = weights;
        this.biases = biases;
    }

    public NeuralNet Mutate(){
        NeuralNet result = DuplicateNetwork();
        // Multiplier represents the percentage any value can change by
        float multiplier = 0.5f;

        // Chance represents the probability of a mutation for a given value
        float chance = 0.7f;

        float randNum = 0.0f;

        // Modify the biases
        for (int i = 0; i < result.biases.Length; i++){
            for (int j = 0; j < result.biases[i].Length; j++){
                randNum = UnityEngine.Random.Range(0f, 1f);
                if (randNum < chance){
                    result.biases[i][j] *= UnityEngine.Random.Range(1 - multiplier, 1 + multiplier);
                }
            }
        }

        // Modify the weights
        for (int i = 0; i < result.weights.Length; i++){
            for (int j = 0; j < result.weights[i].Length; j++){
                for (int k = 0; k < result.weights[i][j].Length; k++){
                    randNum = UnityEngine.Random.Range(0f, 1f);
                    if (randNum < chance){
                        result.weights[i][j][k] *= UnityEngine.Random.Range(1 - multiplier, 1 + multiplier);
                    }
                }
            }
        }

        return result;
    }

    public NeuralNet DuplicateNetwork(){
        NeuralNet copy = new NeuralNet(layers);
        copy.overrideValues(this.weights, this.biases);
        
        return copy;
    }

    public int CompareFitness(NeuralNet other){
        if (other == null || other.fitness < this.fitness){
            return 1;
        }
        else if (other.fitness > this.fitness){
            return -1;
        }
        else{
            return 0;
        }
    }
}
