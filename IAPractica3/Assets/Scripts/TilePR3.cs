// David González Jiménez
// Patricia Cabrero Villar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum eTerreno { normal, barro, agujero };
public enum eCadaver { nada, cadaver, sangre, arma };
public class Casilla
{
    public eTerreno terreno = eTerreno.normal;
    public eCadaver contenido = eCadaver.nada;
    public Pos Posicion = new Pos();
}
public class TilePR3 : MonoBehaviour {
	public Casilla estado = new Casilla();

	Image spriteCasilla;

	public Sprite[] Imagenes = new Sprite[3];

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
		/*bool cambio = PuzzleManager.Instance.Bloqueado();
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
		if((int)estado >= 2) Ocupada = true;*/
	}
	public void vuelve () {
		spriteCasilla.sprite = Imagenes[(int) estado.terreno];
	}
	public bool avanza (int coche) {
		estado.terreno = (eTerreno) (coche + 3);
		spriteCasilla.sprite = Imagenes[(int) estado.terreno];
        return true;
	}

}
