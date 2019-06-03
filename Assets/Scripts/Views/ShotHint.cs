using onur.pool.commands;
using onur.pool.signals;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotHint : View
{

    [SerializeField]
    private Transform m_hintBall;
    [SerializeField]
    private Transform m_hintPointer;
    [SerializeField]
    private Transform m_afterHitPointer;
    [SerializeField]
    private Transform m_parent;
    [SerializeField]
    private float m_ballRadius;
    [SerializeField]
    private float m_afterHitLength;

    private Vector3 m_raycastDirection;
    private RaycastHit hit;

    private float m_angle;
    private Vector3 m_hintRotation;
    private Vector3 m_hintPosition;
    private Vector3 m_hintBallPosition;
    private int layerMask;

    private GameController gameController;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        gameController = GameController.Instance;
        //Create cache values to minimize garbage creation
        m_raycastDirection = Vector3.zero;
        m_hintRotation = new Vector3(90.0f, 0, 0);
        m_hintPosition = new Vector3(0.1f, 0, 0.1f);
        m_hintBallPosition = Vector3.zero;
        //To disable all collision except balls
        layerMask = 1 << 8;
    }

    private void Update()
    {
        //if game is at play send raycast otherwise disable hint objects
        if (gameController.gameMode == GameController.GameMode.PLAY)
        {
            RaycastHint();
        }
        else
        {
            DisableDraw(false);
        }
    }

    private void RaycastHint()
    {
        //Get angle from camera holder to determine raycast direction
        m_angle = m_parent.localRotation.eulerAngles.y;
        //Debug.Log("Angle :"+shotPower+" "+angle);

        //Calculate raycast direction
        m_angle += 180f;
        m_angle = 2f * Mathf.PI * (m_angle / 360f);
        m_raycastDirection.x = -Mathf.Sin(m_angle);
        m_raycastDirection.z = -Mathf.Cos(m_angle);

        if (Physics.SphereCast(transform.position ,m_ballRadius, m_raycastDirection, out hit, 30,layerMask))
        {
            DrawHint(transform.position, hit.point, hit.collider.gameObject.transform.localPosition, hit.normal,hit.distance);
            DisableDraw(true);
        }
        else
        {
            DisableDraw(false);
        }
    }

    private void DrawHint(Vector3 startPoint,Vector3 hitPoint,Vector3 hitObjePoint,Vector3 hitnormal,float hitDistance)
    {
        //Place hint pointer and hint ball obje according to info from sphere raycast

        m_hintBallPosition = hitObjePoint + 2 * (hitPoint - hitObjePoint);
        m_hintBall.transform.position = m_hintBallPosition;

        m_hintRotation.y = m_parent.localRotation.eulerAngles.y;
        m_hintPosition.y = hitDistance / 2;
        m_hintPointer.transform.position = (startPoint + m_hintBallPosition) / 2;
        m_hintPointer.transform.localScale = m_hintPosition;
        m_hintPointer.transform.rotation =  Quaternion.Euler(m_hintRotation);
        /*
        m_rotation.y += 90;
        m_afterHitPointer.transform.position = m_hintBallPosition + hitnormal * m_afterHitLength;
        m_afterHitPointer.transform.localScale = new Vector3(0.1f, m_afterHitLength, 0.1f);
        m_afterHitPointer.transform.rotation = Quaternion.Euler(m_rotation);*/


    }
    private void DisableDraw(bool enable)
    {
        //Disable hint ball and pointer
        m_hintPointer.gameObject.SetActive(enable);
        m_hintBall.gameObject.SetActive(enable);
    }
}
