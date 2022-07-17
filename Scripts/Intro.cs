using Godot;
using System;
using System.Collections.Generic;

public class Intro : Node2D
{
	private Player _player;

	private AudioStreamPlayer _tileAudioPlayer;
	private TileMap _tileMap;
	private Dictionary<Tile, AudioStream> _footstepSounds;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var rng = new Random();
		_player = GetNode<Player>("Player");
		_player.SetProtagonist(Player.RandomProtagonistOption(rng));
		_player.Visible = true;

		_tileAudioPlayer = GetNode<AudioStreamPlayer>("TileAudioPlayer");
		_tileMap = GetNode<TileMap>("Map");

		_footstepSounds = new Dictionary<Tile, AudioStream>();
		AddFootstepSound(Tile.Stone,   "footstep_tile_1");
		AddFootstepSound(Tile.Tile,    "footstep_tile_1");
		AddFootstepSound(Tile.Carpet1, "footstep_carpet");
		AddFootstepSound(Tile.Carpet2, "footstep_carpet");
		AddFootstepSound(Tile.Wood,    "footstep_wood_1");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (!Input.IsActionJustPressed("ui_select")) return;
		if (_player.AdjacentToObjective)
		{
			_player.Visible = false;
			GetTree().ChangeScene("res://Scenes/Scene.tscn");
		}   
	}
	public void _on_FootstepClock_timeout()
	{
		_tileAudioPlayer.Stop();
		if (_player.IsMoving())
		{
			var currentTile = PlayerCurrentTile();
			GD.Print($"Playing footstep sound for: {currentTile}");
			_footstepSounds.TryGetValue(currentTile, out AudioStream footstepSound);
			if (footstepSound != null)
			{
				_tileAudioPlayer.Stream = footstepSound;
				_tileAudioPlayer.Play();
			}
		}
	}

	private void AddFootstepSound(Tile tile, string wavFileName)
	{
		var sound = ResourceLoader.Load<AudioStreamSample>($"res://Assets/Sounds/{wavFileName}.wav");
		_footstepSounds.Add(tile, sound);
	}

	private Tile PlayerCurrentTile()
	{
		var curTileCoords = _tileMap.WorldToMap(_player.Position);
		var tileI = _tileMap.GetCellv(curTileCoords);
		var tile = (Tile)tileI;
		GD.Print($"{_player.Position} - {tile}");
		return tile;
	}


}
