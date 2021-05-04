using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory {

	List<Square> squares;

	public List<Square> Squares{
		get	{
			return squares;
		}
	}

	PlayerData owner;

	public Territory(List<Square> squares, PlayerData owner){
		this.squares = squares;
		this.owner = owner;
	}

	void DebugSquares(){
		string s = "";
		foreach (Square sq in squares) {
			s += sq.ToString() + ", ";		
		}
		Debug.Log(s);
	}
}
