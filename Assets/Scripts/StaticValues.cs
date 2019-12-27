using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public static class StaticValues
{
    public static Color[] GetColors()
    {
        Color[] colors = new Color[4];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.yellow;
        colors[3] = Color.blue;
        return colors;
    }
    public static bool AreVectorsEqual(Vector2 val1, Vector2 val2)
    {
        bool ret = false;
        bool[] tmp = new bool[2];
        tmp[0] = Mathf.Approximately(val1.x, val2.x);
        tmp[1] = Mathf.Approximately(val1.y, val2.y);
        int check = 0;
        for (int i = 0; i < 2; i++)
        {
            if (tmp[i] == true)
                check++;
        }
        if (check == 2)
            ret = true;
        return ret;
    }
    public static void SetupNetwork()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
}
