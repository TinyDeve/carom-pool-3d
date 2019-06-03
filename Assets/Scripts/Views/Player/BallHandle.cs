using onur.pool.commands;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace onur.pool.views.ball
{
    public class BallHandle : View
    {

        [SerializeField]
        private Rigidbody m_rigidbody;
        [SerializeField]
        private bool m_isPlayer;

        public PoolBall poolBall;
        public CaromPoolDecorator caromPoolBall;

        private GameController gameController;
        private float m_maxForce = 1000.0f;


        protected override void Start()
        {
            base.Start();
            gameController = GameController.Instance;
            poolBall = new PoolBall(m_rigidbody, transform,gameController, m_maxForce);
            caromPoolBall = new CaromPoolDecorator(poolBall, m_isPlayer,gameController);
            //caromPoolBall.SetBall(caromPoolBall);

            gameController.gameBalls.Add(poolBall);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                caromPoolBall.OnCollisionEnterPoolBall(collision);
            }
        }

    }

    public abstract class Ball
    {
        public Rigidbody BallRigidbody { get; set; }
        public Transform BallTransform { get; set; }

        public abstract void AddForce(float power, float angle);
        public abstract void PlaySound(float volume);
        public abstract void OnCollisionEnterPoolBall(Collision collision);
    }

    public class PoolBall : Ball
    {
        public GameController gameController;
        private float m_maxForce;
        private float m_maxSpeed;
        private Vector3 ab;

        public PoolBall(Rigidbody rigidbody,Transform transform,GameController gameController,float maxForce)
        {
            BallRigidbody = rigidbody;
            BallTransform = transform;
            m_maxForce = maxForce;
            this.gameController = gameController;


            ab = Vector3.zero;
            m_maxSpeed = maxForce/(rigidbody.mass*4);
        }


        public override void AddForce(float power, float angle)
        {
            //Debug.Log("power angle :" + power.ToString() + " " + angle.ToString());
            angle += 180f;
            angle = 2f * Mathf.PI * (angle / 360f);
            ab.x = power * Mathf.Sin(angle) * -m_maxForce;
            ab.z = power * Mathf.Cos(angle) * -m_maxForce;

            //Debug.Log("Shot Vector :"+ ab.ToString());
            BallRigidbody.AddForce(ab);
        }

        public override void OnCollisionEnterPoolBall(Collision collision)
        {
            PlaySound(BallRigidbody.velocity.sqrMagnitude / m_maxSpeed);

        }

        public override void PlaySound(float volume)
        {
            GameMaster.Instance.sound.PlayHitSound(volume);
        }
    }

    public abstract class Decorator : Ball
    {
        protected Ball m_ball;

        public Decorator(Ball ball)
        {
            m_ball = ball;
        }

        public override void AddForce(float power, float angle)
        {
            m_ball.AddForce(power, angle);
        }

        public override void OnCollisionEnterPoolBall(Collision collision)
        {
            m_ball.OnCollisionEnterPoolBall(collision);
        }

        public override void PlaySound(float volume)
        {
            m_ball.PlaySound(volume);
        }

    }
    public class CaromPoolDecorator : Decorator
    {
        private bool isPlayer;
        private bool canScore;
        private int firstHit;
        private GameController gameController;

        public CaromPoolDecorator(Ball ball,bool isPlayer,GameController gameController) : base(ball)
        {
            this.isPlayer = isPlayer;
            this.gameController = gameController;
        }

        public override void AddForce(float power, float angle)
        {
            base.AddForce(power, angle);
            ResetHitList();
        }

        public override void OnCollisionEnterPoolBall(Collision collision)
        {
            base.OnCollisionEnterPoolBall(collision);
            if (isPlayer)
            {
                HitOther(collision.gameObject.GetInstanceID());
            }
            
        }

        public override void PlaySound(float volume)
        {
            base.PlaySound(volume);
        }

        private void HitOther(int id)
        {/*
            if (!canScore)
                return;*/


            if(firstHit == 0)
            {
                firstHit = id;
            }
            else if (id != firstHit)
            {
                firstHit = 0;
                gameController.GameScored();
                canScore = false;
            }
        }

        private void ResetHitList()
        {
            firstHit = 0;
            canScore = true;
        }
    }
}
