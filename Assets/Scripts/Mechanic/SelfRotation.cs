using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotation : MonoBehaviour
{
    [SerializeField, Header("Rotation Maximum Speed")]
    private float maxSpeed;

    [SerializeField, Header("Rotation Acceleration Speed")]
    private float accelerationSpeed;

    [SerializeField, Header("Rotation Speed")]
    private float speed;

    [SerializeField, Header("Duration")]
    private float duration;

    public float FeedCoffee { set { duration += value;  } }

    // Start is called before the first frame update
    void Start()
    {
        maxSpeed = 50;
        speed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        speed += Time.deltaTime * accelerationSpeed;
        if (speed >= maxSpeed) speed = maxSpeed;

        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}
