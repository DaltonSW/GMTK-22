using Godot;
using System;
using System.Collections.Generic;

public enum ProtagonistOption
{
	MaleProtagonist1,
	MaleProtagonist2,
	FemaleProtagonist1,
	FemaleProtagonist2,
}

public static class ProtagonistOptionMethods
{
	public static string SceneName(this ProtagonistOption po)
	{
		switch (po)
		{
			case ProtagonistOption.MaleProtagonist1: return "MaleProtagonist1";
			case ProtagonistOption.MaleProtagonist2: return "MaleProtagonist2";
			case ProtagonistOption.FemaleProtagonist1: return "FemaleProtagonist1";
			case ProtagonistOption.FemaleProtagonist2: return "FemaleProtagonist2";
			default: throw new NotImplementedException();
		}
	}
}

public class Player : KinematicBody2D
{
	[Export]
	private int _speed = 100;

	private Vector2 _velocity;

	private bool _adjacentToObjective;

	private AnimatedSprite _protagSprite;

	public bool AdjacentToObjective { 
		get { return _adjacentToObjective; }
	}

	public static ProtagonistOption RandomProtagonistOption(Random rng)
	{
		var numOptions = Enum.GetValues(typeof(ProtagonistOption)).Length;
		var i = rng.Next(numOptions);
		return (ProtagonistOption) i;
	}

	public void SetProtagonist(ProtagonistOption po)
	{
		if (_protagSprite != null)
		{
			_protagSprite.QueueFree();
		}
		var protagScene = ResourceLoader.Load<PackedScene>($"res://Scenes/Characters/{po.SceneName()}.tscn");
		_protagSprite = protagScene.Instance<AnimatedSprite>();
		_protagSprite.Position = new Vector2(0, -8);
		AddChild(_protagSprite);
	}

	// Keeps track of which objects the player collided with last time we checked collisions.
	// By default in Godot, when the player collides with an object,
	// collisions are detected repeatedly until the player moves away.
	// Keeping a list gives us a way to check whether a collision is with a "new" object,
	// or the player just stayed next to the same thing for a while.
	private List<CollisionObject2D> _objectsCollidedWithLastCheck;

	public override void _Ready()
	{
		_adjacentToObjective = false;
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

	public override void _Process(float delta)
	{
		_protagSprite.Animation = MovementAnimationUtils.NextMovementAnimation(_velocity, _protagSprite);
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

	private void _on_AdjacentArea_body_entered(Node body)
	{
		if (body.IsInGroup("objective"))
		{
			_adjacentToObjective = true;
		}
	}
	private void _on_AdjacentArea_body_exited(Node body)
	{
		if (body.IsInGroup("objective"))
		{
			_adjacentToObjective = false;
		}
	}
}

public static class MovementAnimationUtils
{
	public static string NextMovementAnimation(Vector2 velocity, AnimatedSprite sprite)
	{
		if (velocity.y > 0)
		{
			return "Walk Down";
		}
		else if (velocity.y < 0)
		{
			return "Walk Up";
		}
		else if (velocity.x < 0)
		{
			return "Walk Left";
		}
		else if (velocity.x > 0)
		{
			return "Walk Right";
		}
		else
		{
			switch (sprite.Animation)
			{
				case "Walk Down":  return "Idle Down";
				case "Walk Up":    return "Idle Up";
				case "Walk Left":  return "Idle Left";
				case "Walk Right": return "Idle Right";
				default: return "Idle Down";
			}
		}
	}
} 
