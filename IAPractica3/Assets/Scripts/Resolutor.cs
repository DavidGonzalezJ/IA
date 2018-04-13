using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dim{
	public int x,y;
	public dim(){}
	public dim (int xI, int yI){
		x=xI;
		y=yI;
	}
	public void Set(int xI, int yI){
		x=xI;
		y=yI;
	}
}

public class NodoA :
Priority_Queue.FastPriorityQueueNode
{
	public dim Posicion;
	public int H;//Coste heurístico Manhattan
	public int G;//Coste de la casilla (+ coste del padre)
	public int g;//Coste original de la casilla
	public int F;//Coste total
	public NodoA padre;
	public bool noPasar = false;
}


public class Resolutor
{

	//Direcciones para acceder más rápido a las casillas adyacentes
	Pair<int, int>[] dirs = new Pair<int, int>[] {
		new Pair<int, int>( -1, 0 ), new Pair<int, int>(0, -1 ),
		new Pair<int, int>( 1, 0 ),new Pair<int, int>( 0, 1 ) };

	int[,] estados;
	NodoA[,] tablero;
	dim destino, origen;
	public List<dim> camino;
	public bool imposible = false;

	///Este es el método que resuelve el problema y devuelve una lista con todas las casillas por las que tiene que pasar
	///desde el origen hasta el destino, utilizando el algoritmo A*
	public Resolutor(int[,] estado, dim posOrigen, dim posDestino)
	{
		//Este será el camino que rellenaremos al final
		camino = new List<dim>();
		tablero = new NodoA[10, 10];
		estados = estado;
		destino = posDestino;
		origen = posOrigen;

		//Cola de prioridad para los nodos adyacentes no checkeados
		//Lista con los nodos ya vistos y por los que no hay que volver a pasar
		List<NodoA> aCheckear = new List<NodoA>();
		//List<NodoA> aCheckear = new List<NodoA>();
		List<NodoA> vistos = new List<NodoA>();

		CrearNodoAs();

		//Cogemos la casilla inicial y se mete a la lista de no chequeados

		NodoA inicio = tablero[origen.x, origen.y];
		inicio.padre = inicio;
		inicio.G = inicio.g = 0;
		inicio.F = inicio.G + inicio.H;

		aCheckear.Add(inicio);

		bool encontrado = false;

		while (aCheckear.Count > 0 && !encontrado)
		{
			//Cogemos el nodo de menor F de la lista, lo quitamos de ella y lo metemos en los ya checkeados
			int pos = 0;
			int valorMax = 10000000;
			for (int i = 0; i < aCheckear.Count; i++){
				if (aCheckear[i].F < valorMax){
					valorMax = aCheckear[i].F;
					pos = i;
				}
			}
			NodoA actual = aCheckear[pos];
			// NodoA actual = aCheckear.Dequeue();
			aCheckear.Remove(actual);
			vistos.Add(actual);

			//Se miran los adyacentes y se les coloca el nodo actual como padre
			//se les pone como valor G su valor actual sumado al de su padre y se calcula su nuevo F
			foreach (Pair<int, int> dir in dirs) {
				//Comprobamos si el adyacente está en el tablero
				dim nuevaPos = new dim(); nuevaPos.Set(actual.Posicion.x + dir.First, actual.Posicion.y + dir.Second);
				if (dentroTablero(nuevaPos))
					//Comprobamos si el adyacente ya está entre los vistos
				if (!vistos.Contains(tablero[nuevaPos.x, nuevaPos.y])){
					//Comprobamos si el adyacente es una casilla bloqueada
					if (tablero[nuevaPos.x, nuevaPos.y].noPasar == true)
						vistos.Add(tablero[nuevaPos.x, nuevaPos.y]);
					//Si no lo es, calculamos sus nuevos valores y lo metemos en la lista de pendientes
					else {
						//Comprobamos si es nuestro destino y si no, seguimos
						if (isGoal(nuevaPos)) {
							encontrado = true;
							tablero[nuevaPos.x, nuevaPos.y].padre = actual;
						}
						else {
							//Miramos si es un camino menos costoso que el anterior
							int nuevoG = actual.G + tablero[nuevaPos.x, nuevaPos.y].g;
							if (nuevoG < tablero[nuevaPos.x, nuevaPos.y].G && aCheckear.Contains(tablero[nuevaPos.x, nuevaPos.y])) {
								tablero[nuevaPos.x, nuevaPos.y].G = nuevoG;
								tablero[nuevaPos.x, nuevaPos.y].padre = actual;
							}
							else if (!aCheckear.Contains(tablero[nuevaPos.x, nuevaPos.y]))
								tablero[nuevaPos.x, nuevaPos.y].padre = actual;
							tablero[nuevaPos.x, nuevaPos.y].F = tablero[nuevaPos.x, nuevaPos.y].G + tablero[nuevaPos.x, nuevaPos.y].H;
							aCheckear.Add(tablero[nuevaPos.x, nuevaPos.y]);
						}
					}
				}
			}
		}
		if (encontrado) calculaCamino();
		else imposible = true;
	}

	private bool isGoal(dim A)
	{
		return A.x == destino.x && A.y == destino.y;
	}
	private int costManhattan(int i, int j)
	{
		return Mathf.Abs(destino.y - j) + Mathf.Abs(destino.x - i);
	}

	//Relleno los costes de cada casilla con la info de sus estados
	// Y relleno los costes heurísticos (Manhattan) de cada casilla sabiendo el destino
	private void CrearNodoAs()
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				NodoA node = new NodoA();
				int g = (int)estados[i, j] + 1;
				if (g > 2)
				{
					node.noPasar = true;
					node.g = 100000000;//Coste casilla
				}
				else
					node.g = g;//Coste casilla
				node.G = g;
				node.Posicion = new dim(i, j);

				node.H = costManhattan(i, j);//Coste heurístico

				tablero[i, j] = node;
			}
		}
	}

	private void calculaCamino()
	{
		//Una vez tenemos los padres de cada nodo, creamos el camino
		NodoA auxiliar = tablero[destino.x, destino.y];
		camino.Add(auxiliar.Posicion);
		do
		{
			auxiliar = auxiliar.padre;
			camino.Add(auxiliar.Posicion);
		} while (auxiliar.padre != auxiliar);
		//Lo invertimos y lo devolvemos
		camino.Reverse();
	}

	private bool dentroTablero(dim pos)
	{
		return pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 10;
	}

	private void copyMatrix(int[,] origen, int[,] destino)
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				destino[i, j] = origen[i, j];
			}
		}
	}
}