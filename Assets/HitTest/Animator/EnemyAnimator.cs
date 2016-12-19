using UnityEngine;
using System.Collections;

namespace EnemyHitTest{
	public class EnemyAnimator : MonoBehaviour {

		const string BOOL_HIT_GROUND 	= "hit_ground";
		const string BOOL_HIT_AIR 		= "hit_air";
		const string BOOL_JUGGLE 		= "juggle";
		const string BOOL_IDLE 			= "idle";
		const string BOOL_FALL 			= "fall";
		const string BOOL_WAKE_UP 		= "wake_up";
		const string BOOL_BOUNCE 		= "bounce";


		Enemy Parent;
		Animator Anim;

		// Use this for initialization
		void Start () {
			Parent = GetComponentInParent<Enemy>();
			Anim = GetComponent<Animator>();

			Parent.OnBeginHitAir 		+= Enemy_OnBeginHitAir;
			Parent.OnBeginHitGround 	+= Enemy_OnBeginHitGround;
			Parent.OnEndHit				+= Enemy_OnEndHit;
			Parent.OnJuggle 			+= Enemy_OnJuggle;
			Parent.OnIdle 				+= Enemy_OnIdle;
			Parent.OnFall 				+= Enemy_OnFall;
			Parent.OnWakeUp 			+= Enemy_OnWakeUp;
			Parent.OnBounce				+= Enemy_OnBounce;
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void Enemy_OnBeginHitGround(Vector2 hitVector) {
			Anim.SetBool(BOOL_HIT_GROUND, true);
			Anim.SetBool(BOOL_HIT_AIR, false);
			Anim.SetBool(BOOL_IDLE, false);
			Anim.SetBool(BOOL_JUGGLE, false);
			Anim.SetBool(BOOL_FALL, false);
		}

		void Enemy_OnBeginHitAir() {
			Anim.SetBool(BOOL_HIT_AIR, true);

			Anim.SetBool(BOOL_HIT_GROUND, false);
			Anim.SetBool(BOOL_IDLE, false);
			Anim.SetBool(BOOL_JUGGLE, false);
			Anim.SetBool(BOOL_FALL, false);
		}

		void Enemy_OnEndHit() {
			Anim.SetBool(BOOL_HIT_GROUND, false);
			Anim.SetBool(BOOL_HIT_AIR, false);
			Anim.SetBool(BOOL_IDLE, false);
		}

		void Enemy_OnJuggle(Vector2 hitVector) {
			Anim.SetBool(BOOL_JUGGLE, true);

			Anim.SetBool(BOOL_WAKE_UP, false);
			Anim.SetBool(BOOL_FALL, false);
			Anim.SetBool(BOOL_IDLE, false);
			Anim.SetBool(BOOL_HIT_GROUND, false);
			Anim.SetBool(BOOL_HIT_AIR, false);
			Anim.SetBool(BOOL_BOUNCE, false);
		}

		void Enemy_OnIdle() {
			Anim.SetBool(BOOL_IDLE, true);

			Anim.SetBool(BOOL_WAKE_UP, false);
			Anim.SetBool(BOOL_FALL, false);
			Anim.SetBool(BOOL_JUGGLE, false);
		}

		void Enemy_OnFall() {
			Anim.SetBool(BOOL_FALL, true);

			Anim.SetBool(BOOL_HIT_AIR, false);
			Anim.SetBool(BOOL_HIT_GROUND, false);
			Anim.SetBool(BOOL_WAKE_UP, false);
			Anim.SetBool(BOOL_JUGGLE, false);
		}

		void Enemy_OnWakeUp() {
			Anim.SetBool(BOOL_WAKE_UP, true);

			Anim.SetBool(BOOL_BOUNCE, false);

			Anim.SetBool(BOOL_FALL, false);
			Anim.SetBool(BOOL_JUGGLE, false);
			Anim.SetBool(BOOL_IDLE, false);
		}

		void Enemy_OnBounce() {
			Anim.SetBool(BOOL_BOUNCE, true);
			Anim.SetBool(BOOL_FALL, false);
		}

	}
}