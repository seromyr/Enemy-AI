using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNextObjective : MonoBehaviour
{

    public bool followPlayer;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float distance;

    private SpriteRenderer spr;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Vector3 _targetDir = (target.position - transform.position).normalized;
        _targetDir.y = 0;
        Quaternion _lookDir = Quaternion.LookRotation(_targetDir);
        Quaternion _newLookDir = Quaternion.Euler(_lookDir.eulerAngles.x + 90, _lookDir.eulerAngles.y - 90, _lookDir.eulerAngles.z);
        transform.rotation = (_newLookDir);

        if (followPlayer)
        {
            transform.position = (player.position + _targetDir * 2 + new Vector3(0, 0.5f, 0));
        }

        distance = (target.position - transform.position).magnitude;

        if ((target.position - transform.position).magnitude < 2.5f)
        {
            spr.enabled = false;
        }
        else
        {
            spr.enabled = true;
        }
    }
}
