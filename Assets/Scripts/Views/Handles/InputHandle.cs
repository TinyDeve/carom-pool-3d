using strange.extensions.mediation.impl;
using onur.pool.signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace onur.pool.views.input
{
    public class InputHandle : View
    {

        [Inject]
        public MoveSignal MoveSignal { get; set; }

        [Inject]
        public ShotSignal ShotSignal { get; set; }

        [Inject]
        public ShotPower ShotPower { get; set; }

        [Inject]
        public GameModeSignal GameModeSignal { get; set; }
        
        //To save move of mouse at screen
        private float m_moveHorizontal;
        private float m_moveVertical;

        //To save how long is pressed to shot
        private float m_shotTime;

        public bool isGamePlaying;

        protected override void Awake()
        {
            base.Awake();
            isGamePlaying = false;
        }



        protected override void Start()
        {
            base.Start();
            GameModeSignal.AddListener(
                (bool gameMode)=>ChangeGameMode(gameMode));
        }

        private void ChangeGameMode(bool gameMode)
        {
            isGamePlaying = gameMode;
        }

        private void Update()
        {
            //If game is not in the play,
            //there is not need for collecting input
            if (isGamePlaying)
            {
                //Debug.Log("Update");
                //Save time if space bar is pressed
                //And return to do not change camera angle
                if (Input.GetKey(KeyCode.Space))
                {
                    //Debug.Log("KeyDown");
                    m_shotTime += Time.deltaTime;
                    ShotPower.Dispatch(m_shotTime);
                    return;
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    //Debug.Log("KeyMove");
                    m_shotTime += Time.deltaTime;
                    ShotSignal.Dispatch(m_shotTime);
                    m_shotTime = 0;
                    return;
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("MouseDown :"+ Input.mousePosition.x+" "+ Input.mousePosition.y);
                //Get move values and send them to listener at camera hanle
                m_moveHorizontal = Input.mousePosition.x;
                m_moveVertical = Input.mousePosition.y;
                return;
            }
            if (Input.GetMouseButton(0))
            {
                //Debug.Log("MouseHeld :" + Input.mousePosition.x + " " + Input.mousePosition.y);
                //Debug.Log("MouseMoveHeld :" + (Input.mousePosition.x - m_moveHorizontal) + " " + (Input.mousePosition.y - m_moveVertical));
                //Get move values and send them to listener at camera hanle
                m_moveHorizontal = Input.mousePosition.x - m_moveHorizontal;
                m_moveVertical = Input.mousePosition.y - m_moveVertical;
                MoveSignal.Dispatch(m_moveHorizontal, m_moveVertical);
                m_moveHorizontal = Input.mousePosition.x;
                m_moveVertical = Input.mousePosition.y;
            }
        }
    }
}

