using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct PlayerData {
	public string name;
	public Material material;
	public int buildingSquaresOnBoard;
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public PlayerData player1;
	public PlayerData player2;
	public PlayerData npc;

	public Material squareMaterial;

	private Building[] buildings;
	private Board board;
	void Awake(){
		if(instance == null){
			instance = this;
		} else if (instance != this){
			Destroy (this);
		}
	}

	// Use this for initialization
	void Start () {
		player1.name = "Edu";
		player1.material = new Material(Shader.Find("Sprites/Diffuse"));
		player1.material.color = Random.ColorHSV();

		player2.name = "Bass";
		player2.material = new Material(Shader.Find("Sprites/Diffuse"));
		player2.material.color = Random.ColorHSV();

		npc.name = "City";
		npc.material = squareMaterial;

		buildings = FindObjectsOfType <Building>();
		foreach(Building build in buildings) {
			if (build.gameObject.tag.Equals("Player1")){
				build.SetColor(player1.material);
				build.owner = player1;
			}
			if(build.gameObject.tag.Equals("Player2")) {
				build.SetColor(player2.material);
				build.owner = player2;
			}
		}

		board = GameObject.FindObjectOfType<Board>();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (Building building in buildings)
            {
				building.GoBack();
            }
			board.resetSquares();
			TerritoryMarker.Reset();
        }
	}
}
