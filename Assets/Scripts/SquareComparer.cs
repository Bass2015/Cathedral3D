using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareComparer : IComparer<Square>{
	public int Compare (Square a, Square b){
		Vector3 squareA = a.gameObject.transform.position;
		Vector3 squareB = b.gameObject.transform.position;
		if(squareA.z == squareB.z) {
			if (squareA.x < squareB.x)
				return -1;
			else
				return 1;
		}else if(squareA.z < squareB.z){
			return 1;
		}else{
			return -1;
		}
	}

}
