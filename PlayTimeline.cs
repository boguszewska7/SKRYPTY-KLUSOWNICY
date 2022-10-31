using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayTimeline : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector director1;
    [SerializeField]
    private PlayableDirector director2;
    [SerializeField] 
    private PlayableDirector director2b;
    [SerializeField]
    private PlayableDirector director3;
    [SerializeField]
    private PlayableDirector director3b;
    [SerializeField]
    private PlayableDirector director4;
    [SerializeField]
    private PlayableDirector director5;
    [SerializeField]
    private PlayableDirector director6;
    [SerializeField]
    private PlayableDirector director6b;
    [SerializeField]
    private PlayableDirector director7;
    [SerializeField]
    private PlayableDirector director8;
    [SerializeField]
    private PlayableDirector director9;
    [SerializeField]
    private PlayableDirector director10;
    [SerializeField]
    private PlayableDirector dance;

    public PlayableDirector currentPlay;
    // Start is called before the first frame update
    void Start()
    {
        currentPlay = director8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayScene1() //zbierz belty
    {
        director1.Play();
        currentPlay = director1;
    }

    public void PlayScene2() //pojdz do klusownikow
    {
        director2.Play();
        currentPlay = director2;
    }
    public void PlayScene2b() //pojdz do klusownikow
    {
        director2b.Play();
        currentPlay = director2b;
    }
    public void PlayScene3() //udani klusownicy
    {
        director3.Play();
        currentPlay = director3;
    }
    public void PlayScene3b() //udani klusownicy
    {
        director3b.Play();
        currentPlay = director3b;
    }
    public void PlayScene4() //dodaj klucz
    {
        director4.Play();
        currentPlay = director4;
    }
    public void PlayScene5() //zly lub brak
    {
        director5.Play();
        currentPlay = director5;
    }
    public void PlayScene6() //wskazowka belty
    {
        director6.Play();
        currentPlay = director6;
    }
    public void PlayScene7() //wskazowka belty
    {
        director7.Play();
        currentPlay = director7;
    }
    public void PlayScene8() //wskazowka belty
    {
        director9.Play();
        currentPlay = director9;
    }

    public void PlayScene9() //przychodzi 3 raz
    {
        director10.Play();
        currentPlay = director10;
    }
    public void PlaySceneDance() //wskazowka belty
    {
        dance.Play();
        currentPlay = dance;
    }

    public void PlaySceneIdle() //wskazowka belty
    {
       
        director8.Play();
        director6b.Play();
        currentPlay = director8;
    }
}
