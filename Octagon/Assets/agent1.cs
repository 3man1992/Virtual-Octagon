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



    public Transform[] LickPorts = new Transform [3]; // Define a public, array of type Transforms of length 3
    // public Transform[] LickPorts = {LickPort6, LickPort7, LickPort8};
    public Transform RewardingPort;
    public int Counter = 0;
    public int ActionCounter;

    public override void OnEpisodeBegin() // Called every time a new episode is started or new game
    {
        
        ActionCounter = 0;
        
        if (Counter > 2)
        {
            Counter = 0;
        }
        
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(Random.value * 8, 
                                                   15.5f, 
                                                   Random.value * -5);

        LickPorts[0] = LickPort6;
        LickPorts[1] = LickPort7;
        LickPorts[2] = LickPort8;

        // int Port = Random.Range(0, 3); # Choose a random port
        RewardingPort = LickPorts[Counter];
        Counter = Counter + 1;

       

        // Choose the rewarding port
        // ArrayList list = new ArrayList();
        // list.Add(LickPort7)

        // Transform[] Ports = {LickPort7, LickPort6, LickPort8}
        // // Transform RewardingPort = 

    }

    public override void CollectObservations(VectorSensor sensor) // Give the agent information
    {
        // Port positions as obersvations
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
        rBody.AddForce(controlSignal * forceMultiplier); // push the agent with some force

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, RewardingPort.localPosition);

        // Reached target
        if (distanceToTarget < 3.42f) // if goal achieved
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform - but maybe change to time in trial
        else if (ActionCounter > 2000)
        {
            EndEpisode();
        }
    }

    // Heuristics allow humans to play the game to test in the environment, by utilise the action buggers
    public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = Input.GetAxis("Horizontal");
            continuousActionsOut[1] = Input.GetAxis("Vertical");
        }    
}