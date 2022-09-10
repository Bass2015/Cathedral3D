using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	public enum Type {
		Bridge, 
		Castle, 
		Hospital, 
		Hostel, 
		Mansion, 
		Monastery,
		Pub,
		Plaza,
		Stable,
		Tower,
		University, 
		Cathedral
	}

	public Type type;
	public PlayerData owner;

	Vector3 initPosition;
	GameObject[] bricks;
	public Square[] positionOnBoard;
	public Square[] neighbourhood;

	bool onBoard;
	bool onAir;
	int height;
	public int width;

	delegate void ResetSquares();
	ResetSquares resetSquares;

	delegate void OccupySquares(Building building);
	OccupySquares occupy;


	void Awake () {
		InitTransform();
		SaveAllPieces();
		SetColor(owner.material);
		InitDimensions();
		resetSquares = new ResetSquares(ResetPositionOnBoard);
	}

	// Use this for initialization
	void Start () {
		SetColor(owner.material);
	}

	#region INITIALIZATION
	void InitTransform()
	{
		gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
		Vector3 pos = gameObject.transform.position;
		initPosition = new Vector3(pos.x, pos.y, pos.z);
	}

	void SaveAllPieces () {
		bricks = new GameObject[gameObject.transform.childCount];
		for (int i = 0; i < bricks.Length; i++) {
			bricks[i] = gameObject.transform.GetChild(i).gameObject;
			bricks[i].AddComponent <Brick>();
		}
	}

	public void SetColor(Material color) {
		MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer rend in renderers)
		{
			rend.sharedMaterial = color;
		}
	}

	void InitDimensions() {
		switch (type) {
			case Type.Bridge:
				height = 3;
				width = 1;
				break;
			case Type.Castle: 
			case Type.Monastery:
			case Type.Mansion:
				height = 3;
				width = 2;
				break;
			case Type.Pub:
				height = 1;
				width = 1;
				break;
			case Type.Stable:
				height = 2;
				width = 1;
				break;
			case Type.Hostel:
			case Type.Plaza:
				height = 2;
				width = 2;
				break;
			case Type.Tower:
			case Type.University:
			case Type.Hospital:
				height = 3;
				width = 3;
				break;
			case Type.Cathedral:
				height = 3;
				width = 4;
				break;
		}
	}
	#endregion

	void Update () {
		//Cambiar esto por un evento, o algo así
		if (onBoard && IsSurrounded()) {
			GoBack();
		}
		if (Input.GetMouseButtonDown(1) && onAir) {
			Rotate();
		}
		if (Input.GetMouseButtonUp(0) && onAir) {
			TryLetDown();
		}

	}
	public void TryLetDown(){
		Square[] squaresUnder;
		if (CanLetDown(out squaresUnder)){
			positionOnBoard = squaresUnder;
			SortPositionOnBoard();
			LetDown();
		}
		else
			GoBack();
	}

	void SortPositionOnBoard(){
		List<Square> positionSquares = new List<Square>(positionOnBoard);
		positionSquares.Sort(new SquareComparer());
		positionOnBoard = positionSquares.ToArray();
	}

	bool CanLetDown (out Square[] squaresUnder) {
		bool isBoardUnder;
		squaresUnder = new Square[bricks.Length];
		for (int i = 0; i < bricks.Length; i++) {
			Brick brick = bricks[i].GetComponent <Brick>();
			isBoardUnder = brick.CanLetDown(out squaresUnder[i]);
			if (!isBoardUnder) { 
				return false;
			}
			resetSquares += new ResetSquares(squaresUnder[i].Reset);
			occupy += new OccupySquares(squaresUnder[i].SetBuilding);
		}
		return true;
	}

	#region BEHAVIOUR
	public void LetDown() {
		Vector3 pos = gameObject.transform.position;
		gameObject.transform.position = SnapPosition(pos);
		onAir = false;
		onBoard = true;
		occupy(this);
		SaveNeighbourhood();
		if (!this.CompareTag("Player0"))
		{
			foreach (var square in positionOnBoard)
			{
				TerritoryObserver.instance.CheckForNewTerritory(square);
			}
		}
    }

	public void Rotate () {
		
		gameObject.transform.Rotate(new Vector3 (0, 90f));
		int aux = height;
		height = width;
		width = aux;
		
	}

	public void Move () {
		if (!onBoard){
			onAir = true;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, 20f);
			gameObject.transform.position = new Vector3(hits[hits.Length - 1].point.x, 0.5f, hits[hits.Length - 1].point.z);
		}
	}

	public void GoBack(){
		if (positionOnBoard != null){
			resetSquares();
		}
		gameObject.transform.position = initPosition;
		onAir = false;
		onBoard = false;
	}
	#endregion

	#region TERRITORY
	void CheckTerritory(){
		for (int i = 0; i < neighbourhood.Length; i++){
			if (neighbourhood[i].occupied){
				if (!neighbourhood[i].buildingOn.owner.Equals(owner)){
					CreateSentry(neighbourhood[i]);
					break;
				}
			}
			else{
				CreateSentry(neighbourhood[i]);
				break;
			}
		}
	}


	void CreateSentry(Square initSquare){
		GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.position = new Vector3(initSquare.transform.position.x, 0.2f, initSquare.transform.position.z); 
		cyl.AddComponent<Sentry>();
		cyl.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
		cyl.transform.forward = Vector3.right;
		cyl.GetComponent<MeshRenderer>().material = owner.material;
		Sentry sentry = cyl.GetComponent<Sentry>();
		sentry.InitialSquare = initSquare;
		sentry.SentryPlayer = owner;
	}
	#endregion

	Vector3 SnapPosition(Vector3 pos){
		float newZ = Mathf.Round(pos.z);
		float newX = Mathf.Round(pos.x);

		if (width % 2 == 0){
			newX += SnapValueIfEven(pos.x);
		}
		if (height % 2 == 0){
			newZ += SnapValueIfEven(pos.z);
		}
		return new Vector3 (newX, 0.2f, newZ);
	}

	float SnapValueIfEven(float value) {
		float decimalPart = value - Mathf.Floor(value);
		if(decimalPart > 0.5f){
			return - 0.5f;
		}else{
			return 0.5f;
		}
	}

	#region NEIGHBOURHOOD
	void SaveNeighbourhood() {
		List<Square> squaresList = new List <Square>();
		foreach (Square sq in positionOnBoard) {
			AddNeighboursToList(sq, squaresList);
		}
		squaresList.Sort(new SquareComparer());
		neighbourhood = squaresList.ToArray();
	}

	void AddNeighboursToList(Square sq, List<Square> list) {
		Square newNeighbour = null;
		float x = sq.coordenates.x;
		float z = sq.coordenates.z;
		for (int i = 0; i < 4; i++) {
			switch (i) {
				case 0:
					newNeighbour = Board.instance.GetSquare(x - 1, z);
					goto case -1;
				case 1:
					newNeighbour = Board.instance.GetSquare(x + 1, z);
					goto case -1;
				case 2:
					newNeighbour = Board.instance.GetSquare(x, z - 1);
					goto case -1;
				case 3:
					newNeighbour = Board.instance.GetSquare(x, z + 1);
					goto case -1;
				case -1:
					if (CanAddNeighbour(list, newNeighbour))
						list.Add(newNeighbour);
					break;
			}
		}
	}

	bool CanAddNeighbour(List<Square> list, Square newNeighbour){
		return newNeighbour != null && !IsSquareInPosition(newNeighbour) && !list.Contains(newNeighbour);
	}
	bool IsSquareInPosition(Square sq) {
		foreach (Square mSq in positionOnBoard) {
			if (mSq.Equals(sq)){
				return true;
			}
		}
		return false;
	}

	//If all squares on the neighbourhood are occupied by the other player, this
	//building is surrounded and must go back out of the board.
	protected virtual bool IsSurrounded() {
		int enemySquares = 0;
		foreach (Square sq in neighbourhood) {
			if (sq.occupied && !sq.buildingOn.owner.Equals(owner)) {
				enemySquares++;
			}
		}
		if (enemySquares == neighbourhood.Length) {
			return true;
		}else  {
			return false;
		}
	}
	#endregion

	void ResetPositionOnBoard() {
		positionOnBoard = null;
	}

	void OnDrawGizmos() {
		/*Gizmos.DrawWireSphere(extendedNeighbourhood[0].gameObject.transform.position, 0.2f);
		Gizmos.DrawSphere(extendedNeighbourhood[1].gameObject.transform.position, 0.1f);

		for (int i = 2; i < extendedNeighbourhood.Length; i++)
		{
			Gizmos.DrawCube(extendedNeighbourhood[i].gameObject.transform.position, new Vector3(0.2f, 0.2f, 0.2f));

		}*/
	}

	public override string ToString(){
		return (type + " of " + owner.name);
	}



}
