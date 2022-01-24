using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movDER : MonoBehaviour
{
    public GameObject CubeVocesDER;
    public float movementSpeed;
    public float SpeedVol;
    public AudioSource Voces;

    private Vector3 target = new Vector3(-151, 139, -163);
    void Start()
    {
        GetComponent<AudioSource>().volume = 1;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    }

    public void VolumenDown()
    {
        Voces.volume -= Time.deltaTime * SpeedVol;
        //Voces.volume = 0.6f;
    }



}
