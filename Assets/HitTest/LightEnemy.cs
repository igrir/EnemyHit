using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EnemyHitTest {
	public class LightEnemy : Enemy {
		// bounce setelah dipukul dari atas
		public float BounceImpact = 2;

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
		public Ease BounceImpactEase;
		public float BounceImpactDuration = 0.5f;

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
		Coroutine SlamRoutine;
		Coroutine BounceImpactRoutine;

		Tween _MovementTween;
		Tween MovementTween{
			get{
				return _MovementTween;
			}
			set{
				if (_MovementTween != null)
					_MovementTween.Kill(false);
				_MovementTween = value;
			}
		}

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
			UpdateImmediateGrounded();
		}

		#region implemented abstract members of Enemy
		public override void Hit (Vector2 hitVector)
		{

			// waking up? don't disturb
			if (State == EnemyState.WAKE_UP || State == EnemyState.BOUNCE) 
				return;

			// grounded and wasn't hit air
			if (IsGrounded && State != EnemyState.HIT_AIR) {
				HitGroundRoutine = StartCoroutine(IEHitGround(hitVector));
				CurrentRoutine = HitGroundRoutine;
			}else{

				if (IsFallGrounded) {
					BounceImpactRoutine = StartCoroutine(IEBounceImpact(new Vector2(hitVector.x, BounceImpact)));
					CurrentRoutine = BounceImpactRoutine;					
				}else{
					HitAirRoutine = StartCoroutine(IEHitAir(hitVector));
					CurrentRoutine = HitAirRoutine;					
				}
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

		public override void Slam ()
		{
			if (State == EnemyState.FALL || State == EnemyState.JUGGLE) {
				SlamRoutine = StartCoroutine(IESlamRoutine());
				CurrentRoutine = SlamRoutine;
			}
		}

		#endregion

		#region Action Routines
		IEnumerator IEHitGround(Vector2 hitVector) {

			// normal hit
			State = EnemyState.HIT;				
			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			MovementTween = Body.DOMoveX(xMovement, HitGroundDuration, false).SetEase(HitGroundEase);

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
			MovementTween = Body.DOMove(new Vector2(xMovement, yMovement), HitGroundDuration, false).SetEase(HitAirEase);

			if (OnBeginHitAir != null)
				OnBeginHitAir();

			yield return new WaitForSeconds(HitAirDuration);

			if (OnEndHit != null) 
				OnEndHit();

			// ada gerakan ke bawah pas di udara
			if (hitVector.y < 0) {
				// endingnya ngeslam aja
				Slam();
			}else{
				// kalau nggak ada jadi kayak daun aja
				Fall();
			}
		}

		IEnumerator IEBounceImpact(Vector2 hitVector) {
			State = EnemyState.BOUNCE_IMPACT;

			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			float yMovement = prevPos.y + hitVector.y;
			MovementTween = Body.DOMove(new Vector2(xMovement, yMovement), BounceImpactDuration, false).SetEase(BounceImpactEase);

			if (OnBeginBounceImpact != null)
				OnBeginBounceImpact();

			yield return new WaitForSeconds(BounceImpactDuration);

			if (OnEndHit != null) 
				OnEndHit();

			Fall();
		}


		IEnumerator IEJuggle(Vector2 hitVector) {
			State = EnemyState.JUGGLE;

			Vector3 prevPos = Body.transform.position;
			float xMovement = prevPos.x + hitVector.x;
			float yMovement = prevPos.y + hitVector.y;
			MovementTween = Body.DOMove(new Vector2(xMovement, yMovement), JuggleDuration, false).SetEase(JuggleEase);

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

		IEnumerator IESlamRoutine() {
			State = EnemyState.SLAM;
			yield return null;

//			MovementTween = null;
			float distance = transform.position.y - GroundPos.y;
			float slamTime = distance / SlamSpeed;
			Vector2 targetPos = new Vector2(
				GroundPos.x,
				GroundPos.y + (Body.position.y - FootPosition.position.y)
			);
			if (!IsFallGrounded) {
				MovementTween = Body.DOMove(targetPos, slamTime, false).SetEase(SlamEase);
			}

			if (OnSlam != null) 
				OnSlam();
		}

		#endregion

		void UpdateImmediateGrounded() {
			if (State == EnemyState.FALL || State == EnemyState.SLAM) {
				if (IsFallGrounded) {
					ImmediateGrounded();
					Bounce();
				}
			}
		}

		// langsung pok
		void ImmediateGrounded() {
			MovementTween = null;
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(FootPosition.position, new Vector2(0, -1), FallGroundRayDistance, Layer);
			for (int i = 0; i < groundRays.Length; i++) {
				Collider2D coll = groundRays[i].collider;
				if (coll != null) {					
					// not self
					if (!SelfCollider.Contains(coll)){
						transform.position = new Vector2(this.transform.position.x, groundRays[i].point.y + FallGroundRayDistance + (FootPosition.position.y - this.transform.position.y));
					}
				}
			}
		}

		protected override void UpdateSpeed ()
		{
			if (!IsGrounded) {
				Body.gravityScale = 0;

				if (State != EnemyState.JUGGLE && State != EnemyState.HIT_AIR) {

					if (State == EnemyState.SLAM) {
//						Body.velocity = new Vector2(0, -SlamSpeed);
					}else{
						Body.velocity = new Vector2(0, -FallingSpeed);
					}

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