using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cathedral : Building {
	protected override bool IsSurrounded(){
		int enemySquares = 0;
		int playerSurrounding = 0;
		foreach (Square sq in neighbourhood) {
			if (sq.occupied){
				if (sq.buildingOn.CompareTag("Player1")) {
					enemySquares++;
					if(playerSurrounding == 2){
						return false;
					}else{
						playerSurrounding = 1;
					}
				}else if(sq.buildingOn.CompareTag("Player2")){
					enemySquares++;
					if(playerSurrounding == 1){
						return false;
					}else{
						playerSurrounding = 2;
					}
				}
			}
		}
		if (enemySquares == neighbourhood.Length) {
			return true;
		}else  {
			return false;
		}
	}
}
