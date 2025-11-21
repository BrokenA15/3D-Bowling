using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMoveOnPress : MonoBehaviour
{
    
    public List<Transform> objetosAMover = new List<Transform>();

    [Header("Configuración de posiciones")]
    public float yBaja = -0.2f;
    public float yAlta = 0.3f;

    [Header("Velocidad del movimiento")]
    public float velocidad = 2f;

    [Header("Tiempo mínimo entre pulsaciones (segundos)")]
    public float tiempoEspera = 2f;

    private bool enPosicionAlta = false;
    private bool puedeTogglear = true;
    private List<Vector3> posicionesObjetivo = new List<Vector3>();

    private void Start()
    {
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

    public void TogglePosition()
    {
        if (!puedeTogglear) return; 
        StartCoroutine(Barreras());
    }

    private IEnumerator Barreras()
    {
        puedeTogglear = false; 

        enPosicionAlta = !enPosicionAlta;

        for (int i = 0; i < objetosAMover.Count; i++)
        {
            if (objetosAMover[i] == null) continue;

            Vector3 pos = objetosAMover[i].localPosition;
            pos.y = enPosicionAlta ? yAlta : yBaja;
            posicionesObjetivo[i] = pos;
        }

        
        yield return new WaitForSeconds(tiempoEspera);
        puedeTogglear = true;
    }
}
