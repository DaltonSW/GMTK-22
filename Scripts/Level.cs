using Godot;
using System;
using System.Collections.Generic;
using Array = Godot.Collections.Array;

public class Level : Node
{
	private PackedScene _bouncerScene;
	private PackedScene _pitbossScene;
	private PackedScene _cardTableScene;
	private PackedScene _slotMachineScene;
	private PackedScene _diceTimerScene;

	private Array _propSpawns;
	private Array _pitbossSpawns;
	private readonly Array _usedPropSpawns = new Array();
	private readonly Array _usedPitbossSpawns = new Array();

	private int _currentLevel = 0;
	private readonly int[] _pitbossSpawnCounts =		{ 0, 0, 1, 2, 2, 3 }; 
	private readonly int[] _slotMachineSpawnCounts =	{ 1, 2, 2, 3, 3, 4 }; 
	private readonly int[] _bouncerSpawnCounts =		{ 3, 4, 4, 5, 6, 7 }; 
	private readonly int[] _cardTableSpawnCounts =		{ 4, 5, 5, 5, 6, 6 }; 

	
	private Random _random;

	private TileMap _tileMap;
	private Player _player;
	private AudioStreamPlayer _tileAudioPlayer;
	private DiceTimer _diceTimer;
	private Position2D _diceTimerPosition;

	private Sprite _success;
	private Sprite _caught;
	private Sprite _enemiesSpawning;
	private AnimatedSprite _levelDie;

	private List<CardTable> _cardTables;
	private List<Node2D> _slotMachines;
	private List<Bouncer> _bouncers;
	private List<Pitboss> _pitBosses;

	private Dictionary<Tile, AudioStream> _footstepSounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_random = new Random();

		_tileMap = GetNode<TileMap>("Map");
		_player = GetNode<Player>("Player");
		_tileAudioPlayer = GetNode<AudioStreamPlayer>("TileAudioPlayer");
		_success = GetNode<Sprite>("Success");
		_caught = GetNode<Sprite>("Caught");
		_enemiesSpawning = GetNode<Sprite>("EnemiesSpawning");
		_levelDie = GetNode<AnimatedSprite>("LevelDie");
		_diceTimerPosition = GetNode<Position2D>("DiceTimerPosition");
		
		_propSpawns = GetTree().GetNodesInGroup("propSpawns");
		_pitbossSpawns = GetTree().GetNodesInGroup("pitbossSpawns");
		
		_cardTables = new List<CardTable>();
		_slotMachines = new List<Node2D>();
		_bouncers = new List<Bouncer>();
		_pitBosses = new List<Pitboss>();

		_bouncerScene = GD.Load<PackedScene>("res://Scenes/Characters/Bouncer.tscn");
		_pitbossScene = GD.Load<PackedScene>("res://Scenes/Characters/Pitboss.tscn");
		_cardTableScene = GD.Load<PackedScene>("res://Scenes/CardTable.tscn");
		_slotMachineScene = GD.Load<PackedScene>("res://Scenes/Props/SlotMachine.tscn");
		_diceTimerScene = GD.Load<PackedScene>("res://Scenes/DiceTimer.tscn");

		_footstepSounds = new Dictionary<Tile, AudioStream>();
		AddFootstepSound(Tile.Stone,   "footstep_tile_1");
		AddFootstepSound(Tile.Tile,    "footstep_tile_1");
		AddFootstepSound(Tile.Carpet1, "footstep_carpet");
		AddFootstepSound(Tile.Carpet2, "footstep_carpet");
		AddFootstepSound(Tile.Wood,    "footstep_wood_1");

		GenerateLevel();
		_diceTimer.MakeVisibleAndStart();
		
		//_diceTimer.Connect("TimerFinished", this, nameof(OnDiceTimerFinished));
	}

	
	private void GenerateLevel()
	{
		GenerateTiles();
		SpawnPlayer();
		ClearCardTables();
		GenerateCardTables();
		ClearSlotMachines();
		GenerateSlotMachines();

		ClearEnemies();
		ClearSpawns();

		RestartDiceTimer();

		GetTree().Paused = false;
		_caught.Visible = false;
		_success.Visible = false;
	}

	private void RestartDiceTimer()
	{
		if (_diceTimer != null)
		{
			if (IsInstanceValid(_diceTimer))
			{
				_diceTimer.QueueFree();
			}
			_diceTimer = null;
		}
		_diceTimer = _diceTimerScene.Instance<DiceTimer>();
		AddChild(_diceTimer);
		_enemiesSpawning.Visible = true;
		_diceTimer.Connect("TimerFinished", this, nameof(OnDiceTimerFinished));
		_diceTimer.Position = _diceTimerPosition.Position;
		_diceTimer.MakeVisibleAndStart();
	}

	private void AddFootstepSound(Tile tile, string wavFileName)
	{
		var sound = ResourceLoader.Load<AudioStreamSample>($"res://Assets/Sounds/{wavFileName}.wav");
		_footstepSounds.Add(tile, sound);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (!Input.IsActionJustPressed("ui_select")) return;
		if (_player.AdjacentToObjective)
		{
			AdvanceLevel();
		} 
		else
		{
			
			GenerateLevel();
		}
	}

	private void GenerateTiles() 
	{
		// Base layer
		for (var x = 1; x < 31; x++)
		{
			for (var y = 2; y < 24; y++)
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
				if (x == 0 || x == 31 || y == 0 || y == 1 || y == 24) continue;
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
		for (var i = 0; i < _cardTableSpawnCounts[_currentLevel]; i++)
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
		for (var i = 0; i < _slotMachineSpawnCounts[_currentLevel]; i++)
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
		int idx;
		while (true)
		{
			idx = _random.Next(0, _propSpawns.Count);
			if (_usedPropSpawns.Contains(idx)) continue;
			_usedPropSpawns.Add(idx);
			break;
		}
		var spawnPoint = (Position2D)_propSpawns[idx];
		return spawnPoint.Position;
	}

	private Vector2 RandomPitbossSpawn()
	{
		int idx;
		while (true)
		{
			idx = _random.Next(0, _pitbossSpawns.Count);
			if (_usedPitbossSpawns.Contains(idx)) continue;
			_usedPitbossSpawns.Add(idx);
			break;
		}
		var spawnPoint = (Position2D)_pitbossSpawns[idx];
		return spawnPoint.Position;
	}

	private void SpawnPlayer()
	{
		_player.SetProtagonist(Player.RandomProtagonistOption(_random));
		//_player.Visible = true;
		_player.Position = RandomPropSpawn();
	}
	
	private void OnDiceTimerFinished()
	{
		_player.Visible = false;
		_enemiesSpawning.Visible = false;
		SpawnEnemies();
	}

	private void ClearEnemies()
	{
		ClearNodes(_bouncers);
		ClearNodes(_pitBosses);
	}

	private void ClearSpawns()
	{
		_usedPropSpawns.Clear();
		_usedPitbossSpawns.Clear();
	}

	private void SpawnEnemies()
	{
		for (var i = 0; i < _bouncerSpawnCounts[_currentLevel]; i++)
		{
			var bouncer = (Bouncer)_bouncerScene.Instance();
			_bouncers.Add(bouncer);
			bouncer.Position = RandomPropSpawn();
			AddChild(bouncer);
		}

		for (var i = 0; i < _pitbossSpawnCounts[_currentLevel]; i++)
		{
			var pitboss = (Pitboss)_pitbossScene.Instance();
			_pitBosses.Add(pitboss);
			pitboss.Position = RandomPitbossSpawn();
			AddChild(pitboss);
		}
	}

	private Tile PlayerCurrentTile()
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

	private async void AdvanceLevel()
	{
		GetTree().Paused = true;
		_success.Visible = true;
		_currentLevel++;
		_levelDie.Frame = _currentLevel + 1;
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		if (_currentLevel > 5)
		{
			GetTree().ChangeScene("res://Scenes/EndScreen.tscn");
		}
		else
		{
			GenerateLevel();
		}
	}

	public async void PlayerLose()
	{
		GetTree().Paused = true;
		_caught.Visible = true;
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		GenerateLevel();
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
