using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeradorDeCaverna : MonoBehaviour
{
    public GameObject[,] Map;
    [Header("Map Configs")]

    [Tooltip("Map Width (squares)")]
    public int MapWidth;
    [Tooltip("Map Height Squares")]
    public int MapHeight;
    [Tooltip("Chance of spawning a wall")]
    [Range(10,75)]
    public int PercentAreWalls;
    [Tooltip("Number of Iteractions")]
    public int NIteraction;

    [Header("Prefabs")]
    [Tooltip("Celula Prefab")]
    public GameObject CelulaPrefab;

    // Quando se inicia Cria um vetor de games objects vazio com o tamo especificado nas variaveis MapWidth e MapHeight
    // E chama o metodo de Instanciar o mapa
    private void Awake()
    {
        Map = new GameObject[MapWidth, MapHeight];
        InstanciateMap();
    }

    // Metodo que instancia os game objects apos instanciar todos os game objects do mapa chama os metodos de randomizar o conteudo
    // E comeca a corotina para atualizar o mapa.
    private void InstanciateMap()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                Map[i, j] = Instantiate(CelulaPrefab, new Vector3(MapHeight/2f - MapHeight + i,MapWidth/2f - MapWidth +j), Quaternion.Euler(0, 0, 0));
                Map[i, j].transform.parent = transform;
            }
        }
        RandomFillMap();
        StartCoroutine(UpdateMap());
    }

    // Metodo que randomiza o conteudo das celulas (botando se e parede ou chao)
    public void RandomFillMap()
    {
        var mapMiddle = 0; // Variavel temporaria

        for (var i = 0; i < MapHeight; i++)
        {
            for (var j = 0; j < MapWidth; j++)
            {
                // Se as coordenadas forem na borda do mapa bota parede
                if (j == 0)
                {
                    Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 1;
                }
                else if (i == 0)
                {
                    Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 1;
                }
                else if (j == MapWidth - 1)
                {
                    Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 1;
                }
                else if (i == MapHeight - 1)
                {
                    Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 1;
                }

                // Se nao for na borda randomize o conteudo
                else
                {
                    mapMiddle = (MapHeight / 2);

                    if (i == mapMiddle)
                    {
                        Map[j, i].GetComponent<Celula>().MyTipo = 0;
                    }
                    else
                    {
                        if (WillInstantiateWall())
                        {
                            Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 1;
                        }
                        else
                        {
                            Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 0;
                        }
                    }
                }
            }
        }
    }

    // Metodo para ver se a posicao ira instanciar ou nao a parede, Se o random cair menor que a porcentagem instancia a parede.
    private bool WillInstantiateWall()
    {
        if (Random.Range(0, 101) <= PercentAreWalls)
        {
            return true;
        }
        return false;
    }

    // Logica do que ira virar parede. 
    // A logica usada e:
    // Se a ceula atual for parede e
    // //Se numero de paredes adjacentes (4-8) for maior ou igual a 4 --- Vira parede
    // //Se numero de paredes adjacentes (0-2) for menor que 2 ---------- Vira Chao
    // Se a celula atual for chao
    // //Se numero de paredes adjacentes (5-8) for maior ou igual a 5 --- Vira parede
    // Se nao for nada disso -------------------------------------------- Vira chao
    private int PlaceWallLogic(int x, int y)
    {
        var numWalls = GetAdjacentWalls(x, y);

        if (Map[x, y].GetComponent<Celula>().MyTipo == (Celula.Tipo) 1)
        {
            if (numWalls >= 4)
            {
                return 1;
            }
            if (numWalls < 2)
            {
                return 0;
            }

        }
        else
        {
            if (numWalls >= 5)
            {
                return 1;
            }
        }
        return 0;
    }

    // Metodo para checar o numero de paredes adjacentes
    private int GetAdjacentWalls(int x, int y)
    {
        var startX = x - 1;
        var startY = y - 1;
        var endX = x + 1;
        var endY = y + 1;

        int wallCounter = 0;

        for (var iY = startY; iY <= endY; iY++)
        {
            for (var iX = startX; iX <= endX; iX++)
            {
                if (iX == x && iY == y)
                {
                    continue;
                }
                if (IsWal(iX, iY))
                {
                    wallCounter += 1;
                }
            }
        }
        return wallCounter;
    }

    // Metodo para checar se a posicao indicada passada por parametro e uma parede ou nao.
    private bool IsWal(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return true;
        }
        switch (Map[x, y].GetComponent<Celula>().MyTipo)
        {
            case (Celula.Tipo) 1:
                return true;
            case (Celula.Tipo) 0:
                return false;
        }
        return false;
    }

    // Checa se a posicao passada por parametro esta dentro do grid do mapa
    private bool IsOutOfBounds(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return true;
        }
        else if (x > MapWidth - 1 || y > MapHeight - 1)
        {
            return true;
        }
        return false;
    }

    // Metodo para botar o mapa como chao
    public void BlankMap()
    {
        for (var i = 0; i < MapHeight; i++)
        {
            for (var j = 0; j < MapWidth; j++)
            {
                Map[j, i].GetComponent<Celula>().MyTipo = (Celula.Tipo) 0;
            }
        }
    }

    // Corotina usada para fazer as iteracoes do mapa e atualizar o conteudo da celula.
    private IEnumerator UpdateMap()
    {
        int n = 0;
        while (n <= NIteraction)
        {
            for (var j = 0; j <= MapHeight - 1; j++)
            {
                for (var i = 0; i <= MapWidth - 1; i++)
                {
                    Map[i, j].GetComponent<Celula>().MyTipo = (Celula.Tipo)PlaceWallLogic(i, j);
                }
            }
            n++;
            yield return new WaitForSeconds(1f);
        }
        
    } 

}
