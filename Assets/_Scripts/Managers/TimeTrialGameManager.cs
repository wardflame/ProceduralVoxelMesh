using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence
{
    public class TimeTrialGameManager : MonoBehaviour
    {
        public static TimeTrialGameManager instance;

        public List<TargetEntity> targets = new();

        public int gameScore;
        public bool trialStarted;
        public float timer;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].TargetHit += AddScore;
            }
        }

        public void AddScore()
        {
            if (trialStarted) gameScore++;
        }
    }
}
