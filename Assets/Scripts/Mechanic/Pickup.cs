using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private BoxCollider bc;
    private bool isPicked;
    private float time;

    public event EventHandler OnPickup;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        isPicked = false;
    }

    void Update()
    {
        if (isPicked)
        {
            PickMechanic(false);
        }

        if (isPicked && Time.time >= time + 10f)
        {
            PickMechanic(true);
            isPicked = false;
        }
    }

    public void Pick()
    {
        if (!isPicked)
        {
            isPicked = true;
            time = Time.time;
            OnPickup?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PickMechanic(bool status)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(status);
            bc.enabled = status;
        }
    }
}
