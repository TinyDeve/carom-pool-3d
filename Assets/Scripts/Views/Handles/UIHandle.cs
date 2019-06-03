using onur.pool.commands;
using onur.pool.models;
using strange.extensions.mediation.impl;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace onur.pool.views.handle
{
    public class UIHandle : View
    {
        [Inject]
        public GameModel GameModel { get; set; }

        [Header("UI Panels")]
        [SerializeField]
        private GameObject m_start;
        [SerializeField]
        private GameObject m_game;
        [SerializeField]
        private GameObject m_finish;
        [SerializeField]
        private GameObject m_scoreBoard;


        [Header("UI Elements")]
        [SerializeField]
        private Slider m_sound;
        [SerializeField]
        private Slider m_music;
        [SerializeField]
        private Slider m_shotPower;
        [SerializeField]
        private TextMeshProUGUI m_time;
        [SerializeField]
        private TextMeshProUGUI m_score;
        [SerializeField]
        private TextMeshProUGUI m_shotCount;


        public GameObject shotSlider;
        public GameObject reWatchButton;

        private GameController gameController;

        private StringBuilder stringBuilder;

        protected override void Start()
        {
            base.Start();
            stringBuilder = new StringBuilder(8,16);
            gameController = GameController.Instance;


            m_start.SetActive(true);
            m_scoreBoard.SetActive(true);

            //Set sound and music start visuals
            GameModel.Load();
            m_sound.gameObject.SetActive(GameModel.Sound <= 1.0f);
            m_sound.SetValueWithoutNotify(GameModel.Sound % 1.001f);
            m_music.gameObject.SetActive(GameModel.Music <= 1.0f);
            m_music.SetValueWithoutNotify(GameModel.Music % 1.001f);
            GameMaster.Instance.music.mute = GameModel.Music > 1.0f;
            GameMaster.Instance.music.volume = GameModel.Music % 1.001f;
            GameMaster.Instance.soundSource.mute = GameModel.Sound > 1.0f;
            GameMaster.Instance.soundSource.volume = GameModel.Sound % 1.001f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameModel.Save();
        }

        //Open and close uı panels according to game mode
        #region UI panels handle
        public void StartGame()
        {
            m_start.SetActive(false);
            m_game.SetActive(true);
            gameController.StartGame();
            GameModel.Save();
        }
        public void PlayFinishGame(bool play)
        {
            m_game.SetActive(play);
            m_finish.SetActive(!play);
            gameController.PlayFinish(play);
        }

        public void Reset()
        {
            gameController.ReStartGame();
        }
        #endregion

        //Function that will be assing UI events
        #region Button function
        public void ReWatch()
        {
            gameController.RePlay();
        }

        public void CloseOpenSound()
        {
            bool openVolume = GameModel.Sound > 1.0f;

            GameMaster.Instance.soundSource.mute = !openVolume;
            m_sound.gameObject.SetActive(openVolume);

            GameModel.Sound += openVolume ? -1f : +1f;
        }

        public void CloseOpenMusic()
        {
            bool openVolume = GameModel.Music > 1.0f;

            GameMaster.Instance.music.mute = !openVolume;
            m_music.gameObject.SetActive(openVolume);

            GameModel.Music += openVolume ? -1f : +1f;
        }

        public void MusicVolumeChanged()
        {
            float volume = m_music.value;

            GameModel.Music = volume;
            GameMaster.Instance.music.volume = volume;
        }

        public void SoundVolumeChanged()
        {
            float volume = m_sound.value;

            GameModel.Sound = volume;
            GameMaster.Instance.soundSource.volume = volume;
        }
        #endregion

        //Functions that will change UI appearance
        #region UI visuals

        public void StopGameVisuals(bool active)
        {
            shotSlider.SetActive(active);
            reWatchButton.SetActive(active);
        }

        //Basic functions
        private void SetProgress(float progress, Slider slider)
        {
            /*
            if(slider != null)
            {
                return;
            }*/
            slider.value = progress;
        }

        private void SetText(StringBuilder stringBuilder , TextMeshProUGUI textMesh)
        {
            textMesh.SetText(stringBuilder);
        }

        //Public functions
        public void SetScore(int score)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(score);

            SetText(stringBuilder, m_score);
        }
        public void SetTime(float pastSeconds)
        {
            //Debug.Log("Time");
            int minutes = (int) pastSeconds / 60;
            int leftSeconds = (int) pastSeconds % 60;

            stringBuilder.Length = 0;
            stringBuilder.AppendFormat("{0}:{1:00}", minutes, leftSeconds);

            SetText(stringBuilder, m_time);
        }
        public void SetShotCount(int shotCount)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(shotCount);

            SetText(stringBuilder, m_shotCount);
        }

        public void SetSound(float volume)
        {
            SetProgress(volume, m_sound);
        }

        public void SetMusic(float volume)
        {
            SetProgress(volume, m_music);
        }
        public void SetShotPower(float power)
        {
            SetProgress(power, m_shotPower);
        }
        #endregion
    }
}

