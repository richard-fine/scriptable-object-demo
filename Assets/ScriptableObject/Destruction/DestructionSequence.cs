using System.Collections;
using UnityEngine;

public abstract class DestructionSequence : ScriptableObject
{
	public abstract IEnumerator SequenceCoroutine(MonoBehaviour runner);
}
