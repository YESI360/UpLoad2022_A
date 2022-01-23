using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using UnityEngine.Events;
using Valve.VR;

public class MyMessageListenerSHARED : MonoBehaviour
{
    public GameObject sphereBelly;
    public GameObject sphereChest;
    public PDSensorSHARED pdScript;//chest de la escenaCITY
    //de la escenaCALIBRACION
    public LuzBelly luzB;
    public ShaderSB3 luzSB;
    public LuzHandL luzmanoL;
    public LuzHandR luzmanoR;
    //hice esto para controlar el scale de sphere con input de sensores
    public Vector3 maxLocalScale;
    float maxlocalScaleMagnitude;

    public Text belly;
    public Text chest;
    //datos de sensores
    public float datoNormCC;
    public float datoNormCB;
    public int datoL = 0;
    private int datoLant = 1;
    public int datoL2 = 0;
    string[] vec1;

    public FlowCal flowCal;
    public FlowManCITY flowCity;

    public int stepB;
    public int stepsAntB = 0;
    public int stepC;
    public int stepsAntC = 0;


    private void Start()
    {
        maxLocalScale = new Vector3(3, 3, 3);
        maxlocalScaleMagnitude = maxLocalScale.magnitude;
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnLevelWasLoaded(int level)
    {
        Initialize();
    }

    void Update() //botonera INPUT SENSOR //Arrows NO FUNCIONA!!
    {       
        if (Input.GetKey(KeyCode.UpArrow))//CHEST2 INHALA///up/////
        {
            datoL2 = 2;
        }
        if (Input.GetKey(KeyCode.DownArrow))//CHEST1 EXHALA
        {
            datoL2 = 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))//BELLY INHALA///right///////
        {
            datoL = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))//BELLY EXHALA
        {
            datoL = 1;
        }        
    }


    public void OnMessageArrived(string msg)
    {
        //Debug.Log("Arrived: " + msg);
        vec1 = msg.Split(',');
        string c1 = "chest";
        string c2 = (vec1[0]);
        string b1 = "belly";
        string n1 = "CC";
        string n2 = "CB";

        if ((String.Compare(c1, c2)) == 0)//chest to vec
        {
            datoL2 = (Convert.ToInt32(vec1[1]));//CHEST
            //Debug.Log("chest:" + datoL2);
        }
        else if ((String.Compare(b1, c2)) == 0)//belly to vec
        {
            datoL = (Convert.ToInt32(vec1[1]));//belly  
            //Debug.Log("belly:" + datoL);
        }
        else if ((String.Compare(n1, c2)) == 0)//norm to vec
        {
            datoNormCC = float.Parse(vec1[1]);
            //Debug.Log("calibracion:" + datoNormCC);
        }
        else if ((String.Compare(n2, c2)) == 0)//norm to vec
        {
            datoNormCB = float.Parse(vec1[1]);
            //Debug.Log("calibracion:" + datoNormCB);
        }
        ///////////////////////////////////////////////txt 
        belly.text = "belly: " + datoL;
        chest.text = "chest: " + datoL2;
        ///////comentar al usar solo! esc.CALIBRACION/////////descomentar p/ cambiar scene///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //if (datoL2 == 2 && flowCity.CurrentState0 == GameState0.Chest0)
        //{
        //    pdScript.SoundUp();
        //    //Debug.Log("pdCITY");
        //}
        //else { pdScript.SoundDown(); }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (flowCal != null) //////componentes compartidos entre escenas
        {
            ///////////////////////////////////////PECHO////////////CHEST//////////////L2//////////////////////////////////CHEST
            if (datoL2 == 2 && flowCal.CurrentState1 == GameState1.Chest1 )//|| flowC.CurrentState0 == GameState0.Chest0) 
            {
                pdScript.SoundUp();

                //controlar el scale de sphere con input de sensores
                float actualLocalScaleMagnitude = transform.localScale.magnitude;
                if (actualLocalScaleMagnitude < maxlocalScaleMagnitude)
                {
                    sphereChest.transform.localScale += new Vector3(3, 3, 3) * 2 * (Time.deltaTime);
                }

                //////componentes compartidos entre escenas
                //luzSB.LuzUp();
                if (luzSB != null)
                {
                    luzSB.LuzUp();
                }

/*
                 if (luzSB != null )
                 {
                       luzSB.LuzUp();/////////////////////ShaderSB3
 
                    stepC++;
                    Debug.Log("CONTADOR : " + stepC);

                    if (stepC >= 50 )//&& stepC < 65)
                    {                       
                        luzSB = null;//dejar de leer input PERO VOLVER A LEER NORMAL...
                        flag = true;
                    }

                    if (flag == true )// && datoL2 == 1)
                    {
                        luzSB.LuzUp();
                        //_ = luzSB != null;
                    }

                 }
*/
                //luzmanoL.luzHandL();
                if (luzmanoL != null)
                {
                    luzmanoL.luzHandL();
                }
                //luzmanoR.luzHandR();
                if (luzmanoR != null)
                {
                    luzmanoR.luzHandR();
                }
            }
            else
            {
                pdScript.SoundDown();
                sphereChest.transform.localScale = new Vector3(3, 3, 3) * 2 * (Time.deltaTime);

                //luzSB.LuzDown();
                if (luzSB != null)
                {
                    luzSB.LuzDown();
                }
            }

            ////////////////////PANZA////////////BELLY//////////////////////////L//////////////////////BELLY  
            if (datoL != datoLant && flowCal.CurrentState1 == GameState1.Belly1 && !SoundManagerGuia.instance.IsPlaying)//(datoL == 2 && flow.CurrentState1 == GameState1.Belly1)
            {

                stepB++;
                Debug.Log("STEPS : " + stepB);
                    
                    if (stepB == 2)
                    {
                        SoundManagerGuia.instance.PlayInstruccion07();
                        return;
                    }
                    datoLant = datoL;
                    stepsAntB = stepB;

                    if (stepB == 4)
                    {
                        SoundManagerGuia.instance.PlayInstruccion08();
                    SteamVR_LoadLevel.Begin("03_ForestNEW");
                    return;
                    }
                    datoLant = datoL;
                    stepsAntB = stepB;
            }

            //////componentes compartidos entre escenas
            if (datoL == 2 && flowCal.CurrentState1 == GameState1.Belly1)
            {
                if (luzB != null)
                {
                    luzB.luzUpBELLY();
                }
            }
            else
            {
                if (luzB != null)
                {
                    luzB.luzDownB();
                }
            }
        }
    }

    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }

    private void Initialize() //////componentes compartidos entre escenas
    {
        luzB = FindObjectOfType<LuzBelly>();//luz belly
        luzSB = FindObjectOfType<ShaderSB3>();//ShaderSB3
        luzmanoL = FindObjectOfType<LuzHandL>();//luz mano
        luzmanoR = FindObjectOfType<LuzHandR>();//luz mano
        flowCal = FindObjectOfType<FlowCal>();
    }
}
