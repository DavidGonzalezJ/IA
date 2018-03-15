using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


public enum Seleccion{ none, R, G, B};


public class PuzzleManager : MonoBehaviour {
	//Parte Gráfica
	[SerializeField]
	private GameObject puzzle;
	public UnityEngine.UI.Text infoJuego;
	public GameObject [] Flechas = new GameObject [3];

	private Transform Piezas;

	//Lógica interna
	private int tam = 10;
	public Seleccion Seleccion_ = Seleccion.none;
	private dim pSeleccion_ = new dim();
	private dim [] posCoche = new dim [3];
	public eCasilla [,] matriz;

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
        for (int i = 0; i < 3; i++) {
            matriz[i, 0] = (eCasilla)(i + 3);
        }
    }
	
	// Update is called once per frame
	void Update () {
      
    }

	public void SetPiezas(Transform puzzleField){
		Piezas = puzzleField;
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
		infoJuego.text +=  Environment.NewLine + "Se moverá a la posición" + Posicion.x + " " + Posicion.y;
		
		int coche = (int)(Seleccion_) - 1;
		StartCoroutine( resolver(pSeleccion_ , Posicion, coche));
		
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

	IEnumerator resolver(dim origen, dim destino, int coche) {
		Resolutor resolutor = new Resolutor(matriz,origen,destino);
		Transform pieza = Piezas.GetChild(origen.x + origen.y*10);
		TilePR2 logica = pieza.GetComponent<TilePR2>();
        bool avanzar = true;
        for (int i = 1; avanzar && i <= resolutor.camino.Count; i++) {
            logica.vuelve();
            matriz[resolutor.camino[i - 1].x, resolutor.camino[i - 1].y] = logica.estado;
            pieza = Piezas.GetChild(resolutor.camino[i - 1].x + resolutor.camino[i - 1].y * 10);
            logica = pieza.GetComponent<TilePR2>();
            avanzar = logica.avanza(coche);
            if (!avanzar)
            {
                pieza = Piezas.GetChild(resolutor.camino[i - 2].x + resolutor.camino[i - 2].y * 10);
                logica = pieza.GetComponent<TilePR2>();
                logica.avanza(coche);
                matriz[resolutor.camino[i - 2].x, resolutor.camino[i - 2].y] = logica.estado;
            }
        
            yield return new WaitForSecondsRealtime(0.5f);
        }

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
    public 
}
