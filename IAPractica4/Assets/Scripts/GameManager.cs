// David González Jiménez
// Patricia Cabrero Villar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Jackyjjc.Bayesianet;

public enum Seleccion{ none, R, G, B};
public enum estadoJuego { colocaheroe, coloca, simula};
public enum estadoHeroe {normal,huir};

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
	public UnityEngine.UI.Text estadisticas, Resultado;


	estadoJuego eJuego = estadoJuego.colocaheroe;


	//Lógica interna
	System.Random rn = new System.Random();
	private int tam = 10;
	TilePR3 [,] matriz;
	Transform Piezas;
	public int nZombies, nSoldados = 1;
	public bool luz = true;
    bool finDelJuego = false;
    turno turn = turno.turnoZombie;
    int contador = 0;
	int puntuacion=0;

	List<TilePR3> zombies = new List<TilePR3>();
	List<TilePR3> soldados = new List<TilePR3>();
    List<TilePR3> luchas = new List<TilePR3>();

	estadoHeroe heroeEstado = estadoHeroe.normal;
	Pos heroe;

	public void addTile(int i, int j, TilePR3 tile){
		matriz [i, j] = tile;
	}
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
	private static GameManager instance;
	public static GameManager Instance{
		get{
			if(instance== null)
				instance = FindObjectOfType<GameManager>();
			return instance;
		}

	}

	// Use this for initialization
	void Awake () {
		heroe = new Pos ();
		matriz = new TilePR3[tam,tam];
		StartBayesian ();
    }

    enum turno { turnoZombie, lucha1, turnoHeroe, lucha2};

	// Update is called once per frame
	void Update () {
        contador++;
		if (finDelJuego) {
			mensaje ("FIN DEL JUEGO");
		}

		if (eJuego == estadoJuego.simula && contador % 20 == 0 && !finDelJuego) {
			if (turn == turno.turnoZombie) {
				turnoZombie ();
				turn = turno.lucha1;
			} else if (turn == turno.lucha1) {
				lucha ();
				actualizaZombisSoldados ();
				turn = turno.turnoHeroe;
			} else if (turn == turno.turnoHeroe) {
				turnoHeroe ();
				turn = turno.lucha2;
			} else if (turn == turno.lucha2) {
				lucha ();
				actualizaZombisSoldados ();
				turn = turno.turnoZombie;
			}
			if (nZombies == 0) {
				heroeEstado = estadoHeroe.huir;
			}
            mensaje("nZombies: " + nZombies + " nSoldados: " + nSoldados);
			Resultado.text =  ("Resultado: " + puntuacion);
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


	public void colocaHeroe(Transform t, Pos posicion) {
        Transform childT;
        childT = assetsField.GetChild(1);
		childT.transform.position = t.position;
		heroe = posicion;
    }

    void actualizaTablero() {
        for (int i = 0; i < tam * tam; i++) {
            GameObject tile = GameObject.Find(i.ToString());
            TilePR3 tileScript = tile.GetComponent<TilePR3>();
			tileScript.actualiza(matriz[i % tam, i / tam].estado);
			if (matriz[i % tam, i / tam].heroe) {
				tileScript.heroeColoc();
			}
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
			mensaje ("COLOCA SOLDADOS Y ZOMBIES");

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
			mensaje ("nZombies: "+ nZombies + "    nSoldados: "+ nSoldados);
		}

        return 0;
	}

    public void actualizaZombisSoldados() {
        zombies.Clear();
        soldados.Clear();
        for (int i = 0; i < tam; i++)
        {
            for (int j = 0; j < tam; j++)
            {
                if (matriz[i, j].estado.terreno == eTerreno.zombi)
                    zombies.Add(matriz[i, j]);
                else if (matriz[i, j].estado.terreno == eTerreno.soldado || matriz[i, j].estado.terreno == eTerreno.heroe)
                    soldados.Add(matriz[i, j]);
            }
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
            if (movNecesario.First != 0 || movNecesario.Second != 0)
                if (movNecesario.Second == 0)
                {
                    if (movNecesario.First < 0)
                    {
                        //SE MUEVE UNO A LA IZQUIERDA
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie--;
                        matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].nZombie++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        if (matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].heroe || matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].soldado)
                        {
                            matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                            luchas.Add(matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j]);
                        }
                        else
                            matriz[zom.estado.Posicion.i - 1, zom.estado.Posicion.j].estado.terreno = eTerreno.zombi;

                    }
                    else
                    {
                        //SE MUEVE A LA DERECHA
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie--;
                        matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].nZombie++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        if (matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].heroe || matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].soldado)
                        {
                            matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                            luchas.Add(matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j]);
                        }
                        else
                            matriz[zom.estado.Posicion.i + 1, zom.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                }
                else
                {
                    if (movNecesario.Second < 0)
                    {
                        //SE MUEVE UNO ARRIBA
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie--;
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1].nZombie++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        else
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1].heroe || matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1].soldado)
                        {
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1].estado.terreno = eTerreno.lucha;
                            luchas.Add(matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1]);
                        }
                        else
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j - 1].estado.terreno = eTerreno.zombi;
                    }
                    else
                    {
                        //SE MUEVE ABAJO
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie--;
                        matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].nZombie++;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].nZombie == 0)
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.normal;
                        if (matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].heroe || matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].soldado)
                        {
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].estado.terreno = eTerreno.lucha;
                            luchas.Add(matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1]);
                        }
                        else
                            matriz[zom.estado.Posicion.i, zom.estado.Posicion.j + 1].estado.terreno = eTerreno.zombi;
                    }
                }
            //HAY LUCHAAAA
            else
                matriz[zom.estado.Posicion.i, zom.estado.Posicion.j].estado.terreno = eTerreno.lucha;
            
			actualizaTablero ();
		}
			
	}
	public int costManhatan(Pos ini, Pos fin){
		return Mathf.Abs(ini.j - fin.j) + Mathf.Abs(ini.i - fin.i);
	}

	public bool lucha (){
        
        foreach (TilePR3 fight in luchas) {
            int r = rn.Next(0,10);
            int prob;
            if (luz) prob = 0;
            else prob = -1;
            //3 o mas soldados
            if (nSoldados >= 3) {
                //Gana el soldado
                if (r < 9 + prob)
                {
					puntuacion++;
                    nZombies--;
                    matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].nZombie--;
					if (fight.nZombie == 0 && fight.soldado) {
						matriz [fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.soldado;
					} else if (fight.nZombie == 0 && !fight.soldado) {
						matriz [fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.normal;
					}
					if(!fight.soldado)puntuacion += 4;
                }
                //Gana el zombi
                else {
                    nSoldados--;
					puntuacion -= 10;
                    if (fight.heroe && !fight.soldado)
                    {
						puntuacion -= 40;
                        finDelJuego = true;
                        return false;
                    }
                    else if (fight.soldado && !fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                    else if (fight.soldado && fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                    }
                    else
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].heroe = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                }
            }
            //2 soldados
            else if (nSoldados == 2) {
                //Gana el soldado
                if (r < 5 + prob)
                {
                    nZombies--;
                    matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].nZombie--;
                    if (fight.nZombie == 0 && fight.soldado)
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.soldado;
                    else if (fight.nZombie == 0 && !fight.soldado)
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.normal;
					puntuacion++;
					if(!fight.soldado)puntuacion += 4;
                }
                //Gana el zombi
                else
                {
					puntuacion -= 10;
                    nSoldados--;
                    if (fight.heroe && !fight.soldado)
                    {
						puntuacion -= 40;
                        finDelJuego = true;
                        return false;
                    }
                    else if (fight.soldado && !fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                    else if (fight.soldado && fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                    }
                    else
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].heroe = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                }

            }
            //Solo el heroe
            else if (nSoldados == 1) {
                //Gana el soldado
                if (r < 2 + prob)
                {
                    nZombies--;
                    matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].nZombie--;
                    if (fight.nZombie == 0 && fight.soldado)
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.soldado;
                    else if (fight.nZombie == 0 && !fight.soldado)
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.normal;
					puntuacion++;
					if(!fight.soldado)puntuacion += 4;
                }
                //Gana el zombi
                else
                {
					puntuacion -= 10;
                    nSoldados--;
                    if (fight.heroe && !fight.soldado)
                    {
						puntuacion -= 40;
                        finDelJuego = true;
                        return false;
                    }
                    else if (fight.soldado && !fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                    else if (fight.soldado && fight.heroe)
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.lucha;
                    }
                    else
                    {
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].heroe = false;
                        matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
                    }
                }
            }
            //No hay soldados (éste es por si mas de un zombi cae a la vez en la misma casilla)
            else {
                finDelJuego = true;
                matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].soldado = false;
                matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].heroe = false;
                matriz[fight.estado.Posicion.i, fight.estado.Posicion.j].estado.terreno = eTerreno.zombi;
            }
        }
        luchas.Clear();
        actualizaTablero();
        return true;
	}
	public void turnoHeroe (){
		if (estadoHeroe.normal == heroeEstado) {
			int decision = MakeDecision ();
			if (decision == 0) {//Matar
				matar();
			} else if (decision == 1) {// Huir
				heroeEstado = estadoHeroe.huir;
				print ("huir");
			}
		} else
			goHome ();
	}
	public void matar(){

		int distancia = 100;
		Pos posZombie= new Pos();
		Casilla zombieCercano = new Casilla(); 

			//BUSCO SOLDADO CERCANO
		foreach (TilePR3 zom in zombies) {
			posZombie = zom.estado.Posicion;
			int distAux = costManhatan (posZombie, heroe);
			if (distancia > distAux) {
				zombieCercano = zom.estado;
				distancia = distAux;
			}
		}
		//AVANZO Y COMPRUEBO SI LUCHA
		Pair<int,int> movNecesario = new Pair<int, int>(zombieCercano.Posicion.i - heroe.i, zombieCercano.Posicion.j - heroe.j);
		if(mueveHeroe (movNecesario))
			
		actualizaTablero ();
	}
	public void goHome(){
		Pair<int,int> movNecesario = new Pair<int, int>(0 - heroe.i, 9 - heroe.j);
		finDelJuego = mueveHeroe (movNecesario);
		actualizaTablero ();
	}
	public bool mueveHeroe(Pair<int,int> movNecesario){
		if (movNecesario.First != 0 || movNecesario.Second != 0) {
			matriz [heroe.i, heroe.j].heroe = false;

			if (movNecesario.Second == 0) {
				if (movNecesario.First < 0) {
					//SE MUEVE UNO A LA IZQUIERDA
					matriz [heroe.i - 1, heroe.j].heroe = true;
					heroe.i = heroe.i - 1;
				} else {
					//SE MUEVE A LA DERECHA
					matriz [heroe.i + 1, heroe.j].heroe = true;
					heroe.i = heroe.i +1;
				}
			} else {
				if (movNecesario.Second < 0) {
					//SE MUEVE UNO ARRIBA
					matriz [heroe.i, heroe.j - 1].heroe = true;
					heroe.j  = heroe.j -1;

				} else {
					//SE MUEVE ABAJO
					matriz [heroe.i, heroe.j + 1].heroe = true;
					heroe.j = heroe.j+1;
				}
			}
			return false;
		} else
			return true;
	}
	public void resuelve() {
		eJuego = estadoJuego.simula;
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



	///////////////////////////////////////////////////
	/////////////// BAYESIAN RED /////////////////////
	/////////////////////////////////////////////////
	private VariableElimination ve;
	// Use this for initialization
	void StartBayesian () {
		string networkJson = (Resources.Load("enemy_logic_json") as TextAsset).text;
		ve = new VariableElimination(new BayesianJsonParser().Parse(networkJson));
	}

	public Text decisionText;
	public Text probabilityText;

	public int MakeDecision()
	{
		//////SITUACION////////////

		// Observaciones Situación
		List<string> observations = new List<string> {
			"CantidadAliados=" + GetAllyAmount(),
			"CantidadEnemigos=" + GetEnemyAmount(),
		};

		// Distribucion de la situacion
		double[] Distribution = ve.Infer("Situacion", observations);
		double SituacionI = ve.PickOne(Distribution);
		string Situacion = GetSituacion(SituacionI);



		//////DESTREZA////////////
		// Observaciones Destreza
		List<string> observationsSkill = new List<string> {
			"Luz=" + GetLight(),
			"CantidadAliados=" + GetAllyAmount(),
		};

		//Se mete en las observaciones la destreza
		double[] SkillDistribution = ve.Infer("Destreza", observationsSkill);
		double skillI = ve.PickOne (SkillDistribution);
		string skill = GetSkill (skillI);



		//////ESTADO////////////
		// Observaciones ESTADO
		List<string> observationsStateA = new List<string> {
			"Situacion="+ Situacion,
			"Destreza=" + skill,
			"Accion=Avanzar"
		};
		//Se mete en las observaciones la ESTADO
		double[] DistAvanzar = ve.Infer("Estado", observationsStateA);

		List<string> observationsStateR = new List<string> {
			"Situacion="+ Situacion,
			"Destreza=" + skill,
			"Accion=Retroceder"
		};
		double[] DistRetroceder = ve.Infer("Estado", observationsStateR);


		List<string> observationsStateW = new List<string> {
			"Situacion="+ Situacion,
			"Destreza=" + skill,
			"Accion=Esperar"
		};
		double[] DistEsperar = ve.Infer("Estado", observationsStateW);


		double Avanzar = DistAvanzar[0] * 1.0 + DistAvanzar[1] * 0.7 + DistAvanzar[2] * 0.0;
		double Retroceder = DistRetroceder[0] * 1.0 + DistRetroceder[1] * 0.7 + DistRetroceder[2] * 0.0;
		double Esperar = DistEsperar[0] * 1.0 + DistEsperar[1] * 0.7+ DistEsperar[2] * 0.0;

		double decision = System.Math.Max(Avanzar,Retroceder);
		decision = System.Math.Max (decision, Esperar);

		if (Avanzar == decision)
			return 0;
		else if (Retroceder == decision)
			return 1;
		else
			return 2;
	}

	private string GetSkill(double Index){
		string result="";
		if(Index == 0)
			result = "Buena";
		else if(Index== 1.0)
			result = "Regular";
		else if(Index ==2.0)
			result = "Mala";
		return result;
	}
	private string GetSituacion(double Index){
		string result = " ";
		if(Index == 0)
			result = "muchoZombie";
		else if(Index == 1)
			result = "muchoAliado";
		else if(Index == 2)
			result = "neutral";
		return result;
	}
	// you can map continuous values into discrete ones
	private string GetEnemyAmount()
	{
		string result;
		if (nZombies == 0) result = "Ninguno";
		else if (nZombies <= 5) result = "Pocos";
		else result = "Muchos";
		return result;
	}

	private string GetLight()
	{
		return luz.ToString ();
	}
	private string GetAllyAmount()
	{
		string result;
		if (nSoldados == 1) result = "Ninguno";
		else if (nSoldados == 2) result = "Uno";
		else result = "Mas";
		return result;
	}
}
