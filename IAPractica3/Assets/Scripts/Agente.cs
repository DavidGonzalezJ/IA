using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo {
    public Nodo() {
        casilla = new Casilla();
        coste = 1;
        conocida = false;
    }
    public Casilla casilla;
    public int coste;
    public bool conocida;
}

public enum estadoAgente { buscaCadaver, buscaArma, vuelve };

public class Agente{
    System.Random rnd = new System.Random();
    //Direcciones
    Pair<int, int>[] dirs = new Pair<int, int>[] {
                    new Pair<int, int>( -1, 0 ), new Pair<int, int>(0, -1 ),
                    new Pair<int, int>( 1, 0 ),new Pair<int, int>( 0, 1 ) };
    Pair<int, int>[] dirs8 = new Pair<int, int>[] {
                    new Pair<int, int>( -2, 0 ), new Pair<int, int>(0, -2 ),
                    new Pair<int, int>( 2, 0 ),new Pair<int, int>( 0, 2 ),
                    new Pair<int, int>( 1, 1 ),new Pair<int, int>( -1, -1 ),
                    new Pair<int, int>( -1, 1 ),new Pair<int, int>( 1, -1 )};

    public Agente() {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                matrizAgente[i, j] = new Nodo();
                matrizAgente[i, j].casilla.Posicion.Set(i, j);
                matrizCompleta[i, j] = new Casilla();
            }
        GameManager.Instance.getMatriz(ref matrizCompleta);
        casillaAct = matrizCompleta[0, 9];
    }

    private static Casilla[,] matrizCompleta = new Casilla[10,10];
    private Nodo[,] matrizAgente = new Nodo[10, 10];
    private Casilla casillaAct;
    private estadoAgente est = estadoAgente.buscaCadaver;
    bool armaEncontrada = false;
    public bool muerte = false;
    public bool completado = false;

    bool compruebaCasilla(Nodo c, Pair<int, int> par) {
        bool dentro = c.casilla.Posicion.i + par.First < 10 && c.casilla.Posicion.i + par.First >= 0 &&
                                c.casilla.Posicion.j + par.Second < 10 && c.casilla.Posicion.j + par.Second >= 0;
        if (dentro && matrizAgente[c.casilla.Posicion.i + par.First, c.casilla.Posicion.j + par.Second].conocida) return false;
        else {
            return dentro;
        }
    }

    void copiaCasilla(Casilla casillaAcopiar, ref Casilla casillaDestino) {
        casillaDestino.contenido = casillaAcopiar.contenido;
        casillaDestino.terreno = casillaAcopiar.terreno;
        casillaDestino.Posicion = casillaAcopiar.Posicion;
    }

    public Pos IA_agente() {
        if (!muerte)
        {
            if (est == estadoAgente.buscaCadaver)
            {
                //1. Evalúa su casilla actual y le pone coste a las adyacentes
                matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j].conocida = true;
                matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j].casilla = casillaAct;
                ////Terreno
                if (casillaAct.terreno == eTerreno.normal)
                {
                    matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j].coste++;
                }
                else if (casillaAct.terreno == eTerreno.barro)
                {
                    matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j].coste++;
                    //El coste de las adyacentes desconocidas se incrementa en 2
                    foreach (Pair<int, int> dir in dirs)
                    {
                        if (compruebaCasilla(matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j], dir))
                        {
                            matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second].coste += 2;
                        }
                    }
                }
                else if (casillaAct.terreno == eTerreno.agujero)
                {
                    muerte = true;
                }

                ////Contenido
                if (casillaAct.contenido == eCadaver.sangre)
                {
                    //El coste de las adyacentes desconocidas se decrementa en 2
                    matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j].coste -= 2;
                    foreach (Pair<int, int> dir in dirs)
                    {
                        if (compruebaCasilla(matrizAgente[casillaAct.Posicion.i, casillaAct.Posicion.j], dir))
                            matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second].coste -= 3;
                    }
                }
                else if (casillaAct.contenido == eCadaver.arma)
                {

                    armaEncontrada = true;
                }
                else if (casillaAct.contenido == eCadaver.cadaver)
                {
                    est = estadoAgente.buscaArma;
                }

                //2. Lista las casillas adyacentes y elige la de menor coste
                // Si hay varias, lo echa a suertes
                if (est == estadoAgente.buscaCadaver && !muerte)
                {
                    int costeMin = 999;
                    foreach (Pair<int, int> dir in dirs)
                    {
                        if (casillaAct.Posicion.i + dir.First < 10 && casillaAct.Posicion.i + dir.First >= 0 &&
                                casillaAct.Posicion.j + dir.Second < 10 && casillaAct.Posicion.j + dir.Second >= 0 &&
                                matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second].coste < costeMin)
                            costeMin = matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second].coste;
                    }
                    List<Nodo> l = new List<Nodo>();
                    foreach (Pair<int, int> dir in dirs)
                    {
                        if (casillaAct.Posicion.i + dir.First < 10 && casillaAct.Posicion.i + dir.First >= 0 &&
                                casillaAct.Posicion.j + dir.Second < 10 && casillaAct.Posicion.j + dir.Second >= 0 &&
                                matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second].coste == costeMin)
                            l.Add(matrizAgente[casillaAct.Posicion.i + dir.First, casillaAct.Posicion.j + dir.Second]);
                    }
                    int r = rnd.Next(0, l.Count);
                    Nodo next = l[r];
                    Casilla siguiente = matrizCompleta[next.casilla.Posicion.i, next.casilla.Posicion.j];

                    //3. Se mueve a la casilla que se ha decidido
                    casillaAct = siguiente;
                    return casillaAct.Posicion;
                }
            }
            else if (est == estadoAgente.buscaArma && !armaEncontrada)
            {


            }
            else completado = true;
            
        }
        return null;
    }
}
