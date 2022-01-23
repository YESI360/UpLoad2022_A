using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSB3 : MonoBehaviour
{
    Renderer rend;
    public float manosLim;//0.147
    public float mirrorLimt;//0.223
    public float luzLimt;//0.301

    public float guiaStandup;//0.401
    public float guiaAvatar;//0.555
    public float bellyLimt;//0.633
    public PointerEvents gaze;

    public LuzAmb luzAmb; 
    public LuzGuia luzguia;
    public tocar3 espejo;
    public ChangeMat avatar;
    public PDSensorSHARED pd;
    public colliderManos touch;
    public activarColliderGuia colliderOnG;
    public activarColliderManoR colliderOnR;
    public activarColliderManoL colliderOnL;

    public int steps = 10;
    public float targetValueDownIncreaseStep;
    public float targetValueUp;
    public float targetValueDown;

    public float MaxContribution;
    public float MinContributionNew;//start in black
    public int MinContributionCount;
    public float VelContribution;
    public float currentValue;
    [Range(0.210f, 1)] public float lerpedValue;

    public float lerpSpeedUp;
    public float lerpSpeedDown;
    private float lerpSpeedUpNew;
    private float lerpSpeedUpCount;
    public float lerpSpeedUpVel;

    public AudioSource instrIni;
    public float delay = 2;
    public float volume = 0.5f;

    public bool instruccionIni;
    public bool InStandup;
    public bool InMirror;
    public bool alCollider;
    public bool InHands;
    public bool InLuzamb;
    public bool InGuia;

    public FlowCal flowcal;

    public MyMessageListenerSHARED contador;


    void Start()
    {      
        instrIni.PlayDelayed(delay);

        InStandup = true;
        InMirror = true;

        InHands = true;
        InLuzamb = true;
        InGuia = true;
    
        targetValueDownIncreaseStep = (targetValueUp - targetValueDown) / steps;
        rend = GetComponent<Renderer>();
        currentValue = rend.material.GetFloat("_ZeroValue");

        MaxContribution = 1;
        targetValueUp = 1;
        targetValueDown = 0;
        MinContributionNew = 0.181f;//0.210f; //=1 resp
        lerpSpeedUpCount = 0;
        MinContributionCount = 0;

    }

    void Update()
    {
        if (Input.GetKeyDown("1")) // (Input.GetKeyDown("1"))//
        {
            MinContributionNew = 0.454f;
            Debug.Log("ver");
        }

        if (Time.timeSinceLevelLoad <= delay || instrIni.isPlaying)//?
            return;
        if (flowcal.CurrentState1 == GameState1.NotStarted1)
            flowcal.SetState(GameState1.Chest1);
        
    }

    public void LuzUp()
    {
        lerpedValue = Mathf.Lerp(currentValue, targetValueUp, Time.deltaTime * lerpSpeedUpNew);
        currentValue = Mathf.Clamp(lerpedValue, MinContributionNew, MaxContribution);
        rend.material.SetFloat("_ZeroValue", currentValue);

        lerpSpeedUpCount = lerpSpeedUpCount + 1; // So you add to the count everytime you go into this function, if it executes once when you have a new breath
        lerpSpeedUpNew = lerpSpeedUpCount * lerpSpeedUpVel; // So this increases by 0.2f everytime this function is called

        MinContributionCount = MinContributionCount + 1;
        MinContributionNew = MinContributionCount * VelContribution;

            if (MinContributionNew >= manosLim && InHands && !SoundManagerGuia.instance.IsPlaying)
            {//////////////////////0.147                   
                 Debug.Log("manosAIR"); //Se prende luz manos x input porque en esta en MyMessageListenerSHARED
            SoundManagerGuia.instance.PlayInstruccion01();//where AIR?
            InHands = false;
            }


            if (MinContributionNew > mirrorLimt && InMirror && !SoundManagerGuia.instance.IsPlaying)
            {//////////////////////////0.223
                espejo.planeMirror.SetActive(true);
                Debug.Log("espejoReal");
            SoundManagerGuia.instance.PlayInstruccion02();//Air enter... air out
            InMirror = false;
            }

            if (MinContributionNew >= luzLimt && InGuia && !SoundManagerGuia.instance.IsPlaying)
            {///////////////////////////////0.301
                    luzguia.luzUpGuia();
                    luzAmb.luzUp();
                    Debug.Log("luzAmb1 + luzGuia");              
            SoundManagerGuia.instance.PlayInstruccion03();//Its me! To guide you ||-NO TOCO, SOLO SALUDO!!-
            InGuia = false;                        
                       colliderOnG.agrandarCollider();////agrando collider GUIA
                       alCollider = true;///////aviso al collider que se puede activar recien aca
            }
        //no mas input CHEST!(va a seguir afectando el shader pero sin trigeriar nada)
        //usuario mira GUIA Y play INSTRUCCION04 take my hand>>>>PointerEvents

        if (gaze == true )//&& InStandup )//&& !SoundManagerGuia.instance.IsPlaying)
        {
            luzAmb.luzUp();
            Debug.Log("standUp");/// por que se pone en false con el 1er input??
            InStandup = false;         
                    colliderOnL.agrandarCollidermanoL();////agrando collider MANOS para que toqen guia+mano
                    colliderOnR.agrandarCollidermanoR();// y luego manos+capsula
        }

        ////////YO TOCO GUIA CON MANO, COLISIONO VOY AL CODIGO >>>colliderManos
        //// Y PlayInstruccion05();//put hands waits [IF alCollider = true]


        var potentialNextValue = Mathf.Clamp(targetValueDown + targetValueDownIncreaseStep, 0, targetValueUp);
        if (currentValue >= potentialNextValue)
        {
            targetValueDown = potentialNextValue;
        }

    }


    public void LuzDown()
    {

        lerpedValue = Mathf.Lerp(currentValue, targetValueDown, Time.deltaTime * lerpSpeedDown);

        rend.material.SetFloat("_ZeroValue", Mathf.Clamp(lerpedValue, MinContributionNew, MaxContribution));

        currentValue = rend.material.GetFloat("_ZeroValue");
    }

}
