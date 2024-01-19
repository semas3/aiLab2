using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;
using Unity.MLAgents.Sensors;

public class AgentPusher : Agent
{
    float m_LateralSpeed = 0.15f;
    float m_ForwardSpeed = 0.5f;

    public bool doorIsOpened = false;
    public float minDistance = 15f;
    float stepsCountWithoutComingCloser = 10;
    public Vector3 doorVector = Vector3.zero;
    private Vector3 diff = new Vector3(0f, 0.5f, 6.29f);

    public GameEnvController gameController;

    [HideInInspector]
    public Rigidbody agentRb;


    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;
    }
    public void setAgentReward(float reward)
    {
        SetReward(reward);
    }

    public void addAgentReward(float reward)
    {
        AddReward(reward);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);

        float newDist;

        if (doorIsOpened)
        {
            newDist = (doorVector - transform.localPosition).magnitude;
        }
        else
        {
            var itemPosition = gameController.buttonsDistributor.prefabsPool[0].transform.localPosition + diff;
            newDist = (itemPosition - transform.localPosition).magnitude;
        }

        if (newDist < minDistance)
        {
            addAgentReward(10f * (minDistance - newDist));
            minDistance = newDist;
            stepsCountWithoutComingCloser = 10;
            return;
        }

        --stepsCountWithoutComingCloser;
        if (stepsCountWithoutComingCloser < 0)
        {
            addAgentReward(-1f);
        }
        return;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = new Vector3(0f, 0.5f, 6.29f);
        sensor.AddObservation(gameController.buttonsDistributor.prefabsPool[0].transform.localPosition + diff);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(gameController.let1.transform.localPosition + diff);
        sensor.AddObservation(gameController.let2.transform.localPosition + diff);
        sensor.AddObservation(doorIsOpened);
        sensor.AddObservation(doorVector);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        doorIsOpened = false;
        stepsCountWithoutComingCloser = 10;
        Vector3 diff = new Vector3(0f, 0.5f, 6.29f);
        doorVector = gameController.buttonsDistributor.prefabsPool[0].transform.localPosition + diff;
        minDistance = 15f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == gameController.let1 ||
            collision.gameObject == gameController.let2)
            addAgentReward(-5f);

    }
}
