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
public enum estadoJuego { colocaheroe, coloca, simula};

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
	public UnityEngine.UI.Text estadisticas;

	estadoJuego eJuego = estadoJuego.colocaheroe;


	//Lógica interna
	System.Random rn = new System.Random();
	private int tam = 10;
	TilePR3 [,] matriz;
	Transform Piezas;
	public int nZombies, nSoldados;
	public bool luz = true;
    int contador = 0;
	List<TilePR3> zombies = new List<TilePR3>();
	List<TilePR3> soldados = new List<TilePR3>();

	public void getMatriz(ref TilePR3[,] m) {
        for (int i = 0; i < tam; i++)
            for (int j = 0; j < tam; j++) {
				m[i, j].estado.terreno = matriz[i, j].estado.terreno;
				m[i, j].estado.Posicion = matriz[i, j].estado.Posicion;
            }
    }

	public bool Colocar(){
		return eJuego == estadoJuego.coloca;
	}

	public estadoJuego estado(){
		return eJuego;
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
		matriz = new TilePR3[tam,tam];
        for (int i = 0; i < tam; i++)
            for (int j = 0; j < tam; j++)
            {
				matriz[i, j] = new TilePR3();
				matriz[i, j].estado.Posicion.Set(i, j);
            }
    }
	
	// Update is called once per frame
	void Update () {
        contador++;
		if (eJuego == estadoJuego.simula && contador%60 == 0) {
			turnoZombie ();
			lucha ();
			turnoHeroe ();
			lucha ();
            contador = 0;
		}
        if (Input.GetKey("escape"))
            Application.Quit();
    }

	public void SetPiezas(Transform mapField){
		Piezas = mapField;
    }

	public int dameTam(){
		return tam;	
	}

    bool compruebaRango(Pos p , Pair<int,int> par) {
        return p.i + par.First < tam && p.i + par.First >= 0 &&
                    p.j + par.Second < tam && p.j + par.Second >= 0;
    }


	public void colocaHeroe(Transform t) {
        Transform childT;
        childT = assetsField.GetChild(1);
		childT.transform.position = t.position;
    }

    void actualizaTablero() {
        for (int i = 0; i < tam * tam; i++) {
            GameObject tile = GameObject.Find(i.ToString());
            TilePR3 tileScript = tile.GetComponent<TilePR3>();
			tileScript.actualiza(matriz[i % tam, i / tam].estado);
        }
    }
	public void changesoldados(int a){
		nSoldados += a;
	}
	public void changeZombies(int a){
		nZombies += a;
	}
	public int Seleccionado(Casilla estado, bool cambio=true){
		if (eJuego == estadoJuego.colocaheroe) {
			matriz [estado.Posicion.i, estado.Posicion.j].heroe = true;
			matriz [estado.Posicion.i, estado.Posicion.j].estado.terreno = eTerreno.heroe;

			//Actualizar tablero
			actualizaTablero ();
			eJuego = estadoJuego.coloca;
			mensaje ("COLOCA LOS SOLDADOS Y LOS ZOMBIES ;)");

		} else {
			switch (estado.terreno) {
			case eTerreno.zombi:
				matriz [estado.Posicion.i, estado.Posicion.j].nZombie = 1;
				matriz [estado.Posicion.i, estado.Posicion.j].estado.terreno = eTerreno.zombi;
				break;
			case eTerreno.soldado:
					matriz [estado.Posicion.i, estado.Posicion.j].soldado = true;
					matriz [estado.Posicion.i, estado.Posicion.j].nZombie = 0;
					matriz [estado.Posicion.i, estado.Posicion.j].estado.terreno = eTerreno.soldado;
					break;
			case eTerreno.normal:
				matriz [estado.Posicion.i, estado.Posicion.j].soldado = false;
				matriz [estado.Posicion.i, estado.Posicion.j].nZombie = 0;
				matriz [estado.Posicion.i, estado.Posicion.j].estado.terreno = eTerreno.normal;
				break;
			}
			mensaje ("nZombies: "+ nZombies + " nSoldados: "+ nSoldados);
		}

        return 0;
	}

    public void resuelve() {
		eJuego = estadoJuego.simula;

		for(int i = 0; i<tam; i++){
			for (int j = 0; j < tam; j++) {
				if (matriz [i, j].estado.terreno == eTerreno.zombi)
					zombies.Add (matriz [i, j]);
				else if (matriz [i, j].estado.terreno == eTerreno.soldado || matriz [i, j].estado.terreno == eTerreno.heroe) 
					soldados.Add (matriz [i, j]);
			}
		}
    }

    /*IEnumerator avanzaAgente(Agente p) {
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
		yield return new WaitForSecondsRealtime(0.5f);

    }*/
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
	public void turnoZombie (){
		Casilla soldadocercano = new Casilla();
		Pos posZombie = new Pos();
		Pos posSoldado = new Pos();
		int distancia = 100;
		foreach (TilePR3 zom in zombies) {
			posZombie = zom.estado.Posicion;

			//BUSCO SOLDADO CERCANO
			foreach (TilePR3 soldier in soldados) {
				posSoldado = soldier.estado.Posicion;
				int distAux = costManhatan (posZombie, posSoldado);
				if (distancia > distAux) {
					soldadocercano = soldier.estado;
					distancia = distAux;
				}
			}
            distancia = 100;
			//AVANZO Y COMPRUEBO SI LUCHA
			Pair<int,int> movNecesario = new Pair<int, int>(soldadocercano.Posicion.i- posZombie.i, soldadocercano.Posicion.j - posZombie.j);
            if(movNecesario.First != 0 || movNecesario.Second != 0)
			if (movNecesario.Second == 0) {
				if (movNecesario.First < 0) {
					//SE MUEVE UNO A LA IZQUIERDA
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j].nZombie --;
					matriz[zom.estado.Posicion.i-1,zom.estado.Posicion.j].nZombie ++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        if (matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].heroe || matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].soldado)
                            matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                        else
                            matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                        zom.estado.Posicion.i--;

				} else {
					//SE MUEVE A LA DERECHA
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j].nZombie --;
					matriz[zom.estado.Posicion.i+1,zom.estado.Posicion.j].nZombie ++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        if (matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].heroe || matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].soldado)
                            matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                        else
                            matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].estado.terreno = eTerreno.zombi;
					zom.estado.Posicion.i++;
                    }
			}else
				if (movNecesario.Second < 0) {
					//SE MUEVE UNO ARRIBA
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j].nZombie --;
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j-1].nZombie ++;
                    if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                    if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j-1].heroe || matriz[zom.estado.Posicion.i, zom.estado.Posicion.j-1].soldado)
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j-1].estado.terreno = eTerreno.lucha;
                    else
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j-1].estado.terreno = eTerreno.zombi;
					zom.estado.Posicion.j--;
                }
                else{
					//SE MUEVE ABAJO
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j].nZombie --;
					matriz[zom.estado.Posicion.i,zom.estado.Posicion.j+1].nZombie ++;
                    if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                    if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].heroe || matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].soldado)
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].estado.terreno = eTerreno.lucha;
                    else
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].estado.terreno = eTerreno.zombi;
					zom.estado.Posicion.j++;
                }
			actualizaTablero ();
		}
			
	}
	public int costManhatan(Pos ini, Pos fin){
		return Mathf.Abs(ini.j - fin.j) + Mathf.Abs(ini.i - fin.i);
	}

	public void lucha (){
		/*El soldado gana el 90% de las veces si quedan 3 o más
		soldados en juego, el 50% si sólo quedan 2, y el 20% si es
		el héroe y ya se encuentra solo
		■ Si no hay luz, los soldados tienen un -10%
		Los zombis pueden solaparse entre sí (se les combate a todos, uno por uno)
		y el héroe puede solaparse con sus aliados: si justo entonces un zombi cae en
		esa casilla, este combatirá primero con el aliado y luego con el héroe*/

	}
	public void turnoHeroe (){
		
	}

	public void toogleluz(bool toggle){
		luz = toggle;
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
