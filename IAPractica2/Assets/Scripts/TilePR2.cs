using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct dim{
	public int x_,y_;
	public void SetPos(int x, int y){
		x_=x;
		y_=y;
	}
}
public enum eCasilla { normal,embarrado, bloqueado, obsR, obsG, obsB }

public class TilePR2 : MonoBehaviour {
	bool Ocupada = false;
	public dim Posicion;
	public eCasilla estado = eCasilla.normal;
	Image spriteCasilla;
	public Sprite[] Imagenes = new Sprite[6];
	public int x;
	public int y;


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
		
		if((int)estado < 3){
			estado = (eCasilla)(((int)estado+1) % 3);
			spriteCasilla.sprite = Imagenes[(int) estado];
		}else{
			//Que pasaría si seleccionamos el coche
		}
		//Avisamos al manager de que ha cambiado
		int coche = PuzzleManager.Instance.Seleccionado(Posicion, estado);
	}
}
