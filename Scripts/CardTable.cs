using Godot;
using System;

public class CardTable : Node2D
{
	private Node2D _dealer;

	public override void _Ready()
	{
		_dealer = GetNode<Node2D>("Dealer");
	}

	public void RemoveDealer()
	{
		_dealer.QueueFree();
		_dealer = null;
	}
}
