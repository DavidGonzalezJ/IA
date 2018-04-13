// David González Jiménez
// Patricia Cabrero Villar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum eTerreno { normal, zombi, soldado, lucha };
public class Casilla
{
    public Casilla() { 
		terreno = eTerreno.normal;
        Posicion = new Pos();
    }
    public eTerreno terreno;
    public Pos Posicion;
}

public class TilePR3 : MonoBehaviour {
	
	int nZombie = 0;
	bool soldado = false;
	bool heroe = false;

	public Casilla estado = new Casilla();

	Image spriteCasilla;

	public Sprite[] Imagenes = new Sprite[4];

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
		spriteCasilla.sprite = Imagenes[(int)estado.terreno];
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Click (){
		bool cambio = !(GameManager.Instance.Simulando());
        //Si puedo cambiar el estado de la casilla, lo cambio 
		if(cambio){
			estado = (eTerreno)(((int)estado+1) % 3);
			switch (estado) {
			case eTerreno.soldado:
				soldado = true;
				nZombie = 0;
				break;
			}
			spriteCasilla.sprite = Imagenes[(int) estado];
		}
		//Avisamos al manager de que ha cambiado
		//Este método devuelve un int que representa que coche esta seleccionado
		GameManager.Instance.Seleccionado(Posicion, estado);
		//Ponemos la variable de Ocupado a true si hay un coche o si es una roca
		if((int)estado >= 2) Ocupada = true;
        GameManager.Instance.Seleccionado(estado);
    }
	public void vuelve () {
		spriteCasilla.sprite = Imagenes[(int) estado.terreno];
	}
	public bool avanza (int coche) {
		estado.terreno = (eTerreno) (coche + 3);
		spriteCasilla.sprite = Imagenes[(int) estado.terreno];
        return true;
	}
    public void actualiza(Casilla c) {
        estado.terreno = c.terreno;
        spriteCasilla.sprite = Imagenes[(int)estado.terreno];
        estado.contenido = c.contenido;
        if (estado.contenido != eCadaver.nada) {
            GameManager.Instance.colocaAsset(this.transform, estado.contenido);
        }
    }

}
