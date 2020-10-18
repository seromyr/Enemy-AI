using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversation : MonoBehaviour
{
    private Button btn;
    private Text text;
    private Queue<string> dialog;

    private int i;
    void Start()
    {
        btn = transform.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(Converse);

        text = transform.GetComponentInChildren<Text>();

        dialog = new Queue<string>();
        dialog.Enqueue("Hello mortal!");
        dialog.Enqueue("I am Inky.");
        dialog.Enqueue("I give advice for free.");
        dialog.Enqueue("Listen carefully");
        dialog.Enqueue("Set your wifi password to 2444666668888888");
        dialog.Enqueue("So when someone ask tell them it's 12345678");
        dialog.Enqueue("Need more?");
        dialog.Enqueue("Sorry, I only give one per day.");
        dialog.Enqueue("Come back tomorrow.");
        dialog.Enqueue("I said COME BACK TOMORROW");

        text.text = dialog.Dequeue();
    }

    void Update()
    {
        
    }

    private void Converse()
    {
        text.text = dialog.Count != 0 ? dialog.Dequeue() : "Goodbye!";
    }

    public bool End { get { return dialog.Count == 0; } }
}
