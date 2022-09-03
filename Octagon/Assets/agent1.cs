
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators; // Related to action buffers

public class agent1 : Agent // The agent1 has to be the exact same name as the name of this file, and maybe even the object that it inherits
// Agent is a class within Unity
{
    Rigidbody rBody; // In order to use the roller boll as a physical body we need to import rigid body
    void Start () {
        rBody = GetComponent<Rigidbody>(); // At the start of the episode get the rigid body
    }

    // Define lick ports as Transforms, in Unity Transform component has three visible properties. Position, rotation and scale.
    // Every game object has a Transform
    public Transform LickPort1;
    public Transform LickPort2;
    public Transform LickPort3;
    public Transform LickPort4;
    public Transform LickPort5;
    public Transform LickPort6;
    public Transform LickPort7; 
    public Transform LickPort8;

    // public Transform[] LickPorts = new Transform[2]; // Define a public, array of type Transforms of length 3
    public List<Transform> LickPorts;
    public Transform RewardingPort;
    public int ActionCounter; // To keep track of time spent in arena and trial length

    public override void OnEpisodeBegin() // Called every time a new episode is started or new game
    {

        // Set the action countet to 0, also consider this as t = 0
        ActionCounter = 0;

        //Set the rewarding port
        var index = Random.Range(0, 8); // Create a port index randomly
        RewardingPort = LickPorts[index];

        // Chnage the colour of the rewarding port
        RewardingPort.GetComponent<Renderer>().material.color = new Color(0, 255, 0); // The CS

        // Set the agent velocity to zero at the start of the trial and place the agent in a random place within the Octagon
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(Random.value * 8, 
                                                   15.5f, 
                                                   Random.value * -5);
        
    }

    public override void CollectObservations(VectorSensor sensor) // Give the agent information
    {
        // Port positions as obersvations
        sensor.AddObservation(LickPort1.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort2.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort3.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort4.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort5.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort6.localPosition); // Feed the target position to the agent 3x
        sensor.AddObservation(LickPort7.localPosition); // Feed the target position to the agent 3x 
        sensor.AddObservation(LickPort8.localPosition); // Feed the target position to the agent 3x

        // Where is the agent
        sensor.AddObservation(this.transform.localPosition); // Tell the agent where it is 3x 

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x); // Also tell the agent how fast it is moving 1x 
        sensor.AddObservation(rBody.velocity.z); // 1x 
    }

    public float forceMultiplier = 10; // speed of agent
    public override void OnActionReceived(ActionBuffers actionBuffers) // Afer I get observations, what is my output?
    {
        ActionCounter = ActionCounter + 1;
        
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero; // Vector 3 means 3dimensions
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        controlSignal.y = actionBuffers.ContinuousActions[2]; // adding a rotational turn 
        rBody.AddForce(controlSignal * forceMultiplier); // push the agent with some force

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, RewardingPort.localPosition);

        // Reached target
        if (distanceToTarget < 3.42f) // if goal achieved
        {
            SetReward(1.0f);
            RewardingPort.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
            EndEpisode();
        }

        // Fell off platform - but maybe change to time in trial
        else if (ActionCounter > 3000)
        {
            RewardingPort.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
            EndEpisode();
        }
    }

    // Heuristics allow humans to play the game to test in the environment, by utilise the action buggers
    public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = Input.GetAxis("Horizontal");
            continuousActionsOut[1] = Input.GetAxis("Vertical");
            // continuousActionsOut[2] = Input.GetAxis("Vertical");
        }    
}