using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour {
	public static readonly int dimension = 10;
	public static Board instance;

	public delegate void ResetSquares();
	public ResetSquares resetSquares;

	public int Dimension
	{
		get
		{
			return dimension;
		}
	}

	public GameObject boardSquare;

	GameObject[,] squares = new GameObject[dimension, dimension];

	public GameObject[,] Squares
	{
		get
		{
			return squares;
		}
	}

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if (instance != this){
			Destroy (this);
		}
	}
	// Use this for initialization
	void Start () {
		InitializeBoard();
	}

	void InitializeBoard()
	{
		for (int z = 0; z < dimension; z++)
		{
			for (int x = 0; x < dimension; x++)
			{
				squares[z, x] = Instantiate(boardSquare, gameObject.transform) as GameObject;
				squares[z, x].transform.position = new Vector3(x, 0, z);
				resetSquares += new ResetSquares(squares[z, x].GetComponent<Square>().Reset);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public Square GetSquare (float x, float z) {
		try	{
			Square sq = Squares[(int)z, (int)x].GetComponent <Square>();
			return sq;
		} catch (IndexOutOfRangeException ior){
			ior.ToString();
			return null;		
		}
	}

	//Recorre el borde X desde startSquare. Si turnTheCorner es true, gira la esquina
	//y también recorre el borde z. Si encuentra una casilla ocupada por player, la agrega
	//a la lista y la devuelve.
	public static List<Square> FindNextFriendlySquareOnXBorder(Square startSquare, bool turnTheCorner, PlayerData player){
		List<Square> friendlySquares = new List<Square>();
		bool foundOne = false;
		for (int x = (int)startSquare.coordenates.x; x < dimension; x++){
			Square checkedSquare = instance.GetSquare(x, startSquare.coordenates.z);
			if(!checkedSquare.Equals(startSquare) && checkedSquare.occupied){
				if(checkedSquare.Owner.Equals(player)){
					friendlySquares.Add(checkedSquare);
				}
				foundOne = true;
				break;
			}
		}
		if(!foundOne && turnTheCorner){
			Square corner = instance.GetSquare(dimension - 1, startSquare.coordenates.z);
			friendlySquares.AddRange(FindNextFriendlySquareOnZBorder(corner, false, player));
		}
		foundOne = false;
		for (int x = (int)startSquare.coordenates.x; x >= 0; x--){
			Square checkedSquare = instance.GetSquare(x, startSquare.coordenates.z);
			if(!checkedSquare.Equals(startSquare) && checkedSquare.occupied){
				if(checkedSquare.Owner.Equals(player)){
					friendlySquares.Add(checkedSquare);
				}
				foundOne = true;
				break;
			}
		}
		if(!foundOne && turnTheCorner){
			Square corner = instance.GetSquare(0, startSquare.coordenates.z);
			friendlySquares.AddRange(FindNextFriendlySquareOnZBorder(corner, false, player));
		}
		return friendlySquares;
	}


	//Recorre el borde Z desde startSquare. Si turnTheCorner es true, gira la esquina
	//y también recorre el borde z. Si encuentra una casilla ocupada por player, la agrega
	//a la lista y la devuelve.
	public static List<Square> FindNextFriendlySquareOnZBorder(Square startSquare, bool turnTheCorner, PlayerData player)
	{
		List<Square> friendlySquares = new List<Square>();
		bool foundOne = false;
		for (int z = (int)startSquare.coordenates.z; z < dimension; z++)
		{
			Square checkedSquare = instance.GetSquare(startSquare.coordenates.x, z);
			if (!checkedSquare.Equals(startSquare) && checkedSquare.occupied)
			{
				if (checkedSquare.Owner.Equals(player))
				{
					friendlySquares.Add(checkedSquare);
				}
				foundOne = true;
				break;
			}
		}
		if (!foundOne && turnTheCorner)
		{
			Square corner = instance.GetSquare(startSquare.coordenates.x, dimension - 1);
			friendlySquares.AddRange(FindNextFriendlySquareOnXBorder(corner, false, player));
		}
		foundOne = false;
		for (int z = (int)startSquare.coordenates.x; z >= 0; z--)
		{
			Square checkedSquare = instance.GetSquare(startSquare.coordenates.x, z);
			if (!checkedSquare.Equals(startSquare) && checkedSquare.occupied)
			{
				if (checkedSquare.Owner.Equals(player))
				{
					friendlySquares.Add(checkedSquare);
				}
				foundOne = true;
				break;
			}
		}
		if (!foundOne && turnTheCorner)
		{
			Square corner = instance.GetSquare(startSquare.coordenates.x, 0);
			friendlySquares.AddRange(FindNextFriendlySquareOnXBorder(corner, false, player));
		}
		return friendlySquares;
	}



	public static void DebugSquares(Square[] a){
		string s = "";
		foreach (var item in a)
		{
			s += item.ToString() + ", ";
		}
		print(s);
	}
}
