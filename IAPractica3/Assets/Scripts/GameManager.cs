// David González Jiménez
// Patricia Cabrero Villar


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public enum Seleccion{ none, R, G, B};
public enum estadoJuego { cadaver, agujero, explora};

public class GameManager : MonoBehaviour {

	//Parte Gráfica
	public GameObject [] Flechas = new GameObject [3];
    public Image Fondo;
    estadoJuego eJuego = estadoJuego.cadaver;

	private Transform Piezas;

	//Lógica interna
	private int tam = 10;
	public Seleccion Seleccion_ = Seleccion.none;
	private Pos pSeleccion_ = new Pos();
	public Casilla [,] matriz;

    public Color[] colores = new Color[3];

	//Esto es para instanciar al manager desde cualquier script
	//EJ:PuzzleManager.Instance.Seleccionado()
	private static GameManager instance;
	public static GameManager Instance{
		get{
			if(instance== null)
				instance = FindObjectOfType<GameManager>();
			return instance;
		}

	}
	// Use this for initialization
	void Start () {
		matriz = new Casilla[tam,tam];

        /*for (int i = 0; i < 3; i++) {
            matriz[i, 0] = (Casilla)(i + 3);
        }

        Transform pieza = Piezas.GetChild(0);
        TilePR2 logica = pieza.GetComponent<TilePR2>();
        logica.avanza(0);
        matriz[0, 0] = Casilla.obsR;

        pieza = Piezas.GetChild(9);
        logica = pieza.GetComponent<TilePR2>();
        logica.avanza(1);
        matriz[9, 0] = eCasilla.obsG;


        pieza = Piezas.GetChild(99);
        logica = pieza.GetComponent<TilePR2>();
        logica.avanza(2);
        matriz[9, 9] = eCasilla.obsB;

        Fondo.color = colores[0];*/
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("escape"))
            Application.Quit();
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
		//StartCoroutine(flechaDelay(flechaS));

	}
    /*
	public void GoTo(Pos Posicion){

		//Llama al método resolutor con esa posición y la posicion del elemento seleccionado
		int coche = (int)(Seleccion_) - 1;

		Resolutor resolutor = new Resolutor(matriz, pSeleccion_, Posicion);
        if(!resolutor.imposible)
            StartCoroutine(resolver(resolutor.camino, pSeleccion_ , Posicion, coche));

		//Quita la selección
		Seleccion_ = Seleccion.none;
	}*/

	public bool Bloqueado(){
		return Seleccion_ == Seleccion.none;
	}
	public int Seleccionado(Pos Posicion, Casilla estado){
		matriz[(int)Posicion.j,(int)Posicion.i] = estado;


        /*if((int)estado.terreno > 2){//Es uno de los coches
            if (Seleccion_ == (Seleccion)((int)estado - 2)) Seleccion_ = Seleccion.none;
            else
            {
                Seleccion_ = (Seleccion)((int)estado - 2);
                pSeleccion_.Set(Posicion.x, Posicion.y);
            }
		}else if(estado == eCasilla.bloqueado && Seleccion_ != Seleccion.none) Seleccion_ = Seleccion.none;

        Fondo.color = colores[(int)Seleccion_];
        return (int)(Seleccion_) - 1;*/
        return 0;
	}

  /*  IEnumerator resolver(List<dim> camino, dim origen, dim destino, int coche) {

		Transform pieza = Piezas.GetChild(origen.x + origen.y*10);
        TilePR2 logica = pieza.GetComponent<TilePR2>();
        Image tileColor = pieza.GetComponent<Image>();
        tileColor.color = colores[coche+1];

        bool avanzar = true;
        int i;

        for (i = 1; avanzar && i <= camino.Count; i++) {
            logica.vuelve();
            matriz[camino[i - 1].x, camino[i - 1].y] = logica.estado;
            float aux = (float)matriz[camino[i - 1].x, camino[i - 1].y] + 1;
            pieza = Piezas.GetChild(camino[i - 1].x + camino[i - 1].y * 10);
            logica = pieza.GetComponent<TilePR2>();
            tileColor = pieza.GetComponent<Image>();
            tileColor.color = colores[coche+1];
            avanzar = logica.avanza(coche);

            if (!avanzar)
            {
                pieza = Piezas.GetChild(camino[i - 2].x + camino[i - 2].y * 10);
                logica = pieza.GetComponent<TilePR2>();
                logica.avanza(coche);
                matriz[camino[i - 2].x, camino[i - 2].y] = logica.estado;
                aux = 0;
            }
            yield return new WaitForSecondsRealtime(0.5f * aux);
        }
        actualizaTablero(camino);
        if(!avanzar)recalcula(camino[i - 3], destino, coche);
    }
    IEnumerator espera(float aux) {
        yield return new WaitForSecondsRealtime(0.5f * aux);
    }

    IEnumerator flechaDelay(int flecha) {
       //Ahora las aplico en plan bonito
       yield return new WaitForSecondsRealtime(2.5f);
       Flechas[flecha].SetActive(false);
    }

    private void actualizaTablero(List<dim> camino)
    {
        for (int i = 0; i < 100; i++)
        {
            int x = i % 10;
            int y = i / 10;
            Transform pieza = Piezas.GetChild(i);
            TilePR2 logica = pieza.GetComponent<TilePR2>();
            matriz[x, y] = logica.estado;
        }
        for (int i = 0; i < camino.Count; i++)
        {
            Transform pieza = Piezas.GetChild(camino[i].x + camino[i].y * 10);
            Image tileColor = pieza.GetComponent<Image>();
            tileColor.color = new Color(255, 255, 255, 255);

        }
    }
    */
    public void reinicia() {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Salir() {
        Application.Quit();
    }
}
