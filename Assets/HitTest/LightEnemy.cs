using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EnemyHitTest {
	public class LightEnemy : Enemy {


		[Header("Duration")]
		public float HitDuration = 0.1f;
		public float HitAirDuration = 0.1f;
		public float WakeUpDuration = 0.5f;
		public float JuggleDuration = 0.5f;

		Coroutine JuggleRoutine;
		Coroutine WakeUpRoutine;
		Coroutine IdleRoutine;
		Coroutine FallRoutine;

		Coroutine HitGroundRoutine;
		Coroutine HitAirRoutine;

		Coroutine _CurrentRoutine;
		Coroutine CurrentRoutine{
			get{
				return _CurrentRoutine;
			}
			set{
				if (_CurrentRoutine != null) {
					StopCoroutine(_CurrentRoutine);
				}
				_CurrentRoutine = value;
			}
		}

		protected override void Update ()
		{
			base.Update ();
			UpdateWakeUp();
		}

		#region implemented abstract members of Enemy
		public override void Hit (Vector2 hitVector)
		{

			// waking up? don't disturb
			if (State == EnemyState.WAKE_UP) 
				return;
			
			if (IsGrounded) {
				HitGroundRoutine = StartCoroutine(IEHitGround(hitVector));
				CurrentRoutine = HitGroundRoutine;
			}else{
				HitAirRoutine = StartCoroutine(IEHitAir());
				CurrentRoutine = HitAirRoutine;
			}

		}

		public override void Juggle (Vector2 hitVector)
		{
			JuggleRoutine = StartCoroutine(IEJuggle(hitVector));
			CurrentRoutine = JuggleRoutine;
		}

		public override void Idle ()
		{
			IdleRoutine = StartCoroutine(IEIdle());
			CurrentRoutine = IdleRoutine;
		}

		public override void Fall ()
		{
			FallRoutine = StartCoroutine(IEFall());
			CurrentRoutine = FallRoutine;
		}

		public override void WakeUp () 
		{
			WakeUpRoutine = StartCoroutine(IEWakeUp());
			CurrentRoutine = WakeUpRoutine;
		}


		#endregion

		#region Action Routines
		IEnumerator IEHitGround(Vector2 hitVector) {

			// normal hit
			State = EnemyState.HIT;				
			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			transform.DOMoveX(xMovement, HitDuration, false);

			if (OnBeginHitGround != null)
				OnBeginHitGround(hitVector);
			
			yield return new WaitForSeconds(HitDuration);

			if (OnEndHit != null) 
				OnEndHit();

			Idle();

		}

		IEnumerator IEHitAir() {
			State = EnemyState.HIT_AIR;
			yield return null;

			if (OnBeginHitAir != null)
				OnBeginHitAir();

			yield return new WaitForSeconds(HitAirDuration);

			if (OnEndHit != null) 
				OnEndHit();

			Fall();
		}


		IEnumerator IEJuggle(Vector2 hitVector) {
			State = EnemyState.JUGGLE;

			Vector3 prevPos = Body.transform.position;
			float yMovement = 0;
			yMovement = prevPos.y + hitVector.y;
			Body.DOMoveY(yMovement, JuggleDuration, false);

			if (OnJuggle != null)
				OnJuggle(hitVector);

			yield return new WaitForSeconds(JuggleDuration);

			Fall();
		}

		IEnumerator IEWakeUp() {
			State = EnemyState.WAKE_UP;

			if (OnWakeUp != null)
				OnWakeUp();
			yield return new WaitForSeconds(WakeUpDuration);

			Idle();
					
		}

		IEnumerator IEIdle() {
			State = EnemyState.IDLE;
			yield return null;

			if (OnIdle != null)
				OnIdle();
		}

		IEnumerator IEFall() {
			State = EnemyState.FALL;
			yield return null;

			if (OnFall != null)
				OnFall();
		}



		#endregion

		void UpdateWakeUp() {
			// wake up when grounded
			if ( (State == EnemyState.FALL) && IsGrounded) {			
				WakeUp();
			}
		}

		protected override void UpdateSpeed ()
		{
			if (!IsGrounded) {
				Body.gravityScale = 0;

				if (State != EnemyState.JUGGLE && State != EnemyState.HIT_AIR) {
					Body.velocity = new Vector2(0, -FallingSpeed);
				}else{
					Body.velocity = new Vector2(0, 0);
				}
			}else{
				Body.gravityScale = 1;
				Body.velocity = new Vector2(0, 0);
			}
		}
	}
}