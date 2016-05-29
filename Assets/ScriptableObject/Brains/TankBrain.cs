using UnityEngine;

public abstract class TankBrain : ScriptableObject
{
	public virtual void Initialize(TankThinker tank) { }
	public abstract void Think(TankThinker tank);
}
