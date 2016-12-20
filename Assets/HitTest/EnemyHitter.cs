using UnityEngine;
using System.Collections;

namespace EnemyHitTest{
	public class EnemyHitter : MonoBehaviour {

		public Vector2 HitVector = new Vector2(0.5f, 0);
		public Vector2 JuggleVector  = new Vector2(0, 1.5f);
		public Vector2 AerialAttackVector = new Vector2(0.5f, -1.5f);

		Enemy[] Enemies;

		public delegate void _OnHit();
		public _OnHit OnHit;

		public delegate void _OnJuggle();
		public _OnJuggle OnJuggle;

		public delegate void _OnAerialAttack();
		public _OnAerialAttack OnAerialAttack;

		public delegate void _OnSlam();
		public _OnSlam OnSlam;

		// Use this for initialization
		void Start () {
			Enemies = GameObject.FindObjectsOfType<Enemy>();
		}
		
		// Update is called once per frame
		void Update () {

			if (Input.GetKeyDown(KeyCode.X)) {
				HitRight();
			}

			if (Input.GetKeyDown(KeyCode.Z)) {
				HitLeft();
			}

			if (Input.GetKeyDown(KeyCode.A)) {
				Juggle();
			}

			if (Input.GetKeyDown(KeyCode.C)) {
				AerialAttackLeft();
			}

			if (Input.GetKeyDown(KeyCode.V)) {
				AerialAttackRight();
			}

			if (Input.GetKeyDown(KeyCode.S)) {
				Slam();
			}



		}

		public void HitRight() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( new Vector2(-HitVector.x, HitVector.y) );
			}

			if (OnHit != null)
				OnHit();
		}

		public void HitLeft() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( new Vector2(HitVector.x, HitVector.y) );
			}

			if (OnHit != null)
				OnHit();
		}

		public void Juggle() {
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Juggle( JuggleVector );
			}

			if (OnJuggle != null)
				OnJuggle();
		}

		public void AerialAttackRight() {
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( new Vector2(-AerialAttackVector.x, AerialAttackVector.y) );
			}

			if (OnAerialAttack != null)
				OnAerialAttack();
		}

		public void AerialAttackLeft() {
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( new Vector2(AerialAttackVector.x, AerialAttackVector.y) );
			}

			if (OnAerialAttack != null)
				OnAerialAttack();
		}

		public void Slam() {

			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Slam();
			}

			if (OnSlam != null)
				OnSlam();
		}
	}
}
