using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Globalization;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.IO;
using System.Linq;


public class WSClient : MonoBehaviour
{
    WebSocket ws;
    public bool recalc = false;

    public float[] quats;
    public float[] accs;
    public float[] mags;


    public bool haveData = false;

    float stop, dt, start;
    [SerializeField] public float PosX, PosY;

    public Vector3 oneAxis;

    public bool stopGame = false;

    public bool Click = false;

    //stany gry
    public bool gameStart=false;
    public bool gameStartThird = false;
    public bool tutorial = false;
    public bool tutorialEkrany = false;


    //zmiana kamer
    [SerializeField]
    private CinemachineVirtualCamera camOboz;
    [SerializeField]
    private CinemachineVirtualCamera camGra;
    private bool obozCam = true;

    [SerializeField]
    private PlayTimeline play;

    [SerializeField]
    private KuszaController kusza;

    [SerializeField]
    private GameObject KomunikatWC;

    [SerializeField]
    private FadeScript fade;

  
    private Transform firstPositionOfKlusownik;

    [SerializeField]
    private Transform KlusonikContainer;
    [SerializeField]
    private Transform Klusonik;
    private bool SceneNietastacja = false;
    private bool Scene1 = false;
    private bool Scene2 = false;
    private bool Scene2b = false;
    private bool Scene3 = false;
    private bool SceneSpecial = false;
    public bool WC = false;
    private bool reset = false;
    public bool WaitForStopWC = false;
    [SerializeField]
    private PlayableDirector current;

    //audiofile
    [SerializeField] private TextAsset FileAudio;
    //FadeIn/Out audio
    public SetVolume setVolume;

    public GameObject nietastacja;
    public GameObject TBREAK;
    public bool TBreakOff = false;
    public bool TBreakOn = false;
    void Start()
    {
        TBREAK.SetActive(false);
        nietastacja.SetActive(false);
        WaitForStopWC = false;
        Debug.Log("start");
        Screen.SetResolution(1080, 1920, true);
        var _self = this;
        ReadFromFileAudio();
        ReadFromTimeoutInfo();
        firstPositionOfKlusownik = KlusonikContainer;
        fade.HideImage();
        ws = new WebSocket("ws://localhost:3000");
        ws.Connect();
      
       // ws.Send("GAME,CIR,0");
        ws.OnMessage += (sender, e) =>
        {
            
            haveData = true;
            string data = e.Data.ToString();
            //Debug.Log(data);
            if (data == "bp!")
            {
                Click = true;
                Debug.Log("Click");
            }
            if (data.Length > 0)
            {
                if (data == "GAME,SCENE,-1")
                {
                    SceneNietastacja = true; 
                }
                
                if (data == "GAME,SCENE,0")
                {
                  //  StartCoroutine(SendCirTimeout());
                    Scene1 = true;
                }
                else if (data == "GAME,SCENE,1")
                {
                   // StartCoroutine(SendCirTimeout());
                    Scene2 = true;
                }
                else if (data == "GAME,SCENE,2")
                {
                    //StartCoroutine(SendCirTimeout());
                    Scene2b = true;
                }
                else if (data == "GAME,SCENE,3") //wiecej beltow
                {
                   // StartCoroutine(SendCirTimeout());
                    Scene3 = true; 
                }
                else if (data == "GAME,SCENE,4")
                {
                    SceneSpecial = true;
                }
                else if (data == "GAME,TBREAK,1")
                {
                    TBreakOn = true;
                }
                else if (data == "GAME,TBREAK,1")
                {
                    TBreakOff = true;
                }


                if (data == "GAME,WC")
                {
                    WC = true;
     
                    stopGame = true;
                }
                else if (data == "GAME,CR")
                {
                    WC = false;
                    stopGame = false;

                }
                else if(data == "GAME,LC")
                {
                    reset = true;
                }
                if (data.Length > 8)
                {
                   // Click = false;
                    string[] splitData = data.Split(',');
                    float Accx = float.Parse(splitData[0], CultureInfo.InvariantCulture.NumberFormat);
                    float Accy = float.Parse(splitData[1], CultureInfo.InvariantCulture.NumberFormat);
                    float Accz = float.Parse(splitData[2], CultureInfo.InvariantCulture.NumberFormat);
                    float Gyrx = float.Parse(splitData[3], CultureInfo.InvariantCulture.NumberFormat);
                    float Gyry = float.Parse(splitData[4], CultureInfo.InvariantCulture.NumberFormat);
                    float Gyrz = float.Parse(splitData[5], CultureInfo.InvariantCulture.NumberFormat);
                    float Magx = float.Parse(splitData[6], CultureInfo.InvariantCulture.NumberFormat);
                    float Magy = float.Parse(splitData[7], CultureInfo.InvariantCulture.NumberFormat);
                    float Magz = float.Parse(splitData[8], CultureInfo.InvariantCulture.NumberFormat);
                    float Qw = float.Parse(splitData[9], CultureInfo.InvariantCulture.NumberFormat);
                    float Qx = float.Parse(splitData[10], CultureInfo.InvariantCulture.NumberFormat);
                    float Qy = float.Parse(splitData[11], CultureInfo.InvariantCulture.NumberFormat);
                    float Qz = float.Parse(splitData[12], CultureInfo.InvariantCulture.NumberFormat);


                    float[] quat = { Qw, Qx, Qy, Qz};
                    quats = quat;
                    float[] acc = { Accx, Accy, Accz };
                    accs = acc;
                    float[] mag = { Magz, Magy, Magz };
                    mags = mag;


                    PosX = float.Parse(splitData[13], CultureInfo.InvariantCulture.NumberFormat) / 25.0f;
                    PosY = float.Parse(splitData[14], CultureInfo.InvariantCulture.NumberFormat) / 25.0f;

                    if (!((PosX > -1 && PosX < 2) && (PosX > -1 && PosX < 2)))
                    {

                        ws.Send("GAME,FUCKEDUP");
                    }


                }
               

            }
            else
            {
               
                Debug.Log("recalc");
                if (data == "cwo") recalc = true;
            }

        };

        StartCoroutine(SendPing(60f));

    }

    private void Update()
    {
        if (Click)
        {
            StartCoroutine(ResetClick(0.5f));
        }
        if (SceneNietastacja)
        {
            PlayIrSound();
            SceneNietastacja = false;
            nietastacja.SetActive(true);
            StartCoroutine(Nietastacja(5f));
        }
        if (Scene1)
        {
            StartCoroutine(SendCirTimeout());
            PlayIrSound();
            setVolume.SetDown();
          
            PlayScene1();
            Scene1 = false;
        }
        if (Scene2)
        {
            StartCoroutine(SendCirTimeout());
            PlayIrSound();
            setVolume.SetDown();
            PlayScene2();
            Scene2 = false;
        }
        if (Scene2b)
        {
            StartCoroutine(SendCirTimeout());
            PlayIrSound();
            setVolume.SetDown();
            PlayScene2b();
            Scene2b = false;
        }
        if (Scene3)
        {
            StartCoroutine(SendCirTimeout());
            PlayIrSound();
            setVolume.SetDown();
            PlayScene4();
            Scene3 = false;
        }
        if (SceneSpecial)
        {
            PlayIrSound();
            play.PlaySceneDance();
            SceneSpecial = false;
        }
        if (WC && !WaitForStopWC)
        {
            KomunikatWC.SetActive(true);
            WaitForStopWC = true;
            current = play.currentPlay;
            current.Pause();
        }
        else if(!WC && WaitForStopWC)
        {
            KomunikatWC.SetActive(false);
            WaitForStopWC = false;
            current.Resume();
        }
        if (reset)
        {
           
            Reset();
            reset = false;
        }
        if (TBreakOn)
        {
            TBreakOn = false;
            TBREAKON();
        }
        if (TBreakOff)
        {
            TBreakOff = false;
            TBREAKOFF();
        }

        if (Input.GetKeyDown("t"))
        {
            TBreakOn = true;
        }
        if (Input.GetKeyDown("y"))
        {
            TBreakOff = true;
        }
        if (Input.GetKeyDown("1"))
        {
            PlayIrSound();
            setVolume.SetDown();
            PlayScene1();
        }
       
        if (Input.GetKeyDown("2"))
        {
            PlayIrSound();
            setVolume.SetDown();
            PlayScene2();
        }
        if (Input.GetKeyDown("b"))
        {
            SceneNietastacja = true;
        }
        if (Input.GetKeyDown("4"))
        {
            PlayIrSound();
            //play.currentPlay.Stop();
            play.PlaySceneDance();
        }
        if (Input.GetKeyDown("3"))
        {
            PlayIrSound();
            setVolume.SetDown();
            //play.currentPlay.Stop();
            PlayScene4();
        }
        if (Input.GetKeyDown("5"))
        {
            PlayIrSound();
            setVolume.SetDown();
            //play.currentPlay.Stop();
            PlayScene2b();
        }
        if (Input.GetKeyDown("5"))
        {
            PlayIrSound();
            setVolume.SetDown();
            //play.currentPlay.Stop();
            PlayScene2b();
        }
        if (Input.GetKeyDown("4"))
        {
            play.currentPlay.Stop();
            play.PlayScene4();
        }
            

        if (Input.GetKeyDown("r"))
        {
            Reset();
        }
        if (Input.GetKeyDown("q"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown("p"))
        {
            SwwitchCamera();
            play.PlayScene2();
        }
    }
    public void TBREAKON()
    {
        TBREAK.SetActive(true);
        //setVolume.SetUp();
        if (!obozCam)
        {
            camGra.Priority = 0;
            camOboz.Priority = 1;
            obozCam = !obozCam;
        }
       play.currentPlay.Stop();
        tutorialEkrany = false;
        gameStart = false;
        tutorial = false;
        play.PlaySceneIdle();
        kusza.ResetData();
        stopGame = false;
        fade.HideImage();
        Scene1 = false;
        Scene2 = false;
        KomunikatWC.SetActive(false);
        WaitForStopWC = false;
        WC = false;
    }

    public void TBREAKOFF()
    {
        TBREAK.SetActive(false);
        ws.Close();
        SceneManager.LoadScene(0);
    }
    public void PlayIrSound()
    {
        GetComponent<AudioSource>().Play();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
    public void FinishTutorial()
    {
        tutorial = false;
      
    }

    public void FinishGameLoss()
    {
        setVolume.SetUp();
        ws.Send($"GAME,FINISHED,0,{timeoutinfKoniecloss}");
        Debug.Log($"GAME,FINISHED,0,{timeoutinfKoniecloss}");
    }

    public void FinishGameWin()
    {
       // setVolume.SetUp();
        ws.Send($"GAME,FINISHED,1");
        Debug.Log($"GAME,FINISHED,1");
    }
    public void FinishGameWinSpecial()
    {
        
        ws.Send($"GAME,FINISHED,1,{timeoutinfKoniecd}");
        Debug.Log($"GAME,FINISHED,1,{timeoutinfKoniecd}");
    }

    public void SwwitchCamera()
    {
        Debug.Log("switch camera");
        if (obozCam)
        {
            camOboz.Priority = 0;
            camGra.Priority = 1;
        }
        else
        {
            camGra.Priority = 0;
            camOboz.Priority = 1;
            
        }

        obozCam = !obozCam;
    }

    public void EkranTutorial()
    {
        tutorialEkrany = true;
    }

    public void GameStart()
    {
        gameStart = true;
    }

    private void PlayScene1() //dzieciak przychodzi pierwszy raz
    {
        play.currentPlay.Stop();
        play.PlayScene1();
    }
    private void PlayScene2() //dzieciak przychodzi kolejny raz
    {
        play.currentPlay.Stop();
        play.PlayScene7();
    }
    private void PlayScene2b() //dzieciak przychodzi 3 kolejny raz
    {
        play.currentPlay.Stop();
        play.PlayScene9();
    }
    private void PlayScene4() //dzieciak przychodzi kolejny raz
    {
        play.currentPlay.Stop();
        play.PlayScene8();
    }
    private void PlaySceneDance() //dzieciak przychodzi kolejny raz
    {
        play.PlaySceneDance();
    }

    public void Reset()
    {
        
        setVolume.SetUp();
        if (!obozCam)
        {
            camGra.Priority = 0;
            camOboz.Priority = 1;
            obozCam = !obozCam;
        }
        //play.currentPlay.Stop();
        tutorialEkrany = false;
        gameStart = false;
        tutorial = false;
       // play.PlaySceneIdle();
        kusza.ResetData();
        stopGame = false;
        fade.HideImage();
        Scene1 = false;
        Scene2 = false;
        KomunikatWC.SetActive(false);

        //tylko dla Mateusza
       // ws.Send($"GAME,FINISHED,0");
       // Debug.Log($"GAME,FINISHED,0");
        WaitForStopWC = false;
        WC = false;
        ws.Close();
        SceneManager.LoadScene(0);
       



    }

    public void DeleteKolczan()
    {
        Debug.Log("GAME,EQREM,KOLCZAN");
      ws.Send("GAME,EQREM,KOLCZAN");
    }

    public void ReadFromFileAudio()
    {
        var splitFile = new string[] { "\r\n", "\n", "\r" };
        var Lines = FileAudio.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(Lines[0]);
        AudioListener.volume = float.Parse(Lines[0], CultureInfo.InvariantCulture.NumberFormat);
    }

    public void SendCIR()
    {
     Debug.Log("GAME,CIR,0");
     ws.Send("GAME,CIR,0");
    }
    public void SetGameStartThird()
    {
        gameStartThird = true;
    }

    private IEnumerator ResetClick(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Click = false;

    }
    private IEnumerator Nietastacja(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nietastacja.SetActive(false);
        ws.Send("GAME,FINISHED,0");

    }

    private IEnumerator SendPing(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            Debug.Log("GAME,PING");
            ws.Send("GAME,PING");
        }
    }
    private IEnumerator SendCirTimeout()
    {
        
            yield return new WaitForSeconds(1f);
            SendCIR();
       
    }



    public string timeoutinfKoniecd;
    public string timeoutinfKoniecloss;
    public void ReadFromTimeoutInfo()
    {
        string readFromFilePath = Application.streamingAssetsPath + "/TimeoutInfo" + ".txt";
        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();
        Debug.Log(fileLines[1]);
        timeoutinfKoniecd = fileLines[0];
        timeoutinfKoniecloss = fileLines[1];


    }



}
