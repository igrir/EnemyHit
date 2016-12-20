using UnityEngine;
using System.Collections;
using EnemyHitTest;
using DG.Tweening;

public class CameraShakeJelek : MonoBehaviour {

	EnemyHitter _EnemyHitter;
	[SerializeField] Camera TargetCamera;

	public float Duration;
	public float Strength;
	public int Vibrato;
	public float Randomness;

	bool IsShake = true;

	// Use this for initialization
	void Start () {
		_EnemyHitter = GetComponent<EnemyHitter>();

		_EnemyHitter.OnHit += Handle_OnHit;
		_EnemyHitter.OnAerialAttack += Handle_OnAerialAttack;
		_EnemyHitter.OnJuggle += Handle_OnJuggle;
	}

	public void SetShake(bool isShake) {
		Debug.Log(isShake);
		IsShake = isShake;
	}

	public void Shake(){
		if (IsShake)
			TargetCamera.DOShakePosition(Duration, Strength, Vibrato, Randomness);
	}

	void Handle_OnJuggle ()
	{
		Shake();
	}

	void Handle_OnAerialAttack ()
	{
		Shake();
	}

	void Handle_OnHit ()
	{
		Shake();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
