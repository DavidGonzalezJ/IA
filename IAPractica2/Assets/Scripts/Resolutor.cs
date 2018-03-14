using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Resolutor : MonoBehaviour {

    //Direcciones para acceder más rápido a las casillas adyacentes
    Pair<int, int>[] dirs = new Pair<int, int>[]{new Pair<int, int>( -1, 0 ), new Pair<int, int>(0, -1 ), new Pair<int, int>( 1, 0 ),new Pair<int, int>( 0, 1 ) };


    ///Este es el método que resuelve el problema y devuelve una lista con todas las casillas por las que tiene que pasar
    ///desde el origen hasta el destino, utilizando el algoritmo A*
    public List<casilla> caminoAstarManhattan(casilla origen, casilla destino) {
        //Este será el camino que rellenaremos al final
        List<casilla> camino = new List<casilla>();
        casilla[,] tablero = new casilla[10,10];
        eCasilla[,] estados = PuzzleManager.Instance.matriz;
        //Relleno los costes de cada casilla con la info de sus estados
        // Y relleno los costes heurísticos (Manhattan) de cada casilla sabiendo el destino
        int g;
        dim posDestino = new dim();
        posDestino.Set( destino.Posicion.x, destino.Posicion.y );
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                g = (int)estados[i, j] + 1;
                tablero[i, j].g = g;//Coste casilla
                tablero[i, j].G = g;
                if (g > 2) tablero[i, j].noPasar = true;
                tablero[i, j].H = Mathf.Abs(posDestino.y - i) + Mathf.Abs(posDestino.x - j);//Coste heurístico
            }
        }
        //Hacemos una cola de prioridad para los nodos adyacentes no checkeados
        //Y una lista con los nodos ya vistos y por los que no hay que volver a pasar
        List<casilla> aCheckear = new List<casilla>();
        List<casilla> vistos = new List<casilla>();
        //Cogemos la casilla inicial y se mete a la lista de no chequeados
        dim posOrigen= new dim();
        posOrigen.Set(origen.Posicion.x, origen.Posicion.y);
        casilla inicio = tablero[posOrigen.x, posOrigen.y];
        inicio.padre = inicio;
        inicio.G = inicio.g = 0;
        inicio.F = inicio.G + inicio.H;
        aCheckear.Add(inicio);

        bool encontrado = false;
        //Ojo aquí que viene lo gordo
        while (aCheckear.Count > 0 && !encontrado) {
            //Cogemos el nodo de menor F de la lista, lo quitamos de ella y lo metemos en los ya checkeados
            int pos = 0;
            int valorMax = 10000000;
            for (int i = 0; i < aCheckear.Count; i++) {
                if (aCheckear[i].F < valorMax) {
                    valorMax = aCheckear[i].F;
                    pos = i;
                }
            }
            casilla actual = aCheckear[pos];
            aCheckear.Remove(actual);
            vistos.Add(actual);

            //Se miran los adyacentes y se les coloca el nodo actual como padre
            //se les pone como valor G su valor actual sumado al de su padre y se calcula su nuevo F
            foreach (Pair<int, int> dir in dirs)
            {
                //Comprobamos si el adyacente está en el tablero
                dim nuevaPos = new dim(); nuevaPos.Set(actual.Posicion.x + dir.First, actual.Posicion.y + dir.Second);
                if (nuevaPos.x >= 0 &&
                   nuevaPos.x < 10 &&
                   nuevaPos.y >= 0 &&
                   nuevaPos.y < 10) {
                    //Comprobamos si el adyacente ya está entre los vistos
                    if (!vistos.Contains(tablero[nuevaPos.x, nuevaPos.y]))
                    {
                        //Comprobamos si el adyacente es una casilla bloqueada
                        if (tablero[nuevaPos.x, nuevaPos.y].noPasar == true)
                            vistos.Add(tablero[nuevaPos.x, nuevaPos.y]);
                        //Si no lo es, calculamos sus nuevos valores y lo metemos en la lista de pendientes
                        else {
                            //Comprobamos si es nuestro destino y si no, seguimos
                            if (nuevaPos.x == posDestino.x && nuevaPos.y == posDestino.y)
                            {
                                encontrado = true;
                                tablero[nuevaPos.x, nuevaPos.y].padre = actual;
                            }
                            else
                            {
                                //Miramos si es un camino menos costoso que el anterior
                                int nuevoG = actual.G + tablero[nuevaPos.x, nuevaPos.y].g;
                                if (nuevoG < tablero[nuevaPos.x, nuevaPos.y].G && aCheckear.Contains(tablero[nuevaPos.x, nuevaPos.y]))
                                {
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
        }
        //Una vez tenemos los padres de cada nodo, creamos el camino
        casilla auxiliar = tablero[posDestino.x, posDestino.y];
        camino.Add(auxiliar);
        do
        {
            auxiliar = auxiliar.padre;
            camino.Add(auxiliar);
        } while (auxiliar.padre != auxiliar);
        //Lo invertimos y lo devolvemos
        camino.Reverse();
        return camino;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
