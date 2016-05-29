using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour
{
	public float Health = 100;
	public float DamagePerTankCollision = 40;

	public DestructionSequence DestructionSequence;

	public void TakeDamage(float damage)
	{
		if (Health < 0) return;

		Health -= damage;

		if (Health < 0 && DestructionSequence)
			StartCoroutine(DestructionSequence.SequenceCoroutine(this));
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			TakeDamage(DamagePerTankCollision);
		}
	}
}
