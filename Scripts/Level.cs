using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Array = Godot.Collections.Array;

public class Level : Node
{
	private PackedScene _enemyScene;
	private PackedScene _cardTableScene;
	private PackedScene _slotMachineScene;

	private Array _propSpawns;
	
	private Random _random;

	private TileMap _tileMap;
	private Player _player;
	private AudioStreamPlayer _tileAudioPlayer;
	private DiceTimer _diceTimer;

	private List<CardTable> _cardTables;
	private List<Node2D> _slotMachines;
	private Label _youWinMessage;

	private Dictionary<Tile, AudioStream> _footstepSounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();

		_tileMap = GetNode<TileMap>("Map");
		_player = GetNode<Player>("Player");
		_tileAudioPlayer = GetNode<AudioStreamPlayer>("TileAudioPlayer");
		_diceTimer = GetNode<DiceTimer>("DiceTimer");
		_youWinMessage = GetNode<Label>("YouWinMessage");
		_propSpawns = GetTree().GetNodesInGroup("propSpawns");
		_cardTables = new List<CardTable>();
		_slotMachines = new List<Node2D>();

		_enemyScene = GD.Load<PackedScene>("res://Scenes/Characters/Bouncer.tscn");
		_cardTableScene = GD.Load<PackedScene>("res://Scenes/CardTable.tscn");
		_slotMachineScene = GD.Load<PackedScene>("res://Scenes/Props/SlotMachine.tscn");

		_footstepSounds = new Dictionary<Tile, AudioStream>();
		AddFootstepSound(Tile.Stone,   "footstep_tile_1");
		AddFootstepSound(Tile.Tile,    "footstep_tile_1");
		AddFootstepSound(Tile.Carpet1, "footstep_carpet");
		AddFootstepSound(Tile.Carpet2, "footstep_carpet");
		AddFootstepSound(Tile.Wood,    "footstep_wood_1");

		GenerateLevel();

		_diceTimer.MakeVisibleAndStart();
		_diceTimer.Connect("TimerFinished", this, nameof(OnDiceTimerFinished)); 
	}

	private void GenerateLevel()
	{
		GenerateTiles();
		SpawnPlayer();
		ClearCardTables();
		GenerateCardTables();
		ClearSlotMachines();
		GenerateSlotMachines();
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
			if (_player.AdjacentToObjective)
			{
				_youWinMessage.Visible = true;
				GetTree().Paused = true;
			} 
			else
			{
				GenerateLevel();
			}
		}
	}

	public void GenerateTiles() 
	{
		// Base layer
		for (var x = 1; x < 31; x++)
		{
			for (var y = 1; y < 24; y++)
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
				if (x == 0 || x == 31 || y == 0 || y == 24) continue;
				tm.SetCell(x, y, tile);
			}
		}
	}

	private static void ClearNodes<T>(List<T> nodes)
		where T : Node2D
	{
		foreach (var node in nodes)
		{
			node.QueueFree();
		}
		nodes.Clear();
	}

	private void ClearCardTables()
	{
		ClearNodes(_cardTables);
	}

	private void GenerateCardTables()
	{
		for (var i = 0; i < 3; i++)
		{
			AddCardTable(i == 1);
		}
	}

	private void AddCardTable(bool objective)
	{
		var cardTable = _cardTableScene.Instance<CardTable>();
		_cardTables.Add(cardTable);
		AddChild(cardTable);
		if (!objective)
		{
			cardTable.RemoveDealer();
		}
		cardTable.Position = RandomPropSpawn();
	}

	private void ClearSlotMachines()
	{
		ClearNodes(_slotMachines);
	}

	private void GenerateSlotMachines()
	{
		for (var i = 0; i < 1; i++)
		{
			var slotMachine = _slotMachineScene.Instance<Node2D>();
			_slotMachines.Add(slotMachine);
			AddChild(slotMachine);
			MoveChild(slotMachine, 1);
			slotMachine.Position = RandomPropSpawn();
		}
	}

	private Vector2 RandomPropSpawn()
	{
		var idx = _random.Next(0, _propSpawns.Count);
		var spawnPoint = (Position2D)_propSpawns[idx];
		_propSpawns.Remove(idx);
		return spawnPoint.Position;
	}

	private void SpawnPlayer()
	{
		_player.SetProtagonist(Player.RandomProtagonistOption(_random));
		_player.Visible = true;
		_player.Position = RandomSpawnPosition();
	}

	private Vector2 RandomSpawnPosition()
	{
		var spawnTile = new Vector2(_random.Next(1, 30), _random.Next(1, 22)) * 32;
		var spawnPos = spawnTile + new Vector2(16, 16);
		return spawnPos;
	}

	private void OnDiceTimerFinished()
	{
		_player.Visible = false;
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		for (var i = 0; i < 3; i++)
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
				_tileAudioPlayer.Play();
			}
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

