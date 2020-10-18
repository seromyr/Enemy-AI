using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicMechanic : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnLocations;

    private Pickup pickup;
    void Start()
    {
        transform.position = spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)].position;
        pickup = GetComponent<Pickup>();
        pickup.OnPickup += MoveToNewLocation;
    }

    private void MoveToNewLocation(object sender, EventArgs e)
    {
        transform.position = spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)].position;
    }
}
