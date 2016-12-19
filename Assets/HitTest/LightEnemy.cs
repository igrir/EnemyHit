using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EnemyHitTest {
	public class LightEnemy : Enemy {

		[Space(15)]
		public Ease HitGroundEase;
		public float HitGroundDuration = 0.1f;

		[Space(15)]
		public Ease HitAirEase;
		public float HitAirDuration = 0.1f;

		[Space(15)]
		public Ease JuggleEase;
		public float JuggleDuration = 0.5f;

		[Space(15)]
		public float BounceDuration = 0.5f;

		[Space(15)]
		public float WakeUpDuration = 0.5f;


		Coroutine JuggleRoutine;
		Coroutine WakeUpRoutine;
		Coroutine IdleRoutine;
		Coroutine FallRoutine;
		Coroutine HitGroundRoutine;
		Coroutine HitAirRoutine;
		Coroutine BounceRoutine;

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
			CheckSlamFallGrounded();
		}

		#region implemented abstract members of Enemy
		public override void Hit (Vector2 hitVector)
		{

			// waking up? don't disturb
			if (State == EnemyState.WAKE_UP || State == EnemyState.BOUNCE) 
				return;
			
			if (IsGrounded) {
				HitGroundRoutine = StartCoroutine(IEHitGround(hitVector));
				CurrentRoutine = HitGroundRoutine;
			}else{
				HitAirRoutine = StartCoroutine(IEHitAir(hitVector));
				CurrentRoutine = HitAirRoutine;
			}

		}

		public override void Juggle (Vector2 hitVector)
		{

			// waking up? don't disturb
			if (State == EnemyState.WAKE_UP || State == EnemyState.BOUNCE) 
				return;

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

		public override void Bounce ()
		{
			BounceRoutine = StartCoroutine(IEBounceRoutine());
			CurrentRoutine = BounceRoutine;
		}

		#endregion

		#region Action Routines
		IEnumerator IEHitGround(Vector2 hitVector) {

			// normal hit
			State = EnemyState.HIT;				
			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			transform.DOMoveX(xMovement, HitGroundDuration, false).SetEase(HitGroundEase);

			if (OnBeginHitGround != null)
				OnBeginHitGround(hitVector);
			
			yield return new WaitForSeconds(HitGroundDuration);

			if (OnEndHit != null) 
				OnEndHit();

			Idle();

		}

		IEnumerator IEHitAir(Vector2 hitVector) {
			State = EnemyState.HIT_AIR;

			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			float yMovement = prevPos.y + hitVector.y;
			transform.DOMove(new Vector2(xMovement, yMovement), HitGroundDuration, false).SetEase(HitAirEase);

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
			Body.DOMoveY(yMovement, JuggleDuration, false).SetEase(JuggleEase);

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

		IEnumerator IEBounceRoutine() {
			State = EnemyState.BOUNCE;
			yield return null;

			if (OnBounce != null)
				OnBounce();

			yield return new WaitForSeconds(BounceDuration);

			WakeUp();
		}

		#endregion

		void CheckSlamFallGrounded() {
			if (State == EnemyState.FALL) {
				if (IsFallGrounded) {
					SlamFall();
					Bounce();
				}
			}
		}

		// langsung pok
		void SlamFall() {
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(this.transform.position, new Vector2(0, -1), FallGroundRayDistance);
			for (int i = 0; i < groundRays.Length; i++) {
				Collider2D coll = groundRays[i].collider;
				if (coll != null) {					
					// not self
					if (!SelfCollider.Contains(coll)){
						
						transform.position = new Vector2(this.transform.position.x, groundRays[i].point.y + GroundRayDistance);
					}
				}
			}
		}

		void UpdateWakeUp() {
			// wake up when grounded
//			if ( (State == EnemyState.FALL) && IsGrounded) {			
//				WakeUp();
//			}
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