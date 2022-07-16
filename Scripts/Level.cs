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
		AddFootstepSound(Tile.Tile,     "footstep_tile");
		AddFootstepSound(Tile.Carpet1,     "footstep_carpet");
		AddFootstepSound(Tile.Carpet2,     "footstep_carpet");
		AddFootstepSound(Tile.Wood, "footstep_wood_1");

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
			// Tile currentTile = PlayerCurrentTile();
			// _footstepSounds.TryGetValue(currentTile, out AudioStream footstepSound);
			// if (footstepSound != null)
			// {
			// 	_tileAudioPlayer.Stream = footstepSound;
			// 	_tileAudioPlayer.Play();
			// }
			GenerateTiles();
		}
	}

	//TODO: Replace the hardcoded tile values with enums when we settle on tiles and ordering
	public void GenerateTiles() 
	{
		// Base layer
		for (var x = 0; x < 32; x++)
		{
			for (var y = 0; y < 24; y++)
			{
				_tileMap.SetCell(x, y, 0); 
			}
		}

		for (var i = 0; i < 10; i++)
		{
			AddRectangle(_tileMap, 10, 1);
		}
		
		for (var i = 0; i < 11; i++)
		{
			AddRectangle(_tileMap, 9, 2);
		}
		
		for (var i = 0; i < 12; i++)
		{
			AddRectangle(_tileMap, 8, 3);
		}

		for (var i = 0; i < 13; i++)
		{
			AddRectangle(_tileMap, 7, 4);
		}


	}

	private void AddRectangle(TileMap tm, int maxVariance, int tile)
	{
		var topLeft = GetRandomTile();
		var width = _random.Next(2, maxVariance);
		var height = _random.Next(2, maxVariance);

		for (var x = (int)topLeft.x; x < topLeft.x + width; x++)
		{
			for (var y = (int)topLeft.y; y < topLeft.y + height; y++)
			{
				tm.SetCell(x, y, tile);
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

	private Vector2 GetRandomTile()
	{
		return new Vector2(_random.Next(0, 30), _random.Next(0, 20));
	}

}

public enum Tile
{
	Carpet1,
	Carpet2,
	Stone,
	Tile,
	Wood
}

// Small Tiles
// Carpet 1
// Carpet 2
// Stone Tiles
// Wood
