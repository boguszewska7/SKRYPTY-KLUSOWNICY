using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltyController : MonoBehaviour
{
    public KuszaController Kusza;
    public WSClient wSClient;
    public int BoltNow = 0;
    private int HowMuchBolt;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Kusza.DelateBelt)
        {
           
           transform.GetChild(BoltNow).gameObject.SetActive(false);
            Kusza.DelateBelt = false; 
            BoltNow++;
        }
    }

    public void ActivAllBelts()
    {
        BoltNow =0;
        foreach(Transform g in transform)
        {
            g.gameObject.SetActive(true);
        }
    }
}
