using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Vector3 topDes, botDes;

    private RectTransform top, bot;
    private Transform boss;
    private Vector3 destination;
    private float speed, time;
    private bool go, hasArrived, gone;
    private GameObject conversation, end;

    void Start()
    {
        conversation = GameObject.Find("Conversation");
        end = GameObject.Find("End");
        conversation.SetActive(false);
        end.SetActive(false);
        top = GameObject.Find("Cinematic").transform.Find("Top").GetComponent<RectTransform>();
        bot = GameObject.Find("Cinematic").transform.Find("Bot").GetComponent<RectTransform>();

        topDes = top.position;
        botDes = bot.position;

        top.position = new Vector3(topDes.x, topDes.y + 192, topDes.z);
        bot.position = new Vector3(botDes.x, botDes.y - 192, botDes.z);


        boss = GameObject.Find("Boss").transform;
        destination = new Vector3(0, -3.5f, 27);
        go = false;
        hasArrived = false;
        gone = false;

        speed = 1;
    }

    void Update()
    {
        if (go)
        {
            top.position = Vector3.Lerp(top.position, topDes, Time.deltaTime * 1);
            bot.position = Vector3.Lerp(bot.position, botDes, Time.deltaTime * 1);
        }

        if (go && Time.time >= time + 2f)
        {
            boss.position = Vector3.Lerp(boss.position, destination, Time.deltaTime * speed);
        }

        if (boss.position.y >= destination.y - 1)
        {
            hasArrived = true;
        }

        if (hasArrived)
        {
            conversation.SetActive(true);
        }

        if (conversation.GetComponent<Conversation>().End)
        {
            conversation.SetActive(false);

            top.position = Vector3.Lerp(top.position, new Vector3(topDes.x, topDes.y + 512, topDes.z), Time.deltaTime * 1);
            bot.position = Vector3.Lerp(bot.position, new Vector3(botDes.x, botDes.y - 512, botDes.z), Time.deltaTime * 1);

            boss.position = Vector3.Lerp(boss.position, new Vector3(0, -100f, 27), Time.deltaTime * speed);

            gone = true;
        }

        if (gone)
        {
            end.SetActive(true);
        }
    }

    public void Go()
    {
        go = true;
        time = Time.time;
    }

    public bool Gone { get { return gone; } }
}
