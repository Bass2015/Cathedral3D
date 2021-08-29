using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deprecated
public class Sentry : MonoBehaviour {
	private PlayerData mPlayer = new PlayerData();
	public PlayerData SentryPlayer{
		get{
			return mPlayer;
		}
		set{
			mPlayer = value;
		}
	}

	private Square initialSquare;
	public Square InitialSquare{
		get{
			return initialSquare;
		}
		set{
			initialSquare = value;
		}
	}

	private int Turns;
	private int BoundsTouched;
	public List<Square> Border;
	private Vector3 TurnSense;
	private bool AlreadyMoved;
	private bool BorderClosed;
	private bool TurningLeft;

	Sentry(){
		Turns = 0;
		BoundsTouched = 0;
		Border = new List<Square>();
		TurnSense = new Vector3 (0f, 90f, 0f);
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(Patrol());
	}

	IEnumerator Patrol(){
		while (!BorderClosed){
			if(BoundsTouched == 1){
				CheckSide();
			}else{
				if(AlreadyMoved || Turns == 4){
					if (StartPosition()){
						CloseBorder();
					}else 
						CheckSide();
				}else{
					if(CheckNeighbour(transform.forward)){
						Turn();
					}
					CheckSide();
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	bool StartPosition(){
		Vector3 actualPosition = new Vector3(transform.position.x, 0f, transform.position.z);
		return actualPosition == initialSquare.transform.position;
	}

	void CloseBorder(){
		print("Cierro la frontera");
		BorderClosed = true;
		Destroy(this.gameObject);
	}

	void CheckSide(){
		Vector3 sideToCheck = transform.right;
		if(TurningLeft){
			sideToCheck *= -1;
		}
		Building build = CheckNeighbour(sideToCheck);
		if(build != null){
			AddSquaresToBorder(build);
			CheckBorderLimits();
		}else{
			transform.Rotate(TurnSense);
			MoveForward();
		}
	}

	void AddSquaresToBorder(Building build){
		foreach (Square sq in build.positionOnBoard){
			if(!Border.Contains(sq)){
				Border.Add(sq);
			}	
		}
	}

	void MoveForward(){
		Building build = CheckNeighbour(transform.forward);
		if(build != null){
			Turn();
		}else{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, 1f);
			Turns = 0;
			if(!AlreadyMoved){
				AlreadyMoved = true;
			}
		}
	}

	Building CheckNeighbour(Vector3 direction){
		RaycastHit hit = new RaycastHit();
		if (direction != transform.forward)
			Debug.DrawRay(transform.position, direction, Color.black);
		if(Physics.Raycast(transform.position, direction, out hit, 1f)){
			GameObject objectHit = hit.collider.gameObject;
			GameObject parent = objectHit.transform.parent.gameObject;
			Building build = parent.GetComponent<Building>();
			if(build != null && build.owner.Equals(mPlayer)){
				return build;
			}	
		}
		return null;
	}

	void Turn(){
		transform.Rotate(TurnSense * -1);
		Turns++;
	}

	void CheckBorderLimits(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 1f)){
			if(hit.collider.tag.Equals("Player0")){
				print("Toqué el borde");
				BoundsTouched++;
				if(BoundsTouched == 2){
					print("Toqué dos veces");
					CloseBorder();
				}else{
					ChangeSense();
				}
			}else{
				MoveForward();
			}
		}else{
			MoveForward();
		}
	}

	void ChangeSense(){
		transform.Rotate(new Vector3 (0f, 180f, 0f));
		TurningLeft = true;
		TurnSense *= -1;
	}

	// Update is called once per frame
	void Update () {
	}

	void OnDrawGizmos(){
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
