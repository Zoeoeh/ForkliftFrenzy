﻿using UnityEngine;
using System.Collections;

public class GameActor : MonoBehaviour
{
    public float speed = 2;

    private GameObject fork;

    public float turnSpeed = 2.0f;

    private Vector3 offset;

    // Initialise fork object
    void Start()
    {
        fork = GameObject.FindGameObjectWithTag("Fork");

        // camera offset 
        offset = new Vector3(transform.position.x, transform.position.y + 8.0f, transform.position.z + 7.0f);
    }

    void Update()
    {
        CameraRotation();
    }

    public void Drive(float direction)
    {
        Vector3 translation = this.transform.forward;

        translation *= direction;

        this.transform.position += translation;
    }

    public void Turn(float rotation)
    {
        this.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), rotation);
    }

    public void LowerForks()
    {
        fork.transform.Translate(new Vector3(0.0f, -0.1f, 0.0f));
    }

    public void RaiseForks()
    {
        fork.transform.Translate(new Vector3(0.0f, 0.1f, 0.0f));
    }

    public void Ability()
    {

    }

    public void CameraRotation()
    {
       //float h = Input.GetAxis("Mouse Y");
       //float v = Input.GetAxis("Mouse X");
       //Camera.main.gameObject.transform.Rotate(transform.position, h* 40.0f);
    }

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        Camera.main.transform.position = transform.position + offset;
        Camera.main.transform.LookAt(transform.position);
    }

}
