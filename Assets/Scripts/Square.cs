using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour {

	public enum Position{
		Top,
		Bottom, 
		Right,
		Left,
		BottomLeft,
		BottomRight,
		TopLeft,
		TopRight,
		Center
	}

	Position position;
	[SerializeField]
	public PlayerData owner;
	Material defaultMaterial;
	public static PlayerData defaultPlayerData;
	public bool isTerritory;
	public bool occupied;
	public Building buildingOn;
	MeshRenderer mRenderer;
	public Vector3 coordenates;
	string id;
	int sides;

	public Position MyPosition{
		get	{
			return position;
		}
	}
	public string Id{
		get{
			return id;
		}
	}
	public PlayerData Owner{
		get	{
			return owner;
		}
	}

	public bool IsTerritory	{
		get {
			return isTerritory;
		}
	}

	Text idLabel;

	// Use this for initialization
	void Start () {
		mRenderer = gameObject.GetComponent <MeshRenderer> ();
		defaultMaterial = mRenderer.material;
		coordenates = gameObject.transform.position;
		id = PositionToId(this.transform.position);
		DecidePosition();
		defaultPlayerData = owner;

		idLabel = transform.GetChild(0).GetChild(0).GetComponent<Text>();
		idLabel.text = id;
	}

	void DecidePosition(){
		if(coordenates.z == 0){
			if(coordenates.x == 0){
				position = Position.BottomLeft;
				sides = 2;
			}else if(coordenates.x == Board.instance.Dimension - 1){
				position = Position.BottomRight;
				sides = 2;
			}else{
				position = Position.Bottom;
				sides = 3;
			}
		}else if(coordenates.z == Board.instance.Dimension - 1){
			if(coordenates.x == 0){
				position = Position.TopLeft;
				sides = 2;
			}else if(coordenates.x == Board.instance.Dimension - 1){
				position = Position.TopRight;
				sides = 2;
			}else{
				position = Position.Top;
				sides = 3;
			}
		}else if(coordenates.x == 0){
			position = Position.Left;
			sides = 3;
		}else if(coordenates.x == Board.instance.Dimension - 1){
			position = Position.Right;
			sides = 3;
		}else{
			position = Position.Center;
			sides = 4;
		}
	}

	// Update is called once per frame
	void Update () {
		if(!isTerritory){
			OnBuildignOver();
		}
	}

	//Básicamente detecta si hay un edifico encima y se pone del color del edificio. 
	//Esto lo solucionaré cuando tenga un buen ordenador que pueda renderizar sombras.
	public void OnBuildignOver() {
		RaycastHit hitInfo;
		if(Physics.Raycast(gameObject.transform.position, Vector3.up, out hitInfo)){
			Brick brick = hitInfo.collider.gameObject.GetComponent <Brick>(); 
			if(brick != null) {
				Highlight(brick.GetColor());
			}
		}else{
			mRenderer.material = defaultMaterial;
		}
	}

	void Highlight(Color color) {
		float f = 0.5f; // desaturate by 20%
		float br = 1.4f;
		float L = 0.3f * color.r + 0.6f * color.g + 0.1f * color.b;
		float new_r = color.r*br + f * (L - color.r);
		float new_g = color.g*br + f * (L - color.g);
		float new_b = color.b*br + f * (L - color.b);
		Color newColor = new Color(new_r, new_g, new_b);
		Material mat = new Material(defaultMaterial);
		mat.color = newColor;
		mRenderer.material = mat;
	}

	public void Reset(){
		buildingOn = null;
		occupied = false;
		isTerritory = false;
		owner.buildingSquaresOnBoard = 0;
		owner.name = null;
		owner.material = null;
		mRenderer.material = defaultMaterial;

	}

	public void SetBuilding(Building building){
		buildingOn = building;
		owner = building.owner;
		occupied = true;
	}

	public void SetTerritory(PlayerData newOwner){
		if(newOwner.Equals(defaultPlayerData)){
			isTerritory = false; 
			owner = newOwner;
			mRenderer.material = defaultMaterial;
		}else{ 
			isTerritory = true;
			owner = newOwner;
			Highlight(owner.material.color);
		}
	}

	#region FIND NEIGHBOURS
	public Square[] GetFriendlyNeighbours(){
		int myX = (int)coordenates.x;
		int myZ = (int)coordenates.z;
		List<Square> neighbours = new List<Square>(8);

		for (int i = 0; i < 4; i++)
        {
            int x = (int)coordenates.x;
            int z = (int)coordenates.z;
            ChangeXAndZ(i, ref x, ref z);
            if (CoordenatesWithinRange(x, z))
            {
                Square neighbour = Board.instance.GetSquare(x, z);
                if (CanAddNeighbour(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        if (position != Position.Center){
			neighbours.AddRange(GetNeighboursFromBorderSquare());
		}

		return neighbours.ToArray();
	}

    private static void ChangeXAndZ(int i, ref int x, ref int z)
    {
        switch (i)
        {
            case 0:
                x--;
                break;
            case 1:
                x++;
                break;
            case 2:
                z--;
                break;
            case 3:
                z++;
                break;
        }
    }

    private bool NotMyCoordenates(int x, int z){
		return !(x == coordenates.x && z == coordenates.z);
	}
	private bool CoordenatesWithinRange(int x, int z){
		return x >= 0 && x < Board.instance.Dimension  && z >= 0 && z < Board.instance.Dimension;
	}
	private bool CanAddNeighbour(Square neighbour){
		return neighbour != null && neighbour.occupied && neighbour.owner.Equals(this.owner);
	}

	List<Square> GetNeighboursFromBorderSquare(){
		List<Square> friendlySquaresOnBorder = new List<Square>();
		switch (position){
			case Position.Bottom:
			case Position.Top:
				friendlySquaresOnBorder.AddRange(Board.FindNextFriendlySquareOnXBorder(this, true, owner));
				break;
			case Position.Left:
			case Position.Right:
				friendlySquaresOnBorder.AddRange(Board.FindNextFriendlySquareOnZBorder(this, true, owner));
				break;
			case Position.BottomLeft:
			case Position.BottomRight:
			case Position.TopLeft:
			case Position.TopRight:
				friendlySquaresOnBorder.AddRange(Board.FindNextFriendlySquareOnXBorder(this, false, owner));
				friendlySquaresOnBorder.AddRange(Board.FindNextFriendlySquareOnZBorder(this, false, owner));
				break;
		}
		return friendlySquaresOnBorder;
	}
	#endregion

	public static string PositionToId(Vector3 position){
		float intId = position.x * 10 + position.z;
		string id = "";
		if(intId < 10){
			id += "0";
		}
		return id += intId.ToString();
	}

	void OnDrawGizmos(){
		
	}
	public bool IsInsideTerritory(){
		if(occupied){
			return true;
		}
		float myX = coordenates.x;
		float myZ = coordenates.z;

        for (int x = (int)myX - 1; x <= (int)myX + 1; x++)
        {
            for (int z = (int)myZ - 1; z <= (int)myZ + 1; z++)
            {
                Square neighbour = Board.instance.GetSquare(x, z);
                if (neighbour != null)
                {
					print("Hay vecino");
                    if (neighbour.owner.Equals(null) || !neighbour.owner.Equals(this.owner))
                    {
						print(this.ToString() + " Me desmarqué");
                        return false;
                    }
                }
            }
        }

        /*for (int i = 0; i < 4; i++)	{
			Square neighbour = null;
			switch (i) {
				case 0:
					neighbour = Board.instance.GetSquare(myX, myZ + 1);
					break;
				case 1:
					neighbour = Board.instance.GetSquare(myX, myZ - 1);
					break;
				case 2:
					neighbour = Board.instance.GetSquare(myX + 1, myZ);
					break;
				case 3:
					neighbour = Board.instance.GetSquare(myX - 1, myZ);
					break;
			}
			if (neighbour != null) {
				if (neighbour.owner.Equals(null) || !neighbour.owner.Equals(this.owner)) {
					return false;
				}
			}
		}*/
        return true;
	}
		
	bool HasSameOwner(Square other){
		return other.owner.Equals(this.owner);
	}

	public override string ToString(){
		return this.id;
	}
}
