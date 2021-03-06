﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dim{
	public int x,y;
	public dim(){}
	public dim (int xI, int yI){
		x=xI;
		y=yI;
	}
	public void Set(int xI, int yI){
		x=xI;
		y=yI;
	}
}
public enum eCasilla { normal,embarrado, bloqueado, obsR, obsG, obsB }

public class TilePR2 : MonoBehaviour {
	eCasilla estadoAnterior = eCasilla.normal;
	bool Ocupada = false;
	public dim Posicion = new dim();
	public eCasilla estado = eCasilla.normal;
	Image spriteCasilla;
	public Sprite[] Imagenes = new Sprite[6];

	public void SetOcupada(bool O){
		Ocupada = O;
	}

	void Awake(){
		Image a = this.GetComponent<Image>();
        if (a != null){
            spriteCasilla = a;
        }
        else
        	Debug.Log("No encontrado componente imagen TILE");

	}
	// Use this for initialization
	void Start () {
		spriteCasilla.sprite = Imagenes[(int)estado];
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Click (){
		bool cambio = PuzzleManager.Instance.Bloqueado();
        //Si puedo cambiar el estado de la casilla, lo cambio 
		if(cambio && (int)estado < 3){
			estadoAnterior = estado;
			estado = (eCasilla)(((int)estado+1) % 3);
			spriteCasilla.sprite = Imagenes[(int) estado];
		}else if (!cambio && !Ocupada && estado != eCasilla.bloqueado){ 
			//Esta será la casilla a la que queremos ir con el coche seleccionado
			//Ponemos la flecha en la casilla
			PuzzleManager.Instance.Flecha(this.transform);
			//Avisamos al Puzzle manager de que queremos ir alli.
			PuzzleManager.Instance.GoTo(Posicion);
		}
		//Avisamos al manager de que ha cambiado
		//Este método devuelve un int que representa que coche esta seleccionado
		PuzzleManager.Instance.Seleccionado(Posicion, estado);
		//Ponemos la variable de Ocupado a true si hay un coche o si es una roca
		if((int)estado >= 2) Ocupada = true;
	}
	public void vuelve () {
		estado = estadoAnterior;
		spriteCasilla.sprite = Imagenes[(int) estado];
		Ocupada = false;
	}
	public bool avanza (int coche) {
        if (Ocupada) return false;
        estadoAnterior = estado;
		estado = (eCasilla) (coche + 3);
		spriteCasilla.sprite = Imagenes[(int) estado];
		Ocupada = true;
        return true;
	}

}
