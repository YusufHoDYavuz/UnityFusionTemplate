using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessagesUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUIs;

    private Queue messageQueue = new Queue();
    
    void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        Debug.Log($"InGameMessageUIHandler {message}");
        
        messageQueue.Enqueue(message);

        if (messageQueue.Count > 3)
            messageQueue.Dequeue();

        int queueIndex = 0;
        foreach (string messageInQueue in messageQueue)
        {
            textMeshProUGUIs[queueIndex].text = messageInQueue;
            queueIndex++;
        }

    }
}
