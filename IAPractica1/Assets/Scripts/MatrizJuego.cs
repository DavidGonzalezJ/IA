using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct pieza{
    public int valor, i, j;
    public pieza(int v, int y, int g) {
        valor = v;
        i = y;
        j = g;
    }
    public void SetIJ(int Y, int G) { i = Y; j = G; }
};

enum pos {arriba, abajo, izquierda, derecha};



public class MatrizJuego : MonoBehaviour {
    public static int tam = 3;
    pieza[,] matriz = new pieza[tam, tam];
    pieza hueco = new pieza(tam * tam, tam - 1, tam - 1);

    //Inicializa la matriz para cualquier tamaño
    void matrizInicial(){
        for (int i = 0; i < tam; i++) {
            for (int j = 0; j < tam; j++) {
                matriz[i, j].i = i;
                matriz[i, j].j = j;
                matriz[i, j].valor = i * tam + j + 1;
            }
        }
    }

    //Devuelve el valor de una casilla adyacente al hueco o false si no existe.
    bool dameCasilla(pos x, out int valor) {
        valor = 0;
        if (x == pos.arriba) {
            if (hueco.i - 1 >= 0)
            {
                valor = matriz[hueco.i - 1, hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.abajo)
        {
            if (hueco.i + 1 <tam)
            {
                valor = matriz[hueco.i + 1, hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.derecha)
        {
            if (hueco.j + 1 < tam)
            {
                valor = matriz[hueco.i, hueco.j +1].valor;
                return true;
            }
        }
        else if (x == pos.izquierda)
        {
            if (hueco.j - 1 >= 0)
            {
                valor = matriz[hueco.i, hueco.j - 1].valor;
                return true;
            }
        }
        return false;
    }

    //Cambia el hueco por la casilla que se le indica al metodo (si no puede devuelve false)
    bool cambio(pos x) {
        int valor;
        if (dameCasilla(x, out valor)) {
            switch (x)
            {
                case pos.arriba:
                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i - 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i + 1, hueco.j].valor = valor;
                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    break;
                case pos.abajo:
                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i + 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i - 1, hueco.j].valor = valor;
                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    break;
                case pos.izquierda:
                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j -1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i , hueco.j + 1].valor = valor;
                    matriz[hueco.i , hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    break;
                case pos.derecha:
                    matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j + 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j - 1].valor = valor;
                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    break;
            }
            //Método que ilustra el cambio
            return true;
        }
        return false;
    }

    public void baraja() {
        int direccion;
        for (int i = 0; i < 100; i++) {
            direccion = Random.Range(0, 4);
            cambio((pos)direccion);
        }
    }

    public void resuelve1() {




    }
        // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}