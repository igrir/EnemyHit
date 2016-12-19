using UnityEngine;
using System.Collections;

namespace EnemyHitTest{
	public class EnemyHitter : MonoBehaviour {

		public Vector2 HitRightVector = new Vector2(-0.5f, 0);
		public Vector2 HitLeftVector = new Vector2(0.5f, 0);
		public Vector2 JuggleVector  = new Vector2(0, 1.5f);

		Enemy[] Enemies;

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



		}

		public void HitRight() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( HitRightVector );
			}
		}

		public void HitLeft() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( HitLeftVector );
			}
		}

		public void Juggle() {
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Juggle( JuggleVector );
			}
		}
	}
}
