using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu(menuName="Brains/Simple sniper")]
public class SimpleSniper : TankBrain
{

	public float aimAngleThreshold = 2f;
	[MinMaxRange(0, 0.05f)]
	public RangedFloat chargeTimePerDistance;
	[MinMaxRange(0, 10)]
	public RangedFloat timeBetweenShots;

	public override void Think(TankThinker tank)
	{
		GameObject target = tank.Remember<GameObject>("target");
		var movement = tank.GetComponent<TankMovement>();

		if (!target)
		{
			// Find the nearest tank that isn't me
			target =
				GameObject
					.FindGameObjectsWithTag("Player")
					.OrderBy(go => Vector3.Distance(go.transform.position, tank.transform.position))
					.FirstOrDefault(go => go != tank.gameObject);

			tank.Remember<GameObject>("target");
		}

		if (!target)
		{
			// No targets left - drive in a victory circles
			movement.Steer(0.5f, 1f);
			return;
		}

		// aim at the target
		Vector3 desiredForward = (target.transform.position - tank.transform.position).normalized;
		if (Vector3.Angle(desiredForward, tank.transform.forward) > aimAngleThreshold)
		{
			bool clockwise = Vector3.Cross(desiredForward, tank.transform.forward).y > 0;
			movement.Steer(0f, clockwise ? -1 : 1);
		}
		else
		{
			// Stop
			movement.Steer(0f, 0f);
		}

		// Fire at the target
		var shooting = tank.GetComponent<TankShooting>();
		if (!shooting.IsCharging)
		{
			if (Time.time > tank.Remember<float>("nextShotAllowedAfter"))
			{
				float distanceToTarget = Vector3.Distance(target.transform.position, tank.transform.position);
				float timeToCharge = distanceToTarget*Random.Range(chargeTimePerDistance.minValue, chargeTimePerDistance.maxValue);
				tank.Remember("fireAt", Time.time + timeToCharge);
				shooting.BeginChargingShot();
			}
		}
		else
		{
			float fireAt = tank.Remember<float>("fireAt");
			if (Time.time > fireAt)
			{
				shooting.FireChargedShot();
				tank.Remember("nextShotAllowedAfter", Time.time + Random.Range(timeBetweenShots.minValue, timeBetweenShots.maxValue));
			}
		}
	}
}
