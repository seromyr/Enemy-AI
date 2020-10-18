using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplateProjects;
using Constants;
using System.Security.AccessControl;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _health;
    private float Health { get { return _health; } }

    private float maxHp, minHp;

    [SerializeField]
    private Image bloodSplatterEffect;
    private Color bloodEffectColor;
    private bool bloodEffect;

    private Image hpBarFill, hpBarFrame;
    [SerializeField]
    private Gradient hpBarFillGradient;

    private bool isGod;
    private PlayerController controller;
    private GameObject subCam;
    private SimpleCameraController camController;
    private SkinnedMeshRenderer ybot;
    [SerializeField]
    private Material godForm, humanForm, hunterForm, prevForm;
    private CapsuleCollider capsuleCollider;
    private Rigidbody _rigidbody;

    private Form form;

    private float time, hunterDuration;

    public event EventHandler<OnFormChangeArgs> OnFormChange;

    void Start()
    {
        form = Form.Human;
        maxHp = 100;
        minHp = 0;
        _health = maxHp * 0.8f;

        hpBarFill = transform.Find("SpatialHP").transform.Find("Fill").GetComponent<Image>();
        hpBarFrame = transform.Find("SpatialHP").transform.Find("Frame").GetComponent<Image>();
        bloodSplatterEffect = GameObject.Find("BloodSplatter").GetComponent<Image>();

        bloodEffect = false;
        bloodEffectColor = bloodSplatterEffect.color;
        bloodEffectColor.a = 0;
        bloodSplatterEffect.color = bloodEffectColor;

        controller = GetComponent<PlayerController>();

        // Cheat section
        {
            controller = GetComponent<PlayerController>();
            controller.OnSpaceBarPressed += OnSpaceBarPressed;
            subCam = GameObject.Find("CM vcam1");
            camController = GameObject.Find("Main Camera").GetComponent<SimpleCameraController>();
            camController.enabled = false;
            ybot = GameObject.Find("Alpha_Surface").GetComponent<SkinnedMeshRenderer>();
            humanForm = ybot.material;
            capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        hunterDuration = 20;

    }

    void Update()
    {
        HpLimiter();
        ShowHpInHpBar();
        ShowBloodEffect();
        HunterFormMonitor();
    }

    private void HpLimiter()
    {
        if (_health >= maxHp)
        {
            _health = maxHp;
        }

        if (_health <= minHp)
        {
            _health = minHp;
        }
    }

    private void ShowHpInHpBar()
    {
        hpBarFill.fillAmount = _health / 100;
        hpBarFill.color = hpBarFillGradient.Evaluate(_health / 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Acid"))
        {
            bloodEffect = true;
        }

        else if (other.CompareTag("Pickup"))
        {
            TakeDamage(-10);
            other.GetComponent<Pickup>().Pick();
        }

        // This is for boss scene demo, not for AI scene
        else if (other.CompareTag("Boss"))
        {
            transform.GetComponent<PlayerController>().ToggleController(false);
            other.gameObject.GetComponent<Boss>().Go();
            for (int i = 0; i < other.transform.childCount; i++)
            {
                other.transform.GetChild(i).gameObject.SetActive(false);
            }

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        else if (other.CompareTag("Relic"))
        {
            other.GetComponent<Pickup>().Pick();
            form = Form.Hunter;
            ybot.material = hunterForm;
            // Invoke this event if it's not null (null conditional operator). Courtesy of C# 6.0 via Code Monkey
            OnFormChange?.Invoke(this, new OnFormChangeArgs { playerForm = form, fear = true });

            time = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Acid"))
        {
            TakeDamage(0.5f);
        }

        else if (other.CompareTag("Fountain"))
        {
            TakeDamage(-1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Acid"))
        {
            bloodEffect = false;
        }
    }

    private void ShowBloodEffect()
    {
        if (bloodEffect)
        {
            bloodEffectColor.a = Mathf.Sin(Time.time * 10);

            hpBarFrame.color = Color.Lerp(Color.white, Color.red, Mathf.Sin(Time.time * 20));
        }
        else
        {
            bloodEffectColor.a = 0;
            hpBarFrame.color = Color.white;
        }

        bloodSplatterEffect.color = bloodEffectColor;
    }

    public void TakeDamage(float amount)
    {
        _health -= amount;
    }

    private void OnSpaceBarPressed(object sender, EventArgs e)
    {
        isGod = isGod ? false : true;
        GodModeToggle(isGod);
    }

    private void GodModeToggle(bool onOff)
    {
        if (onOff)
        {
            prevForm = ybot.material;
            Debug.Log("You are now a god");
            subCam.SetActive(false);
            camController.enabled = true;
            ybot.material = godForm;
            capsuleCollider.enabled = false;
            _rigidbody.isKinematic = true;
        }
        else
        {
            Debug.Log("You are no longer a god");
            subCam.SetActive(true);
            camController.enabled = false;
            ybot.material = prevForm;
            capsuleCollider.enabled = true;
            _rigidbody.isKinematic = false;
        }
    }

    private void HunterFormMonitor()
    {
        if (form == Form.Hunter && Time.time >= time + hunterDuration)
        {
            form = Form.Human;
            ybot.material = humanForm;
            OnFormChange?.Invoke(this, new OnFormChangeArgs { playerForm = form, fear = false });
        }
    }
}

// Extra information to pass when fire OnFormChange event
public class OnFormChangeArgs : EventArgs
{
    public Form playerForm;
    public bool fear;
} 

