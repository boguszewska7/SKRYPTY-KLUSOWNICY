using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private CanvasGroup c;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            if (c.alpha < 1)
            {
                c.alpha += Time.deltaTime;
                if (c.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }

        if (fadeOut)
        {
            if (c.alpha >=0)
            {
                c.alpha -= Time.deltaTime;
                if (c.alpha == 0)
                {
                    fadeOut = false;
                }
            }
        }
    }
    public void ShowImage()
    {
        fadeIn = true;
    }
    public void HideImage()
    {
        fadeOut = true;
        Debug.Log("HideImage");
    }
}
