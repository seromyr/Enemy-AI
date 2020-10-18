using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateReport : MonoBehaviour
{
    private Text text;

    private AI _AI;
    void Start()
    {
        text = transform.GetComponentInChildren<Text>();
        _AI = GameObject.Find("Enemies").transform.Find(gameObject.name).GetComponent<AI>();
    }

    void Update()
    {
        text.text = _AI.Message;
        switch (_AI.CurrentState)
        {
            case AIState.Patrolling:
                text.color = Color.white;
                break;
            case AIState.Attacking:
                text.color = Color.red;
                break;
            case AIState.Chasing:
                text.color = Color.yellow;
                break;
            case AIState.Searching:
                text.color = Color.magenta;
                break;
            case AIState.Retreating:
                text.color = Color.green;
                break;
            case AIState.Panic:
                text.color = Color.blue;
                break;
        }
    }
}
