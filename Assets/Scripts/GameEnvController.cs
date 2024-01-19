using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class GameEnvController : MonoBehaviour
{
    public int buttonsOnEpisode = 4;
    public int boxesOnEpisode = 3;

    private AgentPusher agent;
    public GridedDistributor buttonsDistributor;
    public GridedDistributor boxDistributor;
    public GridedDistributor agentsDistributor;
    public Door door;
    public GameObject let1, let2;
    [HideInInspector]
    public Vector3 let1Pos, let2Pos;
    public MeshCollider goal;

    void Start()
    {
        let1Pos = let1.transform.position;
        let2Pos = let2.transform.position;
        ResetScene();
    }

    void ResetScene()
    {
        var buttons = buttonsDistributor.Respawn(buttonsOnEpisode);
        boxDistributor.Respawn(boxesOnEpisode);
        var activators = new DoorActivator[buttons.Length];
        for (var i = 0; i < buttons.Length; i++)
        {
            activators[i] = buttons[i].GetComponent<Button>();
        }

        door.ResetActivators(activators);

        agent = agentsDistributor.Respawn(1)[0].GetComponent<AgentPusher>();
    }
    public void OnGoalTriggered()
    {
        if (!agent.doorIsOpened)
        {
            agent.addAgentReward(500f);
            agent.doorIsOpened = true;
            agent.minDistance = 15f;

            if (agent.transform.localPosition[0] > 0f)
            {
                agent.doorVector = new Vector3(4f, 0.5f, 13f);
                return;
            }

            agent.doorVector = new Vector3(-4f, 0.5f, 13f);
        }
    }

    public void OnDoorOpened()
    {
        agent.addAgentReward(350f);
        agent.doorIsOpened = false;
        ResetScene();
        agent.EndEpisode();
    }

    void FixedUpdate()
    {
    }
}
