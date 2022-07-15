using Godot;
using System;

public class ProximityNoiseProp : Node2D
{
	[Export]
	public AudioStream AudioStream;

	private AudioStreamPlayer _audioPlayer;

	public override void _Ready()
	{
		_audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_audioPlayer.Stream = AudioStream;
	}

	public void _on_NoiseArea_body_entered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			GD.Print("Player entered noise area");
			_audioPlayer.Play();
		}
	}
	
	private void _on_NoiseArea_body_exited(Node body)
	{
		if (body.IsInGroup("player"))
		{
			GD.Print("Player left noise area");
			_audioPlayer.Stop();
		}
	}

}



