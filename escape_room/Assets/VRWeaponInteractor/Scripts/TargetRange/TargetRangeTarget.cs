using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRWeaponInteractor
{
	public class TargetRangeTarget : MonoBehaviour 
	{
		private TargetRange _range;

		public TargetRange range
		{
			get { return _range; }
			set { _range = value; }
		}

		private bool sideWays;
		private bool direction;
		//Time interval
		private float _moveDelta = 0.01f;
		//Amount to move each time interval
		private Vector3 _movePositionDelta;
		public Text hitText;
		public Animation _anim;

		private bool _hit;
		private float _lingerTime;
		private float _elapsedTime;

		void Start()
		{
			hitText.gameObject.SetActive(false);
			FindPosition();
		}

		void FixedUpdate()
		{
			if (_hit) return;
			_lingerTime += Time.deltaTime;
			if (_lingerTime > _range.targetLingerTime)
			{
				PlayMiss();
			}

			if (!_range.movingTargets) return;

			_elapsedTime += Time.deltaTime;
			if (_elapsedTime > _moveDelta)
			{
				_elapsedTime = 0f;
				transform.position += _movePositionDelta;
			}
		}

		void FindPosition()
		{
			BoxCollider collider = _range.GetComponent<BoxCollider>();

			float xPos = 0f;
			float yPos = 0f;
			float zPos = 0f;
			if (_range.movingTargets)
			{
				sideWays = Random.Range(0,2) > 0.5f;
				direction = Random.Range(0,2) > 0.5f;
				if (sideWays)
				{
					float offset = collider.size.x * _range.targetMovementDistance;
					float xMoveDelta = offset/(_range.targetMovementTime/_moveDelta);
					if (direction) // Left
					{
						xPos = collider.transform.position.x + Random.Range((-collider.size.x*0.5f)+offset, (collider.size.x*0.5f));
						_movePositionDelta = new Vector3(-xMoveDelta, 0f, 0f);
					} else // Right
					{
						xPos = collider.transform.position.x + Random.Range((-collider.size.x*0.5f), (collider.size.x*0.5f)-offset);
						_movePositionDelta = new Vector3(xMoveDelta, 0f, 0f);
					}
					yPos = collider.transform.position.y + Random.Range(-collider.size.y*0.5f, collider.size.y*0.5f);
					zPos = collider.transform.position.z + Random.Range(-collider.size.z*0.5f, collider.size.z*0.5f);
				} else
				{
					float offset = collider.size.y * _range.targetMovementDistance;
					float yMoveDelta = offset/(_range.targetMovementTime/_moveDelta);
					xPos = collider.transform.position.x + Random.Range(-collider.size.x*0.5f, collider.size.x*0.5f);
					if (direction) // Down
					{
						yPos = collider.transform.position.y + Random.Range((-collider.size.y*0.5f)+offset, (collider.size.y*0.5f));
						_movePositionDelta = new Vector3(0f, -yMoveDelta, 0f);
					} else // Up
					{
						yPos = collider.transform.position.y + Random.Range((-collider.size.y*0.5f), (collider.size.y*0.5f)-offset);
						_movePositionDelta = new Vector3(0f, yMoveDelta, 0f);
					}
					zPos = collider.transform.position.z + Random.Range(-collider.size.z*0.5f, collider.size.z*0.5f);
				}
			} else
			{
				xPos = collider.transform.position.x + Random.Range(-collider.size.x*0.5f, collider.size.x*0.5f);
				yPos = collider.transform.position.y + Random.Range(-collider.size.y*0.5f, collider.size.y*0.5f);
				zPos = collider.transform.position.z + Random.Range(-collider.size.z*0.5f, collider.size.z*0.5f);
			}

			Vector3 randomPositionInBox = new Vector3(xPos, yPos, zPos);

			transform.position = randomPositionInBox;
		}

		public void PlayHit(int hitPoints)
		{
			if (_hit) return;
			_hit = true;
			_anim.Play();
			hitText.text = hitPoints.ToString() + "!";
			_range.RegisterPoints(hitPoints);
			StartCoroutine(HitTextCo());
			Destroy(gameObject, 2f);
		}

		public void PlayMiss()
		{
			if (_hit) return;
			_hit = true;
			_anim.Play();
			hitText.text = "Miss!";
			StartCoroutine(HitTextCo());
			Destroy(gameObject, 0.5f);
		}

		IEnumerator HitTextCo()
		{
			hitText.gameObject.SetActive(true);
			float t = 0;
			float elapsedTime = 0f;
			float moveScale = 0.5f/hitText.transform.lossyScale.y;
			while(t < 1)
			{
				elapsedTime += Time.deltaTime;
				t = elapsedTime / 2f;
				hitText.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, moveScale, 0f), t);
				yield return null;
			}
			hitText.gameObject.SetActive(false);
		}
	}
}