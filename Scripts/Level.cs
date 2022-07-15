using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Level : Node
{
	private TileMap _tileMap;
	private Random _random;
	private Player _player;

	private string[] _tiles = new string[] { "Stone", "Sand", "Grass", "Dirt", "Sandstone" };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();
		_tileMap = GetNode<TileMap>("Map");
		_player = GetNode<Player>("Player");
		GenerateTiles();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_select"))
		{
			PlayerCurrentTile();
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

	public void PlayerCurrentTile()
	{
		var curTileCoords = _tileMap.WorldToMap(_player.Position);
		var tile = _tileMap.GetCellv(curTileCoords);
		GD.Print($"{_player.Position} - {_tiles[tile]}");
	}
}
