using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOnOff : MonoBehaviour
{
    [SerializeField]
    private bool isOn;
    private Button button;
    private GameObject child;

    void Start()
    {
        button = GetComponent<Button>();
        isOn = false;
        button.onClick.AddListener(ToggleMeOnOff);
        child = transform.GetChild(0).gameObject;
        ToggleMeOnOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleMeOnOff()
    {
        isOn = isOn ? false : true;
        child.SetActive(isOn);
    }
}
