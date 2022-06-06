using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class MoveToBaseState : AntAIState
{
	private const float SPEED = 2.0f;

	private Transform _t;
	private Vector3 _targetPos;
	private float _targetAngle;

	public override void Create(GameObject aGameObject)
	{
		_t = aGameObject.GetComponent<Transform>();
	}

	public override void Enter()
	{
		// Search base on the map.
		var go = GameObject.Find("Base");
		if (go != null)
		{
			_targetPos = go.transform.position;

			// Calc target angle.
			_targetAngle = AntMath.AngleDeg(_t.position, _targetPos);
			_t.rotation = Quaternion.Euler(_t.rotation.x, _t.rotation.y, _targetAngle);
			_targetAngle *= Mathf.Deg2Rad;
		}
		else
		{
			Debug.Log("Base not found!");
			Finish();
		}
	}

	public override void Execute(float aDeltaTime, float aTimeScale)
	{
		// Move to the base.
		var pos = _t.position;
		pos.x += SPEED * aDeltaTime * Mathf.Cos(_targetAngle);
		pos.y += SPEED * aDeltaTime * Mathf.Sin(_targetAngle);
		_t.position = pos;

		// Check distance to the base.
		if (AntMath.Distance(pos, _targetPos) <= 0.2f)
		{
			// We arrived!
			// Current action is finished.
			Finish();
		}
	}
}

