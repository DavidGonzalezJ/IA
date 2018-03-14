using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolutor : MonoBehaviour {


    public List<casilla> caminoAstarManhattan(casilla origen, casilla destino) {
        //Este será el camino que rellenaremos al final
        List<casilla> camino = new List<casilla>();
        casilla[,] tablero = new casilla[10,10];
        eCasilla[,] estados = PuzzleManager.Instance.matriz;
        //Relleno los costes de cada casilla con la info de sus estados
        // Y relleno los costes heurísticos (Manhattan) de cada casilla sabiendo el destino
        int G;
        dim posDestino = new dim();
        posDestino.Set( destino.Posicion.x, destino.Posicion.y );
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                G = (int)estados[i, j] + 1;
                tablero[i, j].G = G;//Coste casilla
                if (G > 2) tablero[i, j].noPasar = true;
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
        inicio.G = 0;
        inicio.F = inicio.G + inicio.H;
        aCheckear.Add(inicio);


        //Ojo aquí que viene lo gordo
        while (aCheckear.Count != 0) {
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
        }




        return camino;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
