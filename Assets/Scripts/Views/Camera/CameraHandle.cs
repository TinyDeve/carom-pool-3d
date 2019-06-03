using onur.pool.signals;
using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace onur.pool.views.camera
{
    public class CameraHandle : View
    {
        [Inject]
        public MoveSignal MoveSignal { get; set; }

        [SerializeField]
        private float m_cameraVerticalMaxAngle;
        [SerializeField]
        private float m_cameraVerticalMinAngle;
        [SerializeField]
        private Transform m_cameraHanlerTransform;
        [SerializeField]
        private Transform m_playerTransform;


        private float m_screenHeigth;
        private float m_screenWitdh;
        private Vector3 m_cameraPosition;
        private Vector3 m_cameraRotation;

        private float m_cameraVerticalAngle;
        private float m_cameraHorizontalAngle;
        private float m_cameraVerticalGap;


        protected override void Awake()
        {
            base.Awake();
            //Cache values to optimize code and garbage 
            m_screenHeigth = Screen.height;
            m_screenWitdh = Screen.width;
            m_cameraRotation = new Vector3(30, 0, 0);
        }

        protected override void Start()
        {
            base.Start();
            //Get input from input handle view via dispacth signal
            MoveSignal.AddListener((float verticalMove, float horizantalMove) => HandleMove(verticalMove, horizantalMove));
            //Cache vertical angle gap
            m_cameraVerticalGap = m_cameraVerticalMaxAngle - m_cameraVerticalMinAngle;

        }

        private void Update()
        {
            //Follow player ball
            transform.localPosition = m_playerTransform.localPosition;
        }

        private void HandleMove(float verticalMove, float horizantalMove)
        {
            //Debug.Log("Get Values :" + verticalMove + " " + horizantalMove);
            //Calculate new transform values

            m_cameraRotation = m_cameraHanlerTransform.eulerAngles;
            m_cameraVerticalAngle = m_cameraRotation.x 
                + (horizantalMove / m_screenHeigth)*m_cameraVerticalGap;

            m_cameraHorizontalAngle = m_cameraRotation.y 
                + (verticalMove / m_screenHeigth)*360.0f;

            //Check if camera will be out of vertical borders
            if(m_cameraVerticalAngle > m_cameraVerticalMaxAngle)
            {
                m_cameraRotation.x = m_cameraVerticalMaxAngle;
            }
            else if(m_cameraVerticalAngle < m_cameraVerticalMinAngle)
            {
                m_cameraRotation.x = m_cameraVerticalMinAngle;
            }
            else
            {
                m_cameraRotation.x = m_cameraVerticalAngle;
            }

            m_cameraRotation.y = m_cameraHorizontalAngle;

            //Assing new camera values to transform
            m_cameraHanlerTransform.localRotation = Quaternion.Euler(m_cameraRotation);
        }
    }
}
