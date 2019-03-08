using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private GameObject CH_FindPlayer;
    public static int CH_BulletCount;

    // Start is called before the first frame update
    void Start()
    {
        if(CH_FindPlayer = GameObject.Find("Player"))
        {
            gameObject.transform.position = CH_FindPlayer.transform.position;
        }
        CH_BulletCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CH_BulletCount);

        gameObject.transform.position = CH_FindPlayer.transform.position;

        if(CH_BulletCount<=-6)
        {
            Destroy(gameObject);
        }
    }
}
