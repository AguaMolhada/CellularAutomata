using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celula : MonoBehaviour {

    public enum Tipo
    {
        Floor,
        Wall,
    }

    public Tipo MyTipo;

    private void Update()
    {
        switch (MyTipo)
        {
            case Tipo.Floor:
                gameObject.GetComponent<Renderer>().material.color = Color.white;
                break;
            case Tipo.Wall:
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
