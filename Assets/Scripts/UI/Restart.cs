using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    private Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(RestartScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
