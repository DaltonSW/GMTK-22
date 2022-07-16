using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
	[Export]
	private int _speed = 100;

	private Vector2 _velocity = new Vector2();
	public Position2D CurPos;

	// Keeps track of which objects the player collided with last time we checked collisions.
	// By default in Godot, when the player collides with an object,
	// collisions are detected repeatedly until the player moves away.
	// Keeping a list gives us a way to check whether a collision is with a "new" object,
	// or the player just stayed next to the same thing for a while.
	private List<CollisionObject2D> _objectsCollidedWithLastCheck;

	public override void _Ready()
	{
		CurPos = GetNode<Position2D>("Position");
		_objectsCollidedWithLastCheck = new List<CollisionObject2D>();
	}

	public Vector2 GetInputVelocity()
	{
		var velocity = new Vector2();
		if (Input.IsActionPressed("right"))
		{
			velocity.x += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			velocity.x -= 1;
		}
		if (Input.IsActionPressed("down"))
		{
			velocity.y += 1;
		}
		if (Input.IsActionPressed("up"))
		{
			velocity.y -= 1;
		}
		return velocity.Normalized() * _speed;
	}

	public override void _PhysicsProcess(float delta)
	{
		_velocity = GetInputVelocity();
		_velocity = MoveAndSlide(_velocity);

		var collidedWithThisCheck = new List<CollisionObject2D>();
		int numCollisions = GetSlideCount();
		for (int i = 0; i < numCollisions; i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.Collider as CollisionObject2D;
			if (collider != null)
			{
				collidedWithThisCheck.Add(collider);
				if (!_objectsCollidedWithLastCheck.Contains(collider)
					&& collider.IsInGroup("noise_prop_collision_box"))
				{
					collider.EmitSignal("player_collided");
				}
			}
		}
		_objectsCollidedWithLastCheck = collidedWithThisCheck;
	}

	public bool IsMoving() => !_velocity.IsEqualApprox(new Vector2(0, 0));
}
