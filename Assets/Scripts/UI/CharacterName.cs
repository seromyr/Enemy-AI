using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterName : MonoBehaviour
{
    private Canvas canvas;
    private Text text;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        text = GetComponentInChildren<Text>();

        //text.text = transform.parent.name;
    }

    private void Update()
    {
        text.text = transform.parent.name;
    }

    private void LateUpdate()
    {
        canvas.transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
