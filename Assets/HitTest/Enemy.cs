using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;

namespace EnemyHitTest{
	public abstract class Enemy : MonoBehaviour {

		protected EnemyState State;

		public float GroundRayDistance = 1.8f;
		public float FallGroundRayDistance = 2.3f;

		public float FallingSpeed = 0.2f;

		protected Rigidbody2D Body;

		bool _IsGrounded;
		protected bool IsGrounded{
			get{
				return _IsGrounded;
			}
		}

		bool _IsFallGrounded;
		protected bool IsFallGrounded{
			get{
				return _IsFallGrounded;
			}
		}


		public delegate void _OnBeginHit(Vector2 hitVector);
		public _OnBeginHit OnBeginHitGround;

		public delegate void _OnEndHit();
		public _OnEndHit OnEndHit;

		public delegate void _OnJuggle(Vector2 hitVector);
		public _OnJuggle OnJuggle;

		public delegate void _OnIdle();
		public _OnIdle OnIdle;

		public delegate void _OnWakeUp();
		public _OnWakeUp OnWakeUp;

		public delegate void _OnFall();
		public _OnFall OnFall;

		public delegate void _OnHitAir();
		public _OnHitAir OnBeginHitAir;

		public delegate void _OnBounce();
		public _OnBounce OnBounce;


		protected List<Collider2D> SelfCollider;


		// Use this for initialization
		protected virtual void Start () {
			Body = GetComponent<Rigidbody2D>();
			SelfCollider = GetComponents<Collider2D>().ToList();

			// replace gravity with constant falling speed
			Body.velocity = new Vector2(0, FallingSpeed);
		}
		
		// Update is called once per frame
		protected virtual void Update () {		
			CheckGrounded();
			CheckFallGrounded();
		}

		protected virtual void FixedUpdate() {
			UpdateSpeed();
		}

		void CheckGrounded() {
			_IsGrounded = false;
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(this.transform.position, new Vector2(0, -1), GroundRayDistance);
			Debug.DrawRay(this.transform.position, new Vector2(0, -1) * GroundRayDistance, new Color(1,0,0));
			for (int i = 0; i < groundRays.Length; i++) {
				Collider2D coll = groundRays[i].collider;
				if (coll != null) {					
					// not self
					if (!SelfCollider.Contains(coll)){
						_IsGrounded = true;
					}
				}
			}
		}

		void CheckFallGrounded() {
			_IsFallGrounded = false;
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(this.transform.position, new Vector2(0, -1), FallGroundRayDistance);
			Debug.DrawRay(new Vector2(this.transform.position.x+0.5f, this.transform.position.y), new Vector2(0, -1) * FallGroundRayDistance, new Color(1,0,0));
			for (int i = 0; i < groundRays.Length; i++) {
				Collider2D coll = groundRays[i].collider;
				if (coll != null) {					
					// not self
					if (!SelfCollider.Contains(coll)){
						_IsFallGrounded = true;
					}
				}
			}
		}

		protected virtual void UpdateSpeed() {
			if (!IsGrounded) {
				Body.gravityScale = 0;
				Body.velocity = new Vector2(0, -FallingSpeed);
			}else{
				Body.gravityScale = 1;
				Body.velocity = new Vector2(0, 0);
			}
		}

		public abstract void Hit (Vector2 hitVector);
		public abstract void Juggle (Vector2 hitVector);
		public abstract void Idle ();
		public abstract void Fall();
		public abstract void WakeUp();
		public abstract void Bounce();

	}
}