using Godot;
using System;

public class Intro : Node2D
{
    private Player _player;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var rng = new Random();
        _player = GetNode<Player>("Player");
        _player.SetProtagonist(Player.RandomProtagonistOption(rng));
        _player.Visible = true;
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
}
