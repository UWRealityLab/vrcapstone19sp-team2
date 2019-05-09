using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRWeaponInteractor
{
	public class TargetRange : MonoBehaviour
	{
		public GameObject targetPrefab;
		public Text countdownText;
		public Text finalScore;

		//	Round settings
		public int targetsPerRound = 5;
		//How long target will wait to be shot
		public float targetLingerTime = 5f;
		//If not shot time until next target is spawned
		public float spawnNextTargetTime = 1f;
		//Delay before starting first round
		public float roundDelay = 3f;
		public bool movingTargets;
		public float targetMovementDistance = 0.2f;
		public float targetMovementTime = 2f;
		public bool beginOnStart;

		private int targetIndex;
		private bool inRound;
		private int roundPoints;
		private int maxPossiblePoints;
		private Coroutine roundRoutine;

		void Start()
		{
			if (targetPrefab == null) Debug.LogError("Target prefab is null", gameObject);

			TargetHitBox[] hitboxes = targetPrefab.GetComponentsInChildren<TargetHitBox>();
			int pointsPerTarget = 0;
			foreach(TargetHitBox hitbox in hitboxes) if (hitbox.points > pointsPerTarget) pointsPerTarget = hitbox.points;
			maxPossiblePoints = pointsPerTarget * targetsPerRound;

			countdownText.gameObject.SetActive(false);
			finalScore.gameObject.SetActive(false);

			if (beginOnStart) StartRound();
		}

		public void StartRound()
		{
			if (inRound) return;
			inRound = true;
			roundPoints = 0;
			if (roundRoutine != null) StopCoroutine(roundRoutine);
			finalScore.gameObject.SetActive(false);
			roundRoutine = StartCoroutine(RoundRoutine());
		}

		IEnumerator RoundRoutine()
		{
			StartCoroutine(RoundStartCountdown());
			for(int i=0; i<targetsPerRound; i++)
			{
				float elapsedTime = 0f;
				while(true)
				{
					elapsedTime += Time.deltaTime;
					if (elapsedTime > (i==0 ? roundDelay : spawnNextTargetTime)) break;
					yield return null;
				}
				SpawnTarget();
			}
			yield return new WaitForSeconds(targetLingerTime);
			inRound = false;
			ShowScoreScreen();
		}

		void SpawnTarget()
		{
			//Tell target it's linger time and if it can move
			GameObject targetInstance = Instantiate<GameObject>(targetPrefab);
			TargetRangeTarget target = targetInstance.GetComponentInChildren<TargetRangeTarget>();
			target.range = this;
		}

		IEnumerator RoundStartCountdown()
		{
			countdownText.gameObject.SetActive(true);
			float currentDelay = roundDelay;
			while(currentDelay > 0f)
			{
				if (currentDelay < 1f)
					countdownText.text = "Go!";
				else
					countdownText.text = (currentDelay-0.51f).ToString("0");
				currentDelay -= Time.deltaTime;
				yield return null;
			}
			countdownText.gameObject.SetActive(false);
		}

		void ShowScoreScreen()
		{
			float percent = ((float)roundPoints / (float)maxPossiblePoints) * 100f;
			finalScore.text = "You Scored\n" + roundPoints.ToString() + " / " + maxPossiblePoints + "\n" + percent.ToString("00") + "%";
			finalScore.gameObject.SetActive(true);
		}

		public void RegisterPoints(int points)
		{
			roundPoints += points;
		}
	}
}