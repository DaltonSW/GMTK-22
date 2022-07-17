using Godot;
using System;

public class CardTable : Node2D
{
	private Node2D _dealer;
	private Sprite _exclam;

	public override void _Ready()
	{
		_dealer = GetNode<Node2D>("Dealer");
		_exclam = GetNode<Sprite>("ExclamPoint");
	}

	public void RemoveDealer()
	{
		_dealer.QueueFree();
		_dealer = null;
		_exclam.Visible = false;
	}
}
