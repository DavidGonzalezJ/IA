using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Seleccion{ none, R, G, B};
public struct casilla{
	public dim Posicion;
	public eCasilla estado;
}

public class PuzzleManager : MonoBehaviour {
	//Parte Gráfica
	[SerializeField]
	private GameObject puzzle;

	//Lógica interna
	public int tam = 3;
	public Seleccion Seleccion_ = Seleccion.none;
	private dim pSeleccion_;
	private eCasilla [,] matriz;

	//private MatrizJuego matriz;

	//Esto es para instanciar al manager desde cualquier script
	//EJ:PuzzleManager.Instance.Seleccionado()
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
		matriz = new eCasilla[tam,tam];
		//matriz = this.GetComponent<MatrizJuego>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	public int dameTam(){
		return tam;	
	}
	/*
	public void move(){
        int Pieza;
        int.TryParse(EventSystem.current.currentSelectedGameObject.name, out Pieza );
		matriz.move(Pieza);
	}*/

	public int Seleccionado(dim Posicion, eCasilla estado){

		matriz[Posicion.x_,Posicion.y_] = estado;
		if((int)estado > 2){//Es uno de los coches
			Seleccion_ = (Seleccion)((int)estado - 2);
			pSeleccion_.SetPos(Posicion.x_,Posicion.y_);
		}
		switch(Seleccion_)
		{
			case Seleccion.none:
				return -1;
				break;
			case Seleccion.R:
				return 0;
				break;
			case Seleccion.G:
				return 1;
				break;
			case Seleccion.B:
				return 2;
				break;
		}
		return -1;
	}

}
