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
	public GameObject [] Coches = new GameObject [3];

	private Transform Piezas;

	//Lógica interna
	private int tam = 10;
	public Seleccion Seleccion_ = Seleccion.none;
	private dim pSeleccion_ = new dim();
	private dim [] posCoche = new dim [3];
	public eCasilla [,] matriz;
    bool inicial = true;
    int iJ = 0;

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
        if (inicial)
        {
            colocaCoche(-1, 0);
        }else
            inicial = false;
    }

	public void SetPiezas(Transform puzzleField){
		Piezas = puzzleField;
    }

	public int dameTam(){
		return tam;	
	}
    public GameObject dameCoche(int coche) {
        return Coches[coche];
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
		dim ant = origen;
       	foreach(var step in resolutor.camino ) {
            int casilla = step.x + step.y * 10;
            colocaCoche(casilla, coche);
            /*logica.vuelve();
       		matriz[ant.x, ant.y] = logica.estado;

       		pieza = Piezas.GetChild(step.x + step.y*10);
       		logica = pieza.GetComponent<TilePR2>();
       		logica.avanza(coche);
       		matriz[step.x, step.y] = logica.estado;
			*/
            Debug.Log("Pos: " + step.x + " " + step.y );
       		yield return new WaitForSecondsRealtime(0.5f);
       	};
    }

    private void colocaCoche(int casilla, int coche)
    {
        if (inicial)
        {
            for (int i = 0; i < 3; i++)
            {
                Transform cochePos = Piezas.GetChild(i);
                Coches[i].transform.position = cochePos.position;
            }
        }
        else
        {
            Transform cochePos = Piezas.GetChild(casilla);
            Coches[coche].transform.position = cochePos.position;
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

}
