using Godot;
using System;

public class NoiseProp : Node2D
{
	[Export]
	public AudioStream ProximityAudioStream;

	[Export]
	public AudioStream CollisionAudioStream;

	private AudioStreamPlayer _proximityAudioPlayer;
	private AudioStreamPlayer _collisionAudioPlayer;

	private StaticBody2D _collisionBox;

	public override void _Ready()
	{
		_proximityAudioPlayer = GetNode<AudioStreamPlayer>("ProximityAudioPlayer");
		_proximityAudioPlayer.Stream = ProximityAudioStream;
		
		_collisionAudioPlayer = GetNode<AudioStreamPlayer>("CollisionAudioPlayer");
		_collisionAudioPlayer.Stream = CollisionAudioStream;

		_collisionBox = GetNode<StaticBody2D>("CollisionBox");
		_collisionBox.AddUserSignal("player_collided");
		_collisionBox.Connect("player_collided", this, nameof(OnPlayerCollided));
	}

	public void _on_NoiseArea_body_entered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			GD.Print("Player entered noise area");
			_proximityAudioPlayer.Play();
		}
	}
	
	private void _on_NoiseArea_body_exited(Node body)
	{
		if (body.IsInGroup("player"))
		{
			GD.Print("Player left noise area");
			_proximityAudioPlayer.Stop();
		}
	}

	public void OnPlayerCollided()
	{
		_collisionAudioPlayer.Play();
	}

}



