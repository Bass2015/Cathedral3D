using System;
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
	public PlayerData player0;
    public Building cathedral;

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
	void Start ()
    {
        InitPlayers();

        SetBuildingMaterials();

        board = GameObject.FindObjectOfType<Board>();

    }

    private void PlaceCathedral()
    {
        int turns = Random.Range(0, 4);
        for (int i = 0; i < turns; i++)
        {
            cathedral.Rotate();
        }
        float x = 0;
        while(x == 0 || IsNearRound(x))
        {
            x = Random.Range(1.5f, 7.6f);
        }

        float z = 0;
        while (z == 0 || IsPointFive(z))
        {
            z = Random.Range(1, 8.1f); 
        }
        if (turns % 2 != 0)
        {
            float temp = x;
            x = z;
            z = temp;
        }
        cathedral.transform.position = new Vector3(x, 0.5f, z);
        print("Width: " + cathedral.width + " POS: " + x.ToString() +", " + z.ToString());
        cathedral.TryLetDown();
    }

    bool IsPointFive(float number)
    {
        return Mathf.Abs(number - Mathf.Round(number)) > 0.39f &&
                Mathf.Abs(number - Mathf.Round(number)) < 0.61f;
    }

    bool IsNearRound(float number)
    {
        return Mathf.Abs(number - Mathf.Round(number)) < 0.11f;
    }
    private void SetBuildingMaterials()
    {
        buildings = FindObjectsOfType<Building>();
        foreach (Building build in buildings)
        {
            if (build.gameObject.tag.Equals("Player1"))
            {
                build.SetColor(player1.material);
                build.owner = player1;
            }
            if (build.gameObject.tag.Equals("Player2"))
            {
                build.SetColor(player2.material);
                build.owner = player2;
            }
            if (build.gameObject.tag.Equals("Player0"))
            {
                build.SetColor(player0.material);
                build.owner = player0;
            }
        }
    }

    private void InitPlayers()
    {
        player1.name = "Edu";
        player1.material = new Material(Shader.Find("Standard"));
        player1.material.color = new Color(0.96f, 0.74f, 0.19f);

        player2.name = "Bass";
        player2.material = new Material(Shader.Find("Standard"));
        player2.material.color = new Color(0.37f, 0.42f, 0.96f);

        player0.name = "Cathedral";
        player0.material = new Material(Shader.Find("Standard"));
        player0.material.color = new Color(1, 0.45f, 0.31f);

        npc.name = "City";
        npc.material = squareMaterial;
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
            print(Mathf.Epsilon);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlaceCathedral();
        }
    }
}
