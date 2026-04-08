using UnityEngine;
using UnityEditor; // Librería especial para modificar el editor de Unity

public class ReemplazadorPrefab : EditorWindow
{
    // El prefab que vamos a usar como reemplazo
    public GameObject nuevoPrefab;

    // Esto crea un nuevo menú en la barra de arriba de Unity
    [MenuItem("Herramientas/Reemplazador Masivo")]
    public static void MostrarVentana()
    {
        GetWindow<ReemplazadorPrefab>("Reemplazar Objetos");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reemplazar objetos seleccionados", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Casillero para arrastrar el prefab
        nuevoPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab de Reemplazo", nuevoPrefab, typeof(GameObject), false);

        GUILayout.Space(10);

        if (GUILayout.Button("¡Reemplazar Selección!"))
        {
            ReemplazarObjetos();
        }
    }

    private void ReemplazarObjetos()
    {
        if (nuevoPrefab == null)
        {
            Debug.LogWarning("¡Te olvidaste de asignar el prefab de reemplazo!");
            return;
        }

        // Agarramos todos los objetos que tengas seleccionados en la jerarquía
        GameObject[] seleccion = Selection.gameObjects;

        if (seleccion.Length == 0)
        {
            Debug.LogWarning("No tenés ningún objeto seleccionado.");
            return;
        }

        foreach (GameObject objetoViejo in seleccion)
        {
            // Instanciamos el nuevo prefab respetando que siga siendo un prefab
            GameObject objetoNuevo = (GameObject)PrefabUtility.InstantiatePrefab(nuevoPrefab);

            // Le copiamos la posición, rotación, escala y el "Padre" del viejo
            objetoNuevo.transform.position = objetoViejo.transform.position;
            objetoNuevo.transform.rotation = objetoViejo.transform.rotation;
            objetoNuevo.transform.localScale = objetoViejo.transform.localScale;
            objetoNuevo.transform.parent = objetoViejo.transform.parent;

            // Registramos la acción para poder hacer Ctrl+Z si nos arrepentimos
            Undo.RegisterCreatedObjectUndo(objetoNuevo, "Crear reemplazo");
            Undo.DestroyObjectImmediate(objetoViejo);
        }

        Debug.Log("¡Se reemplazaron " + seleccion.Length + " objetos con éxito!");
    }
}