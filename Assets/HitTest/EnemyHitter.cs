using UnityEngine;
using System.Collections;

namespace EnemyHitTest{
	public class EnemyHitter : MonoBehaviour {

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

		void HitRight() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit( new Vector2(-0.5f, 0f)	);
			}
		}

		void HitLeft() {			
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Hit(new Vector2(0.5f, 0f));
			}
		}

		void Juggle() {
			for (int i = 0; i < Enemies.Length; i++) {
				Enemy hittable = Enemies[i];
				hittable.Juggle(new Vector2(0f, 1.5f));
			}
		}
	}
}
