using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleManager : MonoBehaviour {
	public int tam = 3;
	[SerializeField]
	private GameObject puzzle;
	private MatrizJuego matriz;
	private static PuzzleManager instance;
	public static PuzzleManager Instance{
		get{
			if(instance== null)
				instance = FindObjectOfType<PuzzleManager>();
			return instance;
		}

	}
	// Use this for initialization
	void Start () {
		matriz = this.GetComponent<MatrizJuego>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	public int dameTam(){
		return tam;	
	}
	public void move(){
        int Pieza;
        int.TryParse( EventSystem.current.currentSelectedGameObject.name, out Pieza );
		matriz.move(Pieza);
	}

}
