using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Level : Node
{
	private Random _random;

	private TileMap _tileMap;
	private Player _player;
	private AudioStreamPlayer _tileAudioPlayer;

	private Dictionary<Tile, AudioStream> _footstepSounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();

		_tileMap = GetNode<TileMap>("Map");
		_player = GetNode<Player>("Player");
		_tileAudioPlayer = GetNode<AudioStreamPlayer>("TileAudioPlayer");

		_footstepSounds = new Dictionary<Tile, AudioStream>();
		AddFootstepSound(Tile.Stone,     "footstep_tile");
		AddFootstepSound(Tile.Grass,     "footstep_carpet");
		AddFootstepSound(Tile.Sandstone, "footstep_wood_1");

		GenerateTiles();
	}

	private void AddFootstepSound(Tile tile, string wavFileName)
	{
		var sound = ResourceLoader.Load<AudioStreamSample>($"res://Assets/Sounds/{wavFileName}.wav");
		_footstepSounds.Add(tile, sound);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_select"))
		{
			Tile currentTile = PlayerCurrentTile();
			_footstepSounds.TryGetValue(currentTile, out AudioStream footstepSound);
			if (footstepSound != null)
			{
				_tileAudioPlayer.Stream = footstepSound;
				_tileAudioPlayer.Play();
			}
		}
	}

	public void GenerateTiles()
	{
		for (var i = 0; i < 16; i++)
		{
			for (var j = 0; j < 12; j++)
			{
				_tileMap.SetCell(i, j, _random.Next(0, 5));
			}
		}
	}

	public void SetRandomTile()
	{
		
	}

	public Tile PlayerCurrentTile()
	{
		var curTileCoords = _tileMap.WorldToMap(_player.Position);
		var tileI = _tileMap.GetCellv(curTileCoords);
		var tile = (Tile)tileI;
		GD.Print($"{_player.Position} - {tile}");
		return tile;
	}

}

public enum Tile
{
	Stone,
	Sand,
	Grass,
	Dirt,
	Sandstone
}
