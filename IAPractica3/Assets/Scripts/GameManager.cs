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

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};

public class GameManager : MonoBehaviour {

    //Direcciones
    Pair<int, int>[] dirs = new Pair<int, int>[] {
                    new Pair<int, int>( -1, 0 ), new Pair<int, int>(0, -1 ),
                    new Pair<int, int>( 1, 0 ),new Pair<int, int>( 0, 1 ) };
    Pair<int, int>[] dirs8 = new Pair<int, int>[] {
                    new Pair<int, int>( -2, 0 ), new Pair<int, int>(0, -2 ),
                    new Pair<int, int>( 2, 0 ),new Pair<int, int>( 0, 2 ),
                    new Pair<int, int>( 1, 1 ),new Pair<int, int>( -1, -1 ),
                    new Pair<int, int>( -1, 1 ),new Pair<int, int>( 1, -1 )};

    //Parte Gráfica
    [SerializeField]
    private Transform assetsField;
    public GameObject [] assets = new GameObject [6];
    public Image Fondo;
    estadoJuego eJuego = estadoJuego.cadaver;
    int sangreNum = 0;


	//Lógica interna
	System.Random rn = new System.Random();
	private int tam = 10;
	public Seleccion Seleccion_ = Seleccion.none;
	public Casilla [,] matriz;
	Transform Piezas;

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
        for (int i = 0; i < tam; i++)
            for (int j = 0; j < tam; j++)
                matriz[i, j] = new Casilla();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("escape"))
            Application.Quit();
    }

	public void SetPiezas(Transform mapField){
		Piezas = mapField;
    }

	public int dameTam(){
		return tam;	
	}

	bool colocaFoso (Pos p){
		return (matriz[p.i , p.j].terreno != eTerreno.agujero &&
			matriz[p.i, p.j].contenido == eCadaver.nada && !(p.i == 0 && p.j == tam -1) );
	}
    bool compruebaRango(Pos p , Pair<int,int> par) {
        return p.i + par.First < tam && p.i + par.First >= 0 &&
                    p.j + par.Second < tam && p.j + par.Second >= 0;
    }


	public bool Bloqueado(){
		return Seleccion_ == Seleccion.none;
	}

    public void colocaAsset(Transform t, eCadaver e) {
        Transform childT;
        if (e == eCadaver.sangre)
        {
            childT = assetsField.GetChild((int)e + sangreNum);
            sangreNum++;
        }
        else {
            childT = assetsField.GetChild((int)e);
        }
        childT.transform.position = t.position;
    }

    void actualizaTablero() {
        for (int i = 0; i < tam * tam; i++) {
            GameObject tile = GameObject.Find(i.ToString());
            TilePR3 tileScript = tile.GetComponent<TilePR3>();
            tileScript.actualiza(matriz[i % tam, i / tam]);
        }
        sangreNum = 0;
    }

	public int Seleccionado(Casilla estado){
        if (eJuego == estadoJuego.cadaver)
        {
			matriz[estado.Posicion.i, estado.Posicion.j].contenido = eCadaver.cadaver;
            foreach (Pair<int, int> dir in dirs)
            {
                if(compruebaRango(estado.Posicion,dir))
					matriz[(int)estado.Posicion.i + dir.First, (int)estado.Posicion.j + dir.Second].contenido = eCadaver.sangre;
            }
            bool encontrado = false;
            int rnd = -1;
            while (!encontrado)
            {
                rnd = rn.Next(0, 8);
                if (compruebaRango(estado.Posicion,dirs8[rnd]))
                    encontrado = true;
            }
			matriz[(int)estado.Posicion.i + dirs8[rnd].First, (int)estado.Posicion.j + dirs8[rnd].Second].contenido = eCadaver.arma;
            //Actualizar tablero
            actualizaTablero();
            eJuego = estadoJuego.agujero;

        }
		else if (eJuego == estadoJuego.agujero) {
			if (colocaFoso(estado.Posicion)) {
				matriz [estado.Posicion.i, estado.Posicion.j].terreno = eTerreno.agujero;
				foreach (Pair<int, int> dir in dirs) {
					if (compruebaRango (estado.Posicion, dir)
					   && matriz [estado.Posicion.i + dir.First, estado.Posicion.j + dir.Second].terreno != eTerreno.agujero)
						matriz [estado.Posicion.i + dir.First, estado.Posicion.j + dir.Second].terreno = eTerreno.barro;
				}
				//ActualizaTablero
				actualizaTablero ();
			}
        }
        return 0;
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
