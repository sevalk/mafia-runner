using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pools : MonoBehaviour
{
    public enum Types
    {
        Like = 5,
        MafiaMemberBlue = 10,
        Money = 15,
        Bullet = 20,
    }

    public static string GetTypeStr(Types poolType)
    {
        return Enum.GetName(typeof(Types), poolType);
    }
}
