using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton instance;

    public List<Color> playerColors;
    private int randomColor;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }

        instance = this;
    }

    public Color GetRandomColor()
    {
        randomColor = Random.Range(0, playerColors.Count);
        return playerColors[randomColor];
    }

    public void RemoveColorValue()
    {
        playerColors.RemoveAt(randomColor);
    }
}
