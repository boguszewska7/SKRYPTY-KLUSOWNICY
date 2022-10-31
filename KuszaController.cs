using System.Collections;
using WebSocketSharp;
//using System.Collections.Generic;
using System;
using UnityEngine;
using System.Globalization;
using System.Text;
using UnityEngine.UI;
public class KuszaController : MonoBehaviour
{
    [SerializeField]
    private PlayTimeline play;


    public GameObject WSclient;

    //rotacja
    Vector3 controllerRotation;
    public Vector3 firstRotation;
    Vector3 waitRotation;
    float firstAccx;
    public const float angleMultiplier = 1.0f;
    private bool first_data = true;
    float prev = 0;
    float prevx = 0;
    float prevz = 0;
    public Vector3 oneAxis;

    //sila potrzebna do zaladowania beltu
    [SerializeField] public float powerToAcc = 3f;

    [SerializeField]  private AudioSource audio;
    private Animator kuszaAnimator;


    //zmienne do tworzenia strzal
    [SerializeField] public bool CreateArow = false;
    [SerializeField] public bool PushArow = false;
    [SerializeField] public bool ActiveArrow = false;


    //Projekcja 
    public Transform Point;

    public double Power;
    [SerializeField] private float MaxPower = 20;
    [SerializeField] public float MaxRoatation = 30;
    [SerializeField] public double RotationControllerX;
    [SerializeField] public float minPower = 20;

    //strzelanie
    public GameObject Belt;
    public Renderer BeltFake;
    private Animator BeltAnimator;
    private LineRenderer lineRenderer;
    //usuniecie strzaly(BeltyController)
    [SerializeField] public bool DelateBelt = false;


    private WSClient ws;

    public GameObject CanvasPoints;
    public GameObject EkranTutoria0;
    public GameObject EkranTutoria1;
    public GameObject EkranTutoria2;
    public GameObject EkranTutoria3;

    // punkty

    public int Points = 0;
    public Text PointsText,
    PointsKlusownik;
    public int punktyKlusownika = 5;


    //ilosc wytrzelonych boltow

    public int BoltCount = 0;
    private int BoltCountTutorial = 0;
    public GameObject BoldParent;


    private IEnumerator coroutine;

    private bool firstInTutorial = false;

    //zmienne do tutorialu
    public bool canRotate = false;
    public bool canvasCanDisapear = false;


    public bool canCreateArrow = false;



    //kolczan
    [SerializeField]
    private BeltyController Kolczan;
    bool firstBolt = true;
    //po ulozeniu dobrze kontrolera
    public bool goodPositionOfControler = false;
    private void Start()
    {
        EkranTutoria1.SetActive(false);
        EkranTutoria2.SetActive(false);
        EkranTutoria3.SetActive(false);
        lineRenderer = GetComponent<LineRenderer>();
        kuszaAnimator = GetComponent<Animator>();
        ws = WSclient.GetComponent<WSClient>();
        BeltFake.enabled = false;
        BeltAnimator = Belt.GetComponent<Animator>();
    }


    public bool continueGame = false;
    void Update()
    {
       // Rotation();
        if (ws.tutorialEkrany)
        {
           
            if (!firstInTutorial)
            {
                //ws.SendCIR();
                StartCoroutine(WaitAndFirstDataTrue(1f));
                Kolczan.ActivAllBelts();
                lineRenderer.enabled = true; 
                firstInTutorial = true; 
            }
            if (!ws.stopGame && canRotate && !ws.WC)
            {
                Rotation();      
                CalculatePower();
            }
            if (goodPositionOfControler)
            {
                EkranTutoria1.SetActive(true);
                play.PlayScene2b();
                goodPositionOfControler = false;
            }
            if (ws.stopGame && !ws.WC)
            {
                first_data = true;
            }

            if (CreateArow && !ActiveArrow && canvasCanDisapear) //ekran znika
            {
                CreateArow = false;
                Arrow2(); 
                EkranTutoria1.SetActive(false);
                EkranTutoria2.SetActive(true);
                canvasCanDisapear = false;
                play.PlayScene3();
            }
            if (PushArow && ws.Click && canvasCanDisapear) //drugi ekran znika
            {
                PushArow = false;
                Strzelanie2();
                continueGame = true;
                ws.tutorialEkrany = false;
                ws.tutorial = true;
                EkranTutoria2.SetActive(false);
                firstInTutorial = false;
                canvasCanDisapear = false;
                EkranTutoria1.SetActive(true);

            }

            
        }
       else if (ws.tutorial)
        {
            
            if (!firstInTutorial && !continueGame)
            {
                //ws.SendCIR();
                StartCoroutine(WaitAndFirstDataTrue(5f));
                Kolczan.ActivAllBelts();
                lineRenderer.enabled = true; 
                firstInTutorial = true;
            }
            if (continueGame)
            {
               // Kolczan.ActivAllBelts();
                lineRenderer.enabled = true;
                firstInTutorial = true;
                continueGame = false;
            }
            if (!ws.stopGame && !ws.WC)
            {
                Rotation();
                CalculatePower();

            }
    
            if (ws.stopGame && !ws.WC)
            {
                first_data = true;
            }


            if (CreateArow && !ActiveArrow)
            {
                CreateArow = false;
                Arrow2();
                EkranTutoria1.SetActive(false);
                EkranTutoria2.SetActive(true);
            }

            if (PushArow && ws.Click)
            {
                PushArow = false; 
                Strzelanie2();
                EkranTutoria1.SetActive(true);
                EkranTutoria2.SetActive(false);
               
            }

         
           
            if (BoltCountTutorial == 5)
            {
                canCreateArrow = false;
                CreateArow = false;
                canRotate = false;
                goodPositionOfControler = false;
                EkranTutoria1.SetActive(false);
                ws.FinishTutorial();
                lineRenderer.enabled = false;
                coroutine = AfterRestartData(2f);
                StartCoroutine(coroutine);
                firstInTutorial = false;
               // EkranTutoria3.SetActive(true);
            }
        }
        //Debug.Log(Accx +";"+ Accy + ";"+ Accz);
        else if (ws.gameStart)
        {  
            if (!firstInTutorial)
            {
                StartCoroutine(WaitAndFirstDataTrue(3f)); //dopasuj do lektora
                // ws.DeleteKolczan();
                EkranTutoria3.SetActive(true);
                CanvasPoints.SetActive(true);
                if(!ws.gameStartThird)
                    lineRenderer.enabled = false;
                else
                {
                    lineRenderer.enabled = true;
                    ws.gameStartThird = false;
                }
                   
                Kolczan.ActivAllBelts();
                firstInTutorial = true;
                PointsKlusownik.text = punktyKlusownika.ToString();
            }

            if (!ws.stopGame && canRotate && !ws.WC)
            {
                Rotation();
                CalculatePower();
               
            }
            if (ws.stopGame && !ws.WC)
            {
                first_data = true;
            }

           

            if (CreateArow && !ActiveArrow && BoltCount < 5)
            {
                CreateArow = false;
                Arrow2();
            }

            if (PushArow && ws.Click)
            {
                PushArow = false;
                Strzelanie2();

                Debug.Log(BoltCount);
            }

          
            if (BoltCount == 5)
            {
                CreateArow = false;
                canRotate = false;
                BoltCount++;
                coroutine = WaitAndSwitch(1.5f);
                StartCoroutine(coroutine);
            }
        }
      

    }

    Vector3 calculateRotation(float[] quat)
    {
        Vector3 output;
        // roll (x-axis rotation)
        double sinr_cosp = 2 * (quat[0] * quat[1] + quat[2] * quat[3]);
        double cosr_cosp = 1 - 2 * (quat[1] * quat[1] + quat[2] * quat[2]);
        output.z = (float)Math.Atan2(sinr_cosp, cosr_cosp);
        // pitch (y-axis rotation)
        float sinp = 2 * (quat[0] * quat[2] - quat[3] * quat[1]);
        if (Math.Abs(sinp) >= 1)
            output.x = MathF.Sign((float)sinp) * (float)Math.PI / 2.0f;
        else
            output.x = (float)Math.Asin(sinp);
        // yaw (z-axis rotation)
        double siny_cosp = 2 * (quat[0] * quat[3] + quat[1] * quat[2]);
        double cosy_cosp = 1 - 2 * (quat[2] * quat[2] + quat[3] * quat[3]);
        output.y = (float)Math.Atan2(siny_cosp, cosy_cosp);
        output.x = output.x * 180 / (float)Math.PI;
        output.y = -output.y * 180 / (float)Math.PI;
        output.z = -output.z * 180 / (float)Math.PI;
        return output;
    }

    void Rotation()
    {
        if (first_data)
        {
            firstRotation = calculateRotation(ws.quats);
            firstAccx = ws.accs[0];
            prev = firstRotation.y;
            // firstRotation.y += 90;
            if (ws.Click)
            {
              
                first_data = false;
                EkranTutoria0.SetActive(false);
                EkranTutoria3.SetActive(false);
                goodPositionOfControler = true;
            }
        }
        else if (ws.recalc)
        {
            float n = calculateRotation(ws.quats).y;
            firstRotation.y = n - prev;
            firstRotation.x = n - prevx;
            firstRotation.z = n - prevz;
            ws.recalc = false;
            firstAccx = ws.accs[0];
        }
        else
        {

            controllerRotation = calculateRotation(ws.quats);
            controllerRotation.x -= firstRotation.x;
            controllerRotation.y -= firstRotation.y;
            controllerRotation.z -= firstRotation.z;


            prev = controllerRotation.y;
            prevx = controllerRotation.x;
            prevz = controllerRotation.z;
            oneAxis = new Vector3(-controllerRotation.x, controllerRotation.y, -controllerRotation.z);

            controllerRotation.x = Mathf.Clamp(controllerRotation.x, -40, 7);
            controllerRotation.z = Mathf.Clamp(controllerRotation.z, -75, 75);
            controllerRotation.y = Mathf.Clamp(controllerRotation.y, -60, 60);

            //CreateArrow  zmienna - kiedy true utworz strzale?
            //PushArow zmienna - kiedy true mozna wystrzelic?
            //!ActiveArrow - kiedy true to znaczy ze jest aktywny belt
            //canCreateArrow - zmienna czekajaca na koniec dialogow zanim bedzie mozna wystzrlic

            if (firstAccx - powerToAcc > ws.accs[0] && ws.accs[1] < 6 && CreateArow == false && PushArow == false && !ActiveArrow && canCreateArrow)
            {
                Debug.Log("strzala");
                CreateArow = true;
               // PushArow = true;
                //ActiveArrow = true;
            }
            RotationControllerX = -controllerRotation.x;

            transform.eulerAngles = oneAxis;
        }

    }

    void Rotation2()
    {
        transform.rotation =new Quaternion(-ws.quats[3], ws.quats[0],-ws.quats[2],- ws.quats[1]);
    }

    void Arrow()
    {
        DelateBelt = true;
        Debug.Log("Stworz strzale");    
        kuszaAnimator.Play("Naciaganie");
        BeltFake.enabled = true;
        Belt.transform.GetComponent<Rigidbody>().isKinematic = true;
        CreateArow = false;
    }

    void Arrow2()
    {
        if (BoltCount < 5)
        {
        audio.Play();
        DelateBelt = true;
        Debug.Log("Stworz strzale");
        kuszaAnimator.Play("Naciaganie");
        BeltFake.enabled = true;
        Belt.transform.GetComponent<Rigidbody>().isKinematic = true;
        CreateArow = false;
        PushArow = true;
        ActiveArrow = true;
        }
       
    }



    void Strzelanie()
    {
        Belt.SetActive(false);
        Debug.Log("Stzrelanie!");
        GameObject CreatedArrow = Instantiate(Belt, Point.position, Point.rotation);
        CreatedArrow.SetActive(true);
        kuszaAnimator.Play("Strzelanie");
        ActiveArrow = false;
        PushArow = false;
    }

    void Strzelanie2()
    {
        
        Debug.Log("Strzelanie!");
        GameObject CreateArrow = Instantiate(Belt, Point.position, Point.rotation);
        BeltFake.enabled = false;
        CreateArrow.SetActive(true);
        CreateArrow.transform.localScale = new Vector3(2, 2, 2);
        CreateArrow.transform.GetComponent<Rigidbody>().isKinematic = false;
        CreateArrow.GetComponent<Rigidbody>().velocity = Point.forward * (float)Power;
        CreateArrow.transform.SetParent(BoldParent.transform);
        kuszaAnimator.Play("Strzelanie");
        StartCoroutine(ResetCanCreateArrrow(1f));
        PushArow = false;
        //Belt.SetActive(false);
        if (ws.gameStart)
            BoltCount++;
        else if (ws.tutorial || ws.tutorialEkrany)
            BoltCountTutorial++;

    }

    void CalculatePower()
    {
        Power = minPower + (MaxPower * (RotationControllerX / MaxRoatation));
    }

    public void ResetData()
    {
        Debug.Log("reset data kusza");
        if (canCreateArrow)
        {
            Strzelanie2();
        }
        ActiveArrow = false;
        PushArow = false;
        BeltFake.enabled = false;
        Debug.Log("resetData");
        transform.rotation = Quaternion.identity;
        goodPositionOfControler = false;
        Kolczan.ActivAllBelts();
        ws.stopGame = false;
        ws.tutorial = false;
        ws.tutorial = false;
        EkranTutoria1.SetActive(false); 
        EkranTutoria0.SetActive(false);
        EkranTutoria2.SetActive(false);
        EkranTutoria3.SetActive(false);
        CanvasPoints.SetActive(false);
        firstInTutorial = false;
        BoltCount = 0;
        BoltCountTutorial = 0;
        Points = 0;
        canRotate = false;
        firstBolt = true;
        canCreateArrow = false;
        foreach (Transform child in BoldParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void ResetDataToGame()
    {
        //canRotate = true;
        Debug.Log("resetData");
        transform.rotation = Quaternion.identity;
        goodPositionOfControler = false;
        Kolczan.ActivAllBelts();
        BoltCount = 0;
        BoltCountTutorial = 0;
        Points = 0;
        foreach (Transform child in BoldParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    private IEnumerator AfterRestartData(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        ResetDataToGame();
        //ws.gameStart = true;
        play.PlayScene3b();
        PointsText.text = "0";
        PointsKlusownik.text = punktyKlusownika.ToString();
    }

    private IEnumerator WaitAndReset(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        ResetData();
    }

    private IEnumerator WaitAndFirstDataTrue(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        first_data = true;
        canCreateArrow = true;
        canRotate = true;
    }

    private IEnumerator WaitAndSwitch(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        FinishGame();
    }

    //in tutorial
    public void CanRotate()
    {
        
         canRotate = true;
    }
    public void CanCreateArrow()
    {
        canCreateArrow = true;
    }

    public void CanvasCanDisapear()
    {
        canvasCanDisapear = true;
    }

    public void FinishGame()
    {
        CanvasPoints.SetActive(false);
        ws.gameStart = false;
        if (Points < punktyKlusownika)
        {
          
            play.PlayScene5();
        }

        else
        {
            play.PlayScene4();
        }
       
        ws.SwwitchCamera();
        coroutine = WaitAndReset(2f);
        StartCoroutine(coroutine);
    }
    public void ShowEkran0()
    {
        EkranTutoria0.SetActive(true);
    }
    public void StartGame()
    {
        ws.gameStart = true;
     
    }
    private IEnumerator ResetCanCreateArrrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ActiveArrow = false;

    }
}
