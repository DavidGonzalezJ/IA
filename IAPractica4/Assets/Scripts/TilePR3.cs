// David González Jiménez
// Patricia Cabrero Villar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum eTerreno { normal, zombi, soldado, lucha, heroe };
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
    public TilePR3() { }

	public int nZombie = 0;
	public bool soldado = false;
	public bool heroe = false;


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
		bool cambio = GameManager.Instance.Colocar() && !heroe;
		eTerreno aux = (eTerreno)(((int)estado.terreno + 1) % 3);
		bool change = true;
		if (estado.terreno == eTerreno.zombi)
			GameManager.Instance.changeZombies (-1);
		else if (estado.terreno == eTerreno.soldado)
			GameManager.Instance.changesoldados (-1);




        //Si puedo cambiar el estado de la casilla, lo cambio 
		if (cambio) {
			if (GameManager.Instance.nZombies == 20 && GameManager.Instance.nSoldados == 6) {
				estado.terreno = eTerreno.normal;
				change = false;
			} else if (GameManager.Instance.nSoldados == 6 && aux == eTerreno.soldado) {
				estado.terreno = eTerreno.normal;
				change = false;
			}else if(GameManager.Instance.nZombies == 20 && aux == eTerreno.zombi) {
				estado.terreno = eTerreno.soldado;
				change = false;
			}
			else	
				estado.terreno = (eTerreno)(((int)estado.terreno + 1) % 3);
				switch (estado.terreno) {
				case eTerreno.soldado:
					soldado = true;
					GameManager.Instance.changesoldados (1);
					nZombie = 0;
					break;
				case eTerreno.zombi:
				
					soldado = false;
					GameManager.Instance.changeZombies(1);
					nZombie = 1;
					break;
				}
				spriteCasilla.sprite = Imagenes [(int)estado.terreno];
		}
		if(GameManager.Instance.estado() != estadoJuego.simula)
			//Avisamos al manager de que ha cambiado
        	GameManager.Instance.Seleccionado(estado, change);
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
		if (estado.terreno == eTerreno.heroe) {
			GameManager.Instance.colocaHeroe(this.transform, estado.Posicion);
		}else
			spriteCasilla.sprite = Imagenes[(int)estado.terreno];
    }
	public void heroeColoc(){
		if(heroe)
			GameManager.Instance.colocaHeroe(this.transform, estado.Posicion);
	}
}
