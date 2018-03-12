using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


public enum Seleccion{ none, R, G, B};
public struct casilla{
	public dim Posicion;
	public eCasilla estado;
}

public class PuzzleManager : MonoBehaviour {
	//Parte Gráfica
	[SerializeField]
	private GameObject puzzle;
	public UnityEngine.UI.Text infoJuego;
	public GameObject [] Flechas = new GameObject [3];


	//Lógica interna
	private int tam = 10;
	public Seleccion Seleccion_ = Seleccion.none;
	private dim pSeleccion_;
	private eCasilla [,] matriz;

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
	}
	
	// Update is called once per frame
	void Update () {

	}

	public int dameTam(){
		return tam;	
	}
	public void Flecha(Transform trans){
		int flechaS = (int)(Seleccion_) - 1;
		Flechas[flechaS].transform.position = trans.position;
		Flechas[flechaS].SetActive(true);
		StartCoroutine(flechaDelay(flechaS));

	}
	public void GoTo(dim Posicion){

		//Llama al método resolutor con esa posición y la posicion del elemento seleccionado
		Debug.Log("El coche "+ Seleccion_.ToString() +" se moverá a la posición" + Posicion.x + " " + Posicion.y);
		infoJuego.text +=  Environment.NewLine + "Se moverá a la posición" + Posicion.x + " " + Posicion.y;
				
		//Quita la selección
		Seleccion_ = Seleccion.none;
		StartCoroutine(infoTextoDelay());

	}

	public bool Bloqueado(){
		return Seleccion_ == Seleccion.none;
	}
	public int Seleccionado(dim Posicion, eCasilla estado){

		matriz[(int)Posicion.x,(int)Posicion.y] = estado;
		if((int)estado > 2){//Es uno de los coches
			Seleccion_ = (Seleccion)((int)estado - 2);
			infoJuego.text = "El coche seleccionado: " + Seleccion_.ToString();
			pSeleccion_.Set(Posicion.x,Posicion.y);
		}
		return (int)(Seleccion_) - 1;
	}

	IEnumerator infoTextoDelay() {
        //Ahora las aplico en plan bonito
        yield return new WaitForSecondsRealtime(0.5f);
        infoJuego.text = "El coche seleccionado: " + Seleccion_.ToString();
    }

    IEnumerator flechaDelay(int flecha) {
       //Ahora las aplico en plan bonito
       yield return new WaitForSecondsRealtime(2.5f);
       Flechas[flecha].SetActive(false);
    }

}
