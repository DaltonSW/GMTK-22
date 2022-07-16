using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Level : Node
{
	private PackedScene _enemyScene;
	
	private Random _random;

	private TileMap _tileMap;
	private Player _player;
	private AudioStreamPlayer _tileAudioPlayer;
	private DiceTimer _diceTimer;

	private Dictionary<Tile, AudioStream> _footstepSounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();

		_tileMap = GetNode<TileMap>("Map");
		_player = GetNode<Player>("Player");
		_tileAudioPlayer = GetNode<AudioStreamPlayer>("TileAudioPlayer");
		_diceTimer = GetNode<DiceTimer>("DiceTimer");

		_enemyScene = GD.Load<PackedScene>("res://Scenes/Bouncer.tscn");

		_footstepSounds = new Dictionary<Tile, AudioStream>();
		AddFootstepSound(Tile.Stone,     "footstep_tile");
		AddFootstepSound(Tile.Tile,     "footstep_tile");
		AddFootstepSound(Tile.Carpet1,     "footstep_carpet");
		AddFootstepSound(Tile.Carpet2,     "footstep_carpet");
		AddFootstepSound(Tile.Wood, "footstep_wood_1");

		GenerateTiles();
		SpawnPlayer();
		_diceTimer.MakeVisibleAndStart();
		_diceTimer.Connect("TimerFinished", this, nameof(SpawnEnemies));
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
			SpawnPlayer();
		}
	}

	public void GenerateTiles() 
	{
		// Base layer
		for (var x = 1; x < 31; x++)
		{
			for (var y = 1; y < 23; y++)
			{
				_tileMap.SetCell(x, y, (int) Tile.Tile); 
			}
		}

		for (var i = 0; i < 10; i++)
		{
			AddRectangle(_tileMap, 10,(int) Tile.Carpet1);
		}
		
		for (var i = 0; i < 11; i++)
		{
			AddRectangle(_tileMap, 9, (int) Tile.Carpet2);
		}
		
		for (var i = 0; i < 12; i++)
		{
			AddRectangle(_tileMap, 8, (int) Tile.Wood);
		}

		for (var i = 0; i < 13; i++)
		{
			AddRectangle(_tileMap, 7, (int) Tile.Stone);
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

	private void SpawnPlayer()
	{
		var spawnTile = new Vector2(_random.Next(0, 31), _random.Next(0, 23)) * 32;
		var spawnPos = spawnTile + new Vector2(16, 16);
		_player.Position = spawnPos;
	}

	private void SpawnEnemies()
	{
		for (int i = 0; i < 3; i++)
		{
			var bouncer = (Bouncer)_enemyScene.Instance();
			bouncer.Position = GetRandomTile() * 32;
			AddChild(bouncer);
		}
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
			}
			_tileAudioPlayer.Play();
		}
	}

}

public enum Tile
{
	Tile,
	Carpet1,
	Carpet2,
	Wood,
	Stone
}

