using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {
	Building mainBuilding;


	// Use this for initialization
	void Start () {
		Transform parentTransform = gameObject.transform.parent;
		mainBuilding = parentTransform.gameObject.GetComponent <Building>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDrag () {
		if (Input.GetMouseButton(0)) {
			mainBuilding.Move();
		}
	}

	public Color GetColor(){
		Renderer rend = GetComponent <Renderer>();
		return rend.material.color;
	}

	public bool CanLetDown(out Square boardSqOut)	{
		RaycastHit hitInfo;
		if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hitInfo)){
			Square square = hitInfo.collider.gameObject.GetComponent <Square>();
			if (square != null) {
				if(square.IsTerritory && !square.Owner.Equals(mainBuilding.owner)){
					boardSqOut = null;
					return false;
				}else{
					boardSqOut = square;
					return true;	
				}
			}
		}
		boardSqOut = null;
		return false;
	}

	void OnMouseOver () {
		
	}
}
