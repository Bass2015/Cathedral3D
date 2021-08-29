using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryObserver : MonoBehaviour{

	public static TerritoryObserver instance;

	Square root;

	Stack<Square> openStack = new Stack<Square>(100);
	Stack<Square[]> families = new Stack<Square[]>(100);
	List<Square> closedList = new List<Square>(100);
	//List<Building> buildingsAtNewTerritory


	List<Square> newChildren = new List<Square>(8);
	List<Square> newFamily =  new List<Square>(8);

	bool firstLoop = true;



	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this){
			Destroy (this);
		}
	}


	//Este es el método que se llama cada vez que se coloca una ficha.
	//El parámetro 'firstSquare' es la una de las casillas en las que está
	//colocada esa ficha.
	public void CheckForNewTerritory (Square firstSquare){
		if(!firstLoop){
			firstLoop = true;
		}
		ResetMembers();
		CheckSquare(firstSquare);
	}
	/// <summary>
	/// Algoritmo principal de la clase. Las sucesivas iteraciones pasan por todos los 
	/// nodos de un grafo sin volver atrás y comprobando si se ha cerrado algún ciclo
	/// dentro de él.
	/// </summary>
	/// <param name="squareToCheck">Square to check.</param>
	void CheckSquare (Square squareToCheck)
	{
		//Guardar el edificio del cuadrado
		Square[] familyToCheck = null;
		ResetFamily();
		if (firstLoop)
		{
			root = squareToCheck;
			IterateThroughFriendlyNeighbours(squareToCheck, familyToCheck);
			EndChecking(squareToCheck);
		}
		else 
		{
			familyToCheck = families.Pop();
			if (closedList.Contains(squareToCheck))
			{
				CheckIfANeighbourIsRoot(squareToCheck);
				EndChecking(squareToCheck);
			} 
			else
			{
				closedList.Add(squareToCheck);
				IterateThroughFriendlyNeighbours(squareToCheck, familyToCheck);
				EndChecking(squareToCheck);
			}
		}
			
	}

	void SaveBuilding(Square square)
    {

    }
	/// <summary>
	/// Reinizializa todos los miembros cuando comienza a recorrer un 
	/// nuevo posible territorio.
	/// </summary>
	void ResetMembers()
	{
		ResetFamily();
		openStack.Clear();
		families.Clear();
		closedList.Clear();
		root = null;
	}

	/// <summary>
	/// Reinicializa newChildren y newFamily antes de pasar a 
	/// comprobar una nueva casilla.
	/// </summary>
	void ResetFamily()
	{
		newChildren.Clear();
		newFamily.Clear();
	}


	void CheckIfANeighbourIsRoot(Square squareToCheck){
		Square[] neighbours = squareToCheck.GetFriendlyNeighbours();
		foreach (Square neighbour in neighbours) {
			if(neighbour.Equals(root)){
				CreateNewTerritory();
				break;
			}
		}
	}

	void EndChecking(Square squareToCheck){
		if(newChildren.Count != 0){
			PrepareNewFamily(squareToCheck);
		}
		if(firstLoop){
			firstLoop = false;
		}
		if(openStack.Count != 0){
			CheckSquare(openStack.Pop());
		}
	}

	void PrepareNewFamily(Square squareToCheck){
		foreach (Square child in newChildren)
		{
			newFamily.Add(child);
		}
		newFamily.Add(squareToCheck);
		foreach (Square child in newChildren)
		{
			openStack.Push(child);
			families.Push(newFamily.ToArray());
		}
	}

	void IterateThroughFriendlyNeighbours(Square squareToCheck, Square[] familiyToCheck){
		Square[] neighbours = squareToCheck.GetFriendlyNeighbours();
		foreach (Square neighbour in neighbours)
		{
			CheckNeighbour(neighbour, familiyToCheck);

			//DEBUG
			Debug.DrawLine(squareToCheck.transform.position, neighbour.transform.position, Color.black, 2);

		}
	}

	void CheckNeighbour(Square neighbour, Square[] familyToCheck){
		 
		if (familyToCheck == null || familyToCheck.Length == 0){
			IsNeighbourRootOrChild(neighbour);
		}else{
			if(IsSquareInArrayOrInClosedList(neighbour, familyToCheck)){
				newFamily.Add(neighbour);
			}else{
				IsNeighbourRootOrChild(neighbour);
			}
		}
	
	}

	void IsNeighbourRootOrChild(Square neighbour){
		if (neighbour.Equals(root))	{
			CreateNewTerritory();
		} else {
			newChildren.Add(neighbour);
		}
	}

	bool IsSquareInArrayOrInClosedList(Square square, Square[] array){
		if(closedList.Contains(square)){
			return true;
		}
		foreach (Square squareInArray in array)
		{
			if(square.Equals(squareInArray)){
				return true;
			}
		}

		return false;
	}

	
	public void CreateNewTerritory(){
		List<Square> borderOfNewTerritory = closedList;
		borderOfNewTerritory.Add(root);
		borderOfNewTerritory.Sort(new SquareComparer());
		TerritoryMarker.MarkTerritory(borderOfNewTerritory.ToArray());

		//DEBUG
		//StartCoroutine(DebugFrontera());
		
	}

	IEnumerator DebugFrontera()
    {
		string squaresId = "";
		List<GameObject> marcadores = new List<GameObject>();
		foreach (Square square in closedList)
		{
			squaresId += square.Id + ", ";
			GameObject marcador = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			marcador.transform.localScale = new Vector3(0.1f, 2.0f, 0.1f);
			marcador.transform.position = square.transform.position;
			marcadores.Add(marcador);
		}
		yield return new WaitForSeconds(2);
		foreach (var item in marcadores)
		{
			Destroy(item);
		}
		print(squaresId);
	}
}
