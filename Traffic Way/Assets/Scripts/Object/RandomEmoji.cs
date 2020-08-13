using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomEmoji : MonoBehaviour
{
    private static List<Texture2D> Emoji;

    public static void LoadResources()
    {
        Emoji = new List<Texture2D>();
        Emoji.Add(Resources.Load<Texture2D>("TrafficWay/Textures/Interface/amazing_1"));
        Emoji.Add(Resources.Load<Texture2D>("TrafficWay/Textures/Interface/amazing_2"));
        Emoji.Add(Resources.Load<Texture2D>("TrafficWay/Textures/Interface/amazing_3"));
    }

    void Start()
    {
        GetComponent<RawImage>().texture = Emoji[Random.Range(0, Emoji.Count)];
    }
}
