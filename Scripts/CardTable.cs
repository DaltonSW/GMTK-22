using Godot;
using System;

public class CardTable : Node2D
{
	private Node2D _dealer;
	private Sprite _exclamation;

	public override void _Ready()
	{
		_dealer = GetNode<Node2D>("Dealer");
		_exclamation = GetNode<Sprite>("ExclamationPoint");
	}

	public void RemoveDealer()
	{
		_dealer.QueueFree();
		_dealer = null;
		_exclamation.QueueFree();
	}
}
