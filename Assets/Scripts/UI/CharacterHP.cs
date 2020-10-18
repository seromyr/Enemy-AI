using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHP : MonoBehaviour
{
    private Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        canvas.transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
