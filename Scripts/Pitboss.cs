using Godot;
using System;

public class Pitboss : KinematicBody2D
{
	private Timer _phoneTimer;
	private Timer _alertTimer;

	private Area2D _fieldOfView;

	private AnimatedSprite _sprite;

	private bool _alert;

	[Signal]
	public delegate void DetectedPlayer();

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_phoneTimer = GetNode<Timer>("PhoneTimer");
		_alertTimer = GetNode<Timer>("AlertTimer");
		_fieldOfView = GetNode<Area2D>("FieldOfView");

		OnAlertTimerTimeout();
	}

	public void OnPhoneTimerTimeout()
	{
		_alert = true;
		_sprite.Play("Idle Down");
		_alertTimer.Start();
	}

	public void OnAlertTimerTimeout()
	{
		_alert = false;
		_sprite.Play("Check Phone");
		_phoneTimer.Start();
	}

	public void OnFieldOfViewBodyEntered(Node body)
	{
		if (_alert && body.IsInGroup("player"))
		{
			EmitSignal(nameof(DetectedPlayer));
		}
	}

	private static int positiveMod(int dividend, int mod) => ((dividend % mod) + mod) % mod;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
