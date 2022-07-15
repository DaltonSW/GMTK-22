using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export]
	private int _speed = 100;

	private Vector2 _velocity = new Vector2();
	public Position2D CurPos;

	public override void _Ready()
	{
		CurPos = GetNode<Position2D>("Position");
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
	}

}
