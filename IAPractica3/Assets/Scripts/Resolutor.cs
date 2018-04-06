using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

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




public class Nodo :
    Priority_Queue.FastPriorityQueueNode
{
    public Pos Posicion;
    public int H;//Coste heurístico Manhattan
    public int G;//Coste de la casilla (+ coste del padre)
    public int g;//Coste original de la casilla
    public int F;//Coste total
    public Nodo padre;
    public bool noPasar = false;
}


public class Resolutor
{

    //Direcciones para acceder más rápido a las casillas adyacentes
    Pair<int, int>[] dirs = new Pair<int, int>[] {
                    new Pair<int, int>( -1, 0 ), new Pair<int, int>(0, -1 ),
                    new Pair<int, int>( 1, 0 ),new Pair<int, int>( 0, 1 ) };

    Casilla[,] estados;
    Nodo[,] tablero;
    Pos destino, origen;
    public List<Pos> camino;
    public bool imposible = false;

    ///Este es el método que resuelve el problema y devuelve una lista con todas las casillas por las que tiene que pasar
    ///desde el origen hasta el destino, utilizando el algoritmo A*
    public Resolutor(Casilla[,] estado, Pos posOrigen, Pos posDestino)
    {
        //Este será el camino que rellenaremos al final
        camino = new List<Pos>();
        tablero = new Nodo[10, 10];
        estados = estado;
        destino = posDestino;
        origen = posOrigen;

        //Cola de prioridad para los nodos adyacentes no checkeados
        //Lista con los nodos ya vistos y por los que no hay que volver a pasar
        List<Nodo> aCheckear = new List<Nodo>();
        //List<Nodo> aCheckear = new List<Nodo>();
        List<Nodo> vistos = new List<Nodo>();

        CrearNodos();

        //Cogemos la casilla inicial y se mete a la lista de no chequeados

        Nodo inicio = tablero[origen.j, origen.i];
        inicio.padre = inicio;
        inicio.G = inicio.g = 0;
        inicio.F = inicio.G + inicio.H;

        aCheckear.Add(inicio);

        bool encontrado = false;

        while (aCheckear.Count > 0 && !encontrado)
        {
            //Cogemos el nodo de menor F de la lista, lo quitamos de ella y lo metemos en los ya checkeados
            int Pos = 0;
            int valorMax = 10000000;
            for (int i = 0; i < aCheckear.Count; i++){
                if (aCheckear[i].F < valorMax){
                    valorMax = aCheckear[i].F;
                    Pos = i;
                }
            }
            Nodo actual = aCheckear[Pos];
            // Nodo actual = aCheckear.Dequeue();
            aCheckear.Remove(actual);
            vistos.Add(actual);

            //Se miran los adyacentes y se les coloca el nodo actual como padre
            //se les pone como valor G su valor actual sumado al de su padre y se calcula su nuevo F
            foreach (Pair<int, int> dir in dirs) {
                //Comprobamos si el adyacente está en el tablero
                Pos nuevaPos = new Pos(); nuevaPos.Set(actual.Posicion.j + dir.First, actual.Posicion.i + dir.Second);
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

    private bool isGoal(Pos A)
    {
        return A.j == destino.j && A.i == destino.i;
    }
    private int costManhattan(int i, int j)
    {
        return Mathf.Abs(destino.i - j) + Mathf.Abs(destino.j - i);
    }

    //Relleno los costes de cada casilla con la info de sus estados
    // Y relleno los costes heurísticos (Manhattan) de cada casilla sabiendo el destino
    private void CrearNodos()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Nodo node = new Nodo();
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
        Nodo auxiliar = tablero[destino.x, destino.y];
        camino.Add(auxiliar.Posicion);
        do
        {
            auxiliar = auxiliar.padre;
            camino.Add(auxiliar.Posicion);
        } while (auxiliar.padre != auxiliar);
        //Lo invertimos y lo devolvemos
        camino.Reverse();
    }

    private bool dentroTablero(dim Pos)
    {
        return Pos.x >= 0 && Pos.x < 10 && Pos.y >= 0 && Pos.y < 10;
    }

    private void copyMatrix(eCasilla[,] origen, eCasilla[,] destino)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                destino[i, j] = origen[i, j];
            }
        }
    }
}*/