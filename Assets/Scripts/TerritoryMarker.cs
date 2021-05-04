using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerritoryMarker {
	static List<Territory> territories;
	const int Center = 0;
	const int Left = 1;
	const int Right = 2;
	const int Both = 3;

	public static void MarkTerritory(Square[] borderSquares){
		List<Square> totalSquaresInBorder = new List<Square>();
		bool touchesLeftBorder = false;
		bool touchesRightBorder = false;
		IsBoardLimit(borderSquares, ref touchesLeftBorder, ref touchesRightBorder);
		int actualZ = (int) borderSquares[0].coordenates.z;
		int minX =  (int)borderSquares[0].coordenates.x;
		int maxX = Board.dimension - 1;
		if(touchesLeftBorder){
			minX = 0;
		}
		for (int i = 0; i < borderSquares.Length; i++) {
			if(borderSquares[i].coordenates.z != actualZ){
				totalSquaresInBorder.AddRange(NewRow(actualZ, minX, maxX));
				actualZ = (int)borderSquares[i].coordenates.z;
				if(!touchesLeftBorder){
					minX = (int)borderSquares[i].coordenates.x;
				}
			}
			if(!touchesRightBorder){
				maxX = (int)borderSquares[i].coordenates.x;
			}
		}
		totalSquaresInBorder.AddRange(NewRow(actualZ, minX, maxX));
		MarkAllSquares(totalSquaresInBorder.ToArray(), borderSquares[0].Owner);
		UnMarkSquaresOutsideOfTerritory(totalSquaresInBorder.ToArray());
	}



	static void IsBoardLimit(Square[] borderSquares, ref bool touchesLeftBorder, ref bool touchesRightBorder){
		foreach (Square square in borderSquares){
			if(square.MyPosition == Square.Position.TopLeft || 
				 square.MyPosition == Square.Position.BottomLeft || 
				 square.MyPosition == Square.Position.Left)
			{
				touchesLeftBorder = true;
			}else if(square.MyPosition == Square.Position.TopRight|| 
				square.MyPosition == Square.Position.BottomRight|| 
				square.MyPosition == Square.Position.Right)
			{
				touchesRightBorder = true;
			}
		}
	}

	static List<Square> NewRow(int z, int fromX, int toX)	{
		List<Square> row = new List<Square>();
		for (int x = fromX; x <= toX; x++) {
			row.Add(Board.instance.GetSquare(x, z));
		}
		return row;
	}

	static void MarkAllSquares(Square[] territorySquares, PlayerData owner){

		foreach (var square in territorySquares) {
			if (!square.IsTerritory) {
				square.SetTerritory(owner);
			}
		}
	}

	static void UnMarkSquaresOutsideOfTerritory(Square[] squares){
		bool haveUnMarked = false;
		do {
			haveUnMarked = false;
			for (int i = 0; i < squares.Length; i++){
				if (squares[i] != null && !squares[i].IsInsideTerritory()){
					squares[i].SetTerritory(Square.defaultPlayerData);
					squares[i] = null;
					haveUnMarked = true;
				}
			}
		} while(haveUnMarked);
	}
}
