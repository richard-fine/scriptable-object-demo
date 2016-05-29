using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName="Brains/Random walker")]
public class RandomWalkingTank : TankBrain
{
	[MinMaxRange(0, 10)]
	public RangedFloat idleTime;
	[MinMaxRange(0, 10)]
	public RangedFloat moveTime;
	[MinMaxRange(0, 10)]
	public RangedFloat fireTime;

	public enum States
	{
		Idle,
		Moving,
		Firing
	}

	public override void Initialize(TankThinker tank)
	{
		tank.Remember("state", States.Idle);
		tank.Remember("stateTimeout", Random.Range(idleTime.minValue, idleTime.maxValue));
	}

	public override void Think(TankThinker tank)
	{
		float stateTimeout = tank.Remember<float>("stateTimeout");
		stateTimeout -= Time.deltaTime;
		tank.Remember("stateTimeout", stateTimeout);

		var state = tank.Remember<States>("state");

		var move = tank.GetComponent<TankMovement>();
		if (state == States.Moving)
		{
			move.Steer(tank.Remember<float>("moveForwardBack"), tank.Remember<float>("moveLeftRight"));
		}
		else
		{
			move.Steer(0, 0);
		}

		if (stateTimeout < 0)
		{
			switch (state)
			{
				case States.Idle:
				{
					tank.Remember("state", States.Moving);
					tank.Remember("stateTimeout", Random.Range(moveTime.minValue, moveTime.maxValue));
					tank.Remember("moveForwardBack", Random.Range(-1f, 1f));
					tank.Remember("moveLeftRight", Random.Range(-1f, 1f));
					break;
				}
				case States.Moving:
				{
					tank.Remember("state", States.Firing);
					tank.Remember("stateTimeout", Random.Range(fireTime.minValue, fireTime.maxValue));

					var tankFire = tank.GetComponent<TankShooting>();
					tankFire.BeginChargingShot();

					break;
				}
				case States.Firing:
				{
					var tankFire = tank.GetComponent<TankShooting>();
					tankFire.FireChargedShot();

					tank.Remember("state", States.Idle);
					tank.Remember("stateTimeout", Random.Range(idleTime.minValue, idleTime.maxValue));

					break;
				}
			}
		}
	}
}
