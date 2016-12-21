﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;

namespace EnemyHitTest{
	public abstract class Enemy : MonoBehaviour {

		public LayerMask Layer;
		public Vector2 RBVelocity;
		public Transform Tut;

		[SerializeField] protected Transform FootPosition;
		protected Vector2 GroundPos;

		protected EnemyState State;

		public float GroundRayDistance = 1.8f;
		public float FallGroundRayDistance = 2.3f;

		public float FallingSpeed = 0.2f;

		public Ease SlamEase;
		public float SlamSpeed = 5f;

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

		public delegate void _OnBeginHitAir();
		public _OnBeginHitAir OnBeginHitAir;

		public delegate void _OnBeginBounceImpact();
		public _OnBeginBounceImpact OnBeginBounceImpact;

		public delegate void _OnBounce();
		public _OnBounce OnBounce;

		public delegate void _OnSlam();
		public _OnSlam OnSlam;


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
			RBVelocity = Body.velocity;
		}

		protected virtual void FixedUpdate() {
			UpdateSpeed();
			CheckGrounded();
			CheckFallGrounded();

			float vy = Mathf.Infinity;
			RaycastHit2D groundRay = Physics2D.Raycast(this.FootPosition.position, new Vector2(0, -1), vy, Layer);

			Vector2 rayPos = groundRay.point;
			GroundPos = rayPos;
			Tut.transform.position = rayPos;

		}

		void CheckGrounded() {
			_IsGrounded = false;
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(this.FootPosition.position, new Vector2(0, -1), GroundRayDistance, Layer);
			Debug.DrawRay(this.FootPosition.position, new Vector2(0, -1) * GroundRayDistance, new Color(1,0,0));
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
			RaycastHit2D[] groundRays = Physics2D.RaycastAll(this.FootPosition.position, new Vector2(0, -1), FallGroundRayDistance, Layer);
			Debug.DrawRay(new Vector2(this.FootPosition.position.x+0.5f, this.FootPosition.position.y), new Vector2(0, -1) * FallGroundRayDistance, new Color(1,0,0));
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
		public abstract void Slam();

	}
}