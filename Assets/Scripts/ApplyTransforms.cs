// Sebastian Moncada - A01027028
// Script para hacer que un carro se mueva y rote usando geometría computacional

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
    [SerializeField] Vector3 displacement; // Coordenadas a las que se va a mover
    [SerializeField] float spinSpeed; // Velocidad a la que giran las ruedas
    [SerializeField] GameObject wheelPrefab; // Prefab de las ruedas

    AXIS rotationAxis = AXIS.Y; // Eje en el que gira el carro
    Transform car; // Transform del carro
    float rotationAngle; // Ángulo al que va a rotar el carro
    GameObject[] wheels = new GameObject[4]; // Ruedas
    Vector3[] wheelsPos = new Vector3[4]; // Posición inicial de las ruedas
    Mesh[] meshes; // Mesh del carro y de las ruedas
    // Vertices del carro y de las ruedas
    Vector3[][] baseVertices;
    Vector3[][] newVertices;


    // Start is called before the first frame update
    void Start()
    {
        car = gameObject.transform;
        CreateWheels();

        meshes = new Mesh[5];
        baseVertices = new Vector3[5][];
        newVertices = new Vector3[5][];

        meshes[0] = GetComponentInChildren<MeshFilter>().mesh;
        for (int i = 1; i < 5; i++)
        {
            meshes[i] = wheels[i - 1].GetComponentInChildren<MeshFilter>().mesh;
        }

        for (int i = 0; i < meshes.Length; i++)
        {
            baseVertices[i] = meshes[i].vertices;

            newVertices[i] = new Vector3[baseVertices[i].Length];
            for (int j = 0; j < baseVertices.Length; j++)
            {
                newVertices[i][j] = baseVertices[i][j];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DoTransform();
    }

    // Mover el carro
    void DoTransform()
    {
        // Calcular el ángulo al que el carro y las ruedas tienen que rotar para ir a la dirección de displacement
        rotationAngle = Mathf.Atan2(displacement.z, displacement.x) * Mathf.Rad2Deg - 90;

        // Matriz para mover el carro y las ruedas
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.time,
                                                        displacement.y * Time.time,
                                                        displacement.z * Time.time);

        // Matriz para rotar el carro y las ruedas
        Matrix4x4 rotate = HW_Transforms.RotateMat(rotationAngle, rotationAxis);

        // Matriz para girar las ruedas sobre su propio eje (como si se movieran adelante)
        Matrix4x4 spin = HW_Transforms.RotateMat(spinSpeed * Time.time, AXIS.X);


        // Hacer las operaciones en todos los objetos para que hagan su movimiento pertinente
        for (int i = 0; i < meshes.Length; i++)
        {
            Matrix4x4 composite;

            // Las ruedas
            if (i > 0)
            {
                // Mover las ruedas al origen del carro
                Matrix4x4 goToOrigin = HW_Transforms.TranslationMat(-wheelsPos[i - 1].x,
                                                                    0,
                                                                    -wheelsPos[i - 1].z);

                // Devolver las ruedas a su posición adecuada
                Matrix4x4 goBackToPlace = HW_Transforms.TranslationMat(wheelsPos[i - 1].x,
                                                                        0,
                                                                        wheelsPos[i - 1].z);

                // Lleva las ruedas al origen, las rota en la dirección correcta y las lleva de nuevo a su posición correcta para moverlas junto al carro (todo mientras están girando)
                composite = move * goToOrigin * rotate * goBackToPlace * spin;
            }
            // Solo el carro
            else
                // Rota el carro en la dirección correcta y lo mueve
                composite = move * rotate;

            // Hace los cálculos en los vértices de los objetos para que tengan su comportamiento
            for (int j = 0; j < baseVertices[i].Length; j++)
            {
                Vector4 temp = new Vector4(baseVertices[i][j].x,
                                            baseVertices[i][j].y,
                                            baseVertices[i][j].z,
                                            1);

                newVertices[i][j] = composite * temp;
            }
            meshes[i].vertices = newVertices[i];

            // Recalcular las normales para que la luz se comporte como debería
            meshes[i].RecalculateNormals();
            // Para poder ver siempre los objetos en la simulación sin importar cuánto nos acerquemos
            meshes[i].RecalculateBounds();
        }

    }

    // Crear las ruedas
    void CreateWheels()
    {
        float x = 0.75f, y = 0.30f, z = 1.25f;

        // Posiciones para las ruedas
        wheelsPos[0] = new Vector3(x, y, z);
        wheelsPos[1] = new Vector3(-x, y, z);
        wheelsPos[2] = new Vector3(x, y, -z);
        wheelsPos[3] = new Vector3(-x, y, -z);

        // Crear las ruedas
        for (int i = 0; i < 4; i++)
        {
            Vector3 relativePos = car.position + wheelsPos[i];
            wheels[i] = Instantiate(wheelPrefab, relativePos, Quaternion.identity);
        }
    }
}