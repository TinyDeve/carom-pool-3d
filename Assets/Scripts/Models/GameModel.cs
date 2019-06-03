using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace onur.pool.models
{
	public class GameModel {

        public GameData gameData;
        private string stringData;


        public int Shots
        {
            get
            {
                return gameData.shots;
            }
            set
            {
                gameData.shots = value;
            }
        }

        public int Score
        {
            get
            {
                return gameData.score;
            }
            set
            {
                gameData.score = value;
            }
        }

        public float Time
        {
            get
            {
                return gameData.time;
            }
            set
            {
                gameData.time = value;
            }
        }
        public float Sound
        {
            get
            {
                return gameData.sound;
            }
            set
            {
                gameData.sound = value;
            }
        }
        public float Music
        {
            get
            {
                return gameData.music;
            }
            set
            {
                gameData.music = value;
            }
        }
        private const string KeyGameData = "game_data";

        public void Save()
		{
            stringData = JsonUtility.ToJson(gameData);
            PlayerPrefs.SetString(KeyGameData,stringData);
		}

        public void Load()
        {
            stringData = PlayerPrefs.GetString(KeyGameData);
            gameData = JsonUtility.FromJson<GameData>(stringData);
            if(gameData == null)
            {
                gameData = new GameData();
            }
        }
	}

    [Serializable]
    public class GameData
    {
        public int score = 0;
        public int shots = 0;
        public float time = 0f;

        public float sound = 1.0f;
        public float music = 1.0f;
    }
}
