using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using onur.pool.signals;
using onur.pool.views.ball;
using onur.pool.views.handle;
using onur.pool.models;

namespace onur.pool.commands
{
    public class GameController : View
    {

        public enum GameMode
        {
            NOGAME,
            PLAY,
            MOVING
        }

        public static GameController Instance;

        [Inject]
        public ShotSignal ShotSignal { get; set; }

        [Inject]
        public ShotPower ShotPower { get; set; }

        [Inject]
        public GameModeSignal GameModeSignal { get; set; }

        [Inject]
        public GameModel GameModel { get; set; }

        [SerializeField] private UIHandle m_uIHandle;
        [SerializeField] private Transform m_cameraHandle;
        [SerializeField] private BallHandle m_ballHandle;
        [SerializeField] private float m_maxShotTime = 1.0f;
        [SerializeField] private int m_finishScore = 5;

        [HideInInspector]
        public bool gamePlaying;
        [HideInInspector]
        public bool ballMoving;
        [HideInInspector]
        public List<PoolBall>  gameBalls= new List<PoolBall>();

        private bool isRePlay;

        private float m_time = 0f;
        private int m_shots = 0;
        private int m_score = 0;

        private float lastShotAngle;
        private float lastShotPower;
        private List<Vector3> lastPositions = new List<Vector3>();

        public GameMode gameMode;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        protected override void Start()
        {
            base.Start();
            gameMode = GameMode.NOGAME;
            //Listen when will shot be fired
            ShotSignal.AddListener((float shotPower) => ShotBall(shotPower));
            //Change shot power at bar to inform player
            ShotPower.AddListener((float shotPower) => ChangeBallPowerBar(shotPower));
            LoadScoreBoard();
        }

        private void Update()
        {

            if(gameMode == GameMode.PLAY)
            {
                m_time += Time.deltaTime;
                if (0 == Time.frameCount % 20)
                {
                    m_uIHandle.SetTime(m_time);
                }
            }
            else if(gameMode == GameMode.MOVING)
            {
                if (0 == Time.frameCount % 23)
                {
                    if (IsBallsStoped())
                    {
                        //Debug.Log("TimeToMove");
                        StopMove();
                        PlayFinish(true);
                    }
                }
            }
        }


        public void StartGame()
        {
            PlayFinish(true);
            ReStartGame();
        }

        public void ReStartGame()
        {
            //Reset all Values for fresh start :)
            m_time = 0f;
            m_shots = 0;
            m_score = 0;
            m_uIHandle.SetScore(m_score);
            m_uIHandle.SetShotCount(m_shots);
            m_uIHandle.SetTime(m_time);
            m_uIHandle.SetShotPower(0);
            m_uIHandle.reWatchButton.SetActive(false);
            m_uIHandle.PlayFinishGame(true);
            gameMode = GameMode.PLAY;
        }

        public void FinishGame()
        {
            //Finish game and bring win UI
            m_uIHandle.PlayFinishGame(false);
            gameMode = GameMode.NOGAME;
            PlayFinish(false);
        }

        public void PlayFinish(bool play)
        {
            //Stop getting shot input at input handle view
            GameModeSignal.Dispatch(play);

            //Change game mode to stop time and play functions
            gameMode = play ? GameMode.PLAY : GameMode.MOVING;
            isRePlay = false;
            
            gamePlaying = play;

            //Handle UI visuals
            if (play)
            {
                m_uIHandle.StopGameVisuals(true);
                m_uIHandle.SetShotPower(0f);
            }
            else
            {
                m_uIHandle.StopGameVisuals(false);
            }
            
        }

        public void LoadScoreBoard()
        {
            GameModel.Load();
            m_uIHandle.SetScore(GameModel.Score);
            m_uIHandle.SetShotCount(GameModel.Shots);
            m_uIHandle.SetTime(GameModel.Time);
        }

        public void RePlay()
        {
            /*It is simple replay system it can be changed with more accurate
            *collide point and stop point list and interpolation between points.
            */

            //Get ball at old spot and fire same shot
            for (int i = 0; i < gameBalls.Count; i++)
            {
                gameBalls[i].BallTransform.position = lastPositions[i];
            }
            m_ballHandle.poolBall.AddForce(lastShotPower, lastShotAngle);
            PlayFinish(false);
            isRePlay = true;
        }

        public void SaveLastPositions()
        {
            //Save last position for replay
            lastPositions.Clear();
            foreach (PoolBall ball in gameBalls)
            {
                lastPositions.Add(ball.BallTransform.position);
            }
        }

        public bool IsBallsStoped()
        {
            //Check if all balls are stop to finish turn
            foreach(PoolBall ball in gameBalls)
            {
                if (ball.BallRigidbody.velocity.sqrMagnitude > 0.01f)
                {
                    return false;
                }
            }
            return true;
        }

        private void StopMove()
        {
            //Stop balls if their speed is to low
            foreach (PoolBall ball in gameBalls)
            {
                ball.BallRigidbody.velocity = Vector3.zero;
                ball.BallRigidbody.angularVelocity = Vector3.zero;
            }
        }

        public void GameScored()
        {
            //If score not happening at replay then increase score
            if (!isRePlay)
            {
                m_score += 1;
                m_uIHandle.SetScore(m_score);
            }
            //if reached at finish game score end game
            if (m_score == m_finishScore)
            {
                GameModel.Score = m_score;
                GameModel.Shots = m_shots;
                GameModel.Time = m_time;
                GameModel.Save();
                FinishGame();
            }
        }


        private void ChangeBallPowerBar(float shotPower)
        {
            //Change power bar with info from input handle view
            shotPower /= m_maxShotTime;
            m_uIHandle.SetShotPower(shotPower);
        }

        private void ShotBall(float shotPower)
        {
            //Calculate shot variables
            shotPower /= m_maxShotTime;
            shotPower = shotPower > 1.0f ? 1 : shotPower;
            float angle = m_cameraHandle.localRotation.eulerAngles.y;

            //Change game mode to moving
            PlayFinish(false);

            //Increase shot count and display it
            m_shots += 1;
            m_uIHandle.SetShotCount(m_shots);

            //Save last shot variables and ball positions
            lastShotAngle = angle;
            lastShotPower = shotPower;
            SaveLastPositions();

            //Fire the shot
            m_ballHandle.poolBall.AddForce(shotPower,angle);
        }

    }
}
  
