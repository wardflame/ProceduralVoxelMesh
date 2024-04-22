using Essence.ZedExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Essence
{
    public class TimeTrialGameManager : MonoBehaviour
    {
        public static TimeTrialGameManager instance;

        public TrialModeUI trialUI;

        public List<TargetEntity> targets = new();

        public int previousScore;
        public int activeScore;
        public float trialTimer;
        public bool trialActive;

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
            if (trialActive)
            {
                activeScore++;
                trialUI.currentScore.text = "CURRENT SCORE: " + activeScore.ToString();
            }
        }

        public void StartTrial()
        {
            if (trialActive) return;

            if (trialUI.cg.alpha <= 0) StartCoroutine(trialUI.cg.FadeIn());
            StartCoroutine(RunTrial());
        }

        private IEnumerator RunTrial()
        {
            trialActive = true;

            float timer = trialTimer;
            while (timer > 0)
            {
                DisplayTimeRemaining(timer);

                yield return new WaitForSeconds(1);

                timer -= 1;

                DisplayTimeRemaining(timer);
            }

            yield return new WaitForSeconds(1);

            previousScore = activeScore;
            activeScore = 0;

            trialUI.previousScore.text = "PREVIOUS SCORE: " + previousScore.ToString();
            trialUI.currentScore.text = "CURRENT SCORE: " + activeScore.ToString();

            trialActive = false;
            StartCoroutine(HideUIAfterTime());
        }

        private void DisplayTimeRemaining(float time)
        {
            string timerStr;

            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);

            timerStr = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (time <= trialTimer * .25f)
            {
                timerStr.Insert(0, "<color=red>");
                timerStr += "</color>";
            }

            trialUI.timer.text = timerStr;
        }

        private IEnumerator HideUIAfterTime()
        {
            yield return new WaitForSeconds(10);

            if (!trialActive) StartCoroutine(trialUI.cg.FadeOut());
        }
    }
}
