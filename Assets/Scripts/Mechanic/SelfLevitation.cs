using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfLevitation : MonoBehaviour
{
    private Vector3 sineWave;
    private float amplitude;

    [SerializeField, Header("Distance"), Range(0.0f, 2f)]
    private float distanceModifier;

    [SerializeField, Header("Speed"), Range(0.0f, 2f)]
    private float frequency;

    private float orginalPosition;

    void Start()
    {
        //frequency = 1f;
        //distanceModifier = 1f;

        orginalPosition = transform.position.y;
    }

    void Update()
    {
        amplitude = Mathf.Sin(Time.time / frequency) * distanceModifier;
        sineWave = new Vector3(transform.position.x, amplitude + orginalPosition, transform.position.z);
        transform.position = sineWave;
    }
}
