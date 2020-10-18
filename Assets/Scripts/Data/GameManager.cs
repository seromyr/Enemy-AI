using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Constants;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [SerializeField]
    private Player player;

    [SerializeField]
    private int playerHealth;

    private Phase phase;
    public Phase Phase { get { return phase; } }

    public event EventHandler OnPhaseChange;

    private void Awake()
    {
        // Make the Game Manager a Singleton
        Singleton_dinator();
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        phase = Phase.Hunt;

        player.OnFormChange += SwitchPhase;
    }
    private void Singleton_dinator()
    {
        if (gameManager == null)
        {
            DontDestroyOnLoad(gameObject);
            gameManager = this;
        }
        else if (gameManager != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

    }

    private void SwitchPhase(object sender, OnFormChangeArgs e)
    {
        Debug.Log("Game Manager ackhowledged that Player had changed form to " + e.playerForm);
        if (e.fear)
        {
            Debug.Log("Time to inflict some fear");
            phase = Phase.BeHunted;
        }
        else
        {
            Debug.Log("Fear is over");
            phase = Phase.Hunt;
        }
    }
}

