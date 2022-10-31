using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddPoints : MonoBehaviour
{
    public GameObject kusza;
    //public PointsController PointsController;

    public int PointsToAdd;
    public Text PointsText;
    public void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().Play();
        kusza.GetComponent<KuszaController>().Points += PointsToAdd;
        PointsText.text = kusza.GetComponent<KuszaController>().Points.ToString();
        Debug.Log("tarcza");
    }
}
