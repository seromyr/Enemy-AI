using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    [SerializeField]
    private Material opaqueMaterial;
    [SerializeField]
    private Material transparentMaterial;

    public bool showRay;

    [SerializeField]
    private Transform player;
    private Vector3 raycastStart, raycastDirection;

    [SerializeField]
    private Vector3 raycastStartOffset;
    [SerializeField]
    private Vector3 raycastEndOffset;
    [SerializeField]
    private float raycastLength;
    private Vector3 raycastEnd;

    private RaycastHit hit;

    [SerializeField]
    private Transform hitTransform;

    [SerializeField]
    private Transform currentHitObject;
    [SerializeField]
    private Transform lastHitObject;

    private List<Transform> hitList;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        hitList = new List<Transform>();
    } 
    void Update()
    {
        UpdateRay();

        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Wall"))
            {
                DrawRay(Color.red);
                if (!hitList.Contains(hit.transform)) //(hit.transform != currentHitObject)
                {
                    hitList.Add(hit.transform);
                    hitList[hitList.Count - 1].GetComponent<MeshRenderer>().material = transparentMaterial;
                    currentHitObject = hit.transform;
                }

            }
            else if (hit.transform.CompareTag("Player"))
            {
                DrawRay(Color.blue);
            }
            else
            {
                DrawRay(Color.white);
            }
        }
        else
        {
            DrawRay(Color.black);
            if (hitList.Count > 0)
            {
                hitList[hitList.Count - 1].GetComponent<MeshRenderer>().material = opaqueMaterial;
                hitList.Remove(hitList[hitList.Count - 1]);
            }

            //if (currentHitObject != null)
            //{
            //    lastHitObject = currentHitObject;
            //    currentHitObject = null;
            //}

            //if (lastHitObject != null)
            //{
            //    lastHitObject.GetComponent<MeshRenderer>().material = opaqueMaterial;
            //}
        }
    }

    private void DrawRay(Color color)
    {
        Debug.DrawRay(raycastStart, raycastDirection, color);
    }

    private void UpdateRay()
    {
        raycastStart = transform.position + raycastEndOffset;
        raycastEnd = player.position + raycastStartOffset;
        raycastDirection = (raycastStart - raycastEnd) / raycastLength;

        hitTransform = hit.transform;

        Physics.Raycast(raycastStart, raycastDirection, out hit);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log(other.name);
        }
    }
}
