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
public enum estadoJuego { coloca, simula};

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

	estadoJuego eJuego = estadoJuego.coloca;
    int sangreNum = 0;
	public UnityEngine.UI.Text estadisticas;



	//Lógica interna
	System.Random rn = new System.Random();
	private int tam = 10;
	Casilla [,] matriz;
	Transform Piezas;

    public void getMatriz(ref Casilla[,] m) {
        for (int i = 0; i < tam; i++)
            for (int j = 0; j < tam; j++) {
                m[i, j].contenido = matriz[i, j].contenido;
                m[i, j].terreno = matriz[i, j].terreno;
                m[i, j].Posicion = matriz[i, j].Posicion;
            }
    }
	public bool Simulando(){
		return eJuego == estadoJuego.simula;
	}
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
            {
                matriz[i, j] = new Casilla();
                matriz[i, j].Posicion.Set(i, j);
            }
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
			mensaje ("COLOCA LAS TRAMPAS ;)");

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

    public void resuelve() {
		if (estadoJuego.agujero == eJuego) {
			Agente agent = new Agente ();
			StartCoroutine (avanzaAgente (agent));
			eJuego = estadoJuego.explora;
			mensaje ("INSPECCIONANDO TERRENO");
		}

    }

    IEnumerator avanzaAgente(Agente p) {
        while (!p.muerte && !p.completado)
        {
            Pos pos = p.IA_agente();
            if (pos != null)
            {
                Transform childT;
                childT = assetsField.GetChild(7);
                childT.transform.position = Piezas.GetChild(pos.j * tam + pos.i).position;
                Debug.Log(pos.i + " " + pos.j);
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }
        if (p.completado) {
			mensaje ("VOLVEMOS A CASA");
            //Llamamos a la vuelta a casa
			goHome(p.dameMatrizAgente(), p.dameCasillaAgente());
        }
        if (p.muerte) {
            //Nos morimos
			mensaje ("La próxima vez será! :(");
        }
    }
	public void goHome(int[,]casillas, Pos posI){
		Pos posD = new Pos( 0, tam - 1);
		Resolutor resolutor = new Resolutor(casillas, posI, posD);
		StartCoroutine(home(resolutor.camino, posI , posD));
	}

	IEnumerator home(List<Pos> camino, Pos origen, Pos destino) {
			//Mirar camino mover bicho
		for (int i = 1; i <= camino.Count-1; i++) {
			Debug.Log (camino [i].i + " " + camino [i].j);
			Transform childT;
			childT = assetsField.GetChild (7);
			Transform aux = Piezas.GetChild (camino [i].j * tam + camino [i].i);
			if(aux != null)
				childT.transform.position = aux.position;
			yield return new WaitForSecondsRealtime (0.5f);
		}
    }

	public void mensaje(string Mensaje){
		estadisticas.text = Mensaje;
	}
    public void reinicia() {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Salir() {
        Application.Quit();
    }
}
