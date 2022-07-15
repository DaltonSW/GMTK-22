using Godot;
using System;

public class Level : Node
{
	private TileMap _tileMap;
	private Random _random;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();
		_tileMap = GetNode<TileMap>("Map");
		GenerateTiles();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_select"))
		{
			GenerateTiles();
		}
	}

	public void GenerateTiles()
	{
		for (var i = 0; i < 32; i++)
		{
			for (var j = 0; j < 24; j++)
			{
				_tileMap.SetCell(i, j, _random.Next(0, 5));
			}
		}
	}

	public void SetRandomTile()
	{
		
	}
}
