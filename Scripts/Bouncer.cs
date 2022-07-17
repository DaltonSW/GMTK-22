using Godot;
using System;

public class Bouncer : KinematicBody2D
{
	private Timer _timer;

	//[Export] private float _rotateInterval = 4f;

	[Export] private float _speed = 80f;

	private readonly Vector2[] _dirs = { Vector2.Left, Vector2.Down, Vector2.Right, Vector2.Up };
	
	private int _dirIndex;

	private Vector2 _curDir;

	private AnimatedSprite _sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_timer = GetNode<Timer>("Timer");
		//_timer.WaitTime = _rotateInterval;
		_timer.Start();
		_timer.Connect("timeout", this, nameof(OnTimerTick));
		_curDir = _dirs[_dirIndex];
	}
	public void OnTimerTick()
	{
		_dirIndex = (_dirIndex + 1) % 4;
		_curDir = _dirs[_dirIndex];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		_sprite.Animation = MovementAnimationUtils.NextMovementAnimation(_curDir, _sprite);
		MoveAndSlide(_curDir * _speed);
	}

	[Signal]
	delegate void player_collided();
}
