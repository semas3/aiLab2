using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GoalTrigger : MonoBehaviour
{
    public UnityEvent finished;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent")) finished.Invoke();
    }
}
