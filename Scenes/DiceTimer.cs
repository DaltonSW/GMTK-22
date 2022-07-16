using Godot;
using System;

public class DiceTimer : Node2D
{
    [Signal]
    public delegate void TimerFinished();
    
    private AnimatedSprite _die;
    private Timer _timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _die = GetNode<AnimatedSprite>("Die");
        _timer = GetNode<Timer>("Timer");
        _timer.Connect("timeout", this, nameof(OnTimerTimeout));
        _die.Visible = false;
    }

    public void OnTimerTimeout()
    {
        _die.Frame--;
        if (_die.Frame == 0)
        {
            EmitSignal(nameof(TimerFinished));
            _timer.Stop();
            QueueFree();
        }
    }

    public void MakeVisibleAndStart()
    {
        _die.Frame = 6;
        _die.Visible = true;
        _timer.Start();
    }
}
