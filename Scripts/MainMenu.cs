using Godot;
using System;

public class MainMenu : Control
{
	private AudioStreamPlayer _audioPlayer;
	private AudioStreamSample _menuSong;

	public override void _Ready()
	{
		_menuSong = GD.Load<AudioStreamSample>("res://Assets/Music/TitleScreenGJ.wav");
		_audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		_audioPlayer.Autoplay = true;
		_audioPlayer.Stream = _menuSong;
		_audioPlayer.Play();
	}

	public void _on_StartButton_pressed()
	{ 
		GetTree().ChangeScene("res://Scenes/Intro.tscn");
	}

	public void _on_QuitButton_pressed()
	{
		GetTree().Quit();
	}

	public void _on_CreditsButton_pressed()
	{
		GetTree().ChangeScene("res://Scenes/Credits.tscn");
	}
}
