using System.Collections.Generic;
using UnityEngine;

public class GroupMoveOnPress : MonoBehaviour
{
    [Header("Objetos que se moverán")]
    public List<Transform> objetosAMover = new List<Transform>();

    [Header("Configuración de posiciones")]
    public float yBaja = -0.2f;
    public float yAlta = 0.3f;

    [Header("Velocidad del movimiento")]
    public float velocidad = 2f;

    private bool enPosicionAlta = false;
    private List<Vector3> posicionesObjetivo = new List<Vector3>();

    private void Start()
    {
        // Guarda las posiciones iniciales y ajusta todas a yBaja
        posicionesObjetivo.Clear();
        foreach (var obj in objetosAMover)
        {
            if (obj == null) continue;

            Vector3 pos = obj.localPosition;
            pos.y = yBaja;
            obj.localPosition = pos;

            posicionesObjetivo.Add(pos);
        }
    }

    private void Update()
    {
        // Mueve todos los objetos suavemente a su posición objetivo
        for (int i = 0; i < objetosAMover.Count; i++)
        {
            if (objetosAMover[i] == null) continue;

            objetosAMover[i].localPosition = Vector3.Lerp(
                objetosAMover[i].localPosition,
                posicionesObjetivo[i],
                Time.deltaTime * velocidad
            );
        }
    }

    // Llama a este método desde un botón o evento VR
    public void TogglePosition()
    {
        enPosicionAlta = !enPosicionAlta;

        for (int i = 0; i < objetosAMover.Count; i++)
        {
            if (objetosAMover[i] == null) continue;

            Vector3 pos = objetosAMover[i].localPosition;
            pos.y = enPosicionAlta ? yAlta : yBaja;
            posicionesObjetivo[i] = pos;
        }
    }
}
