using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
    [SerializeField] GameObject[] wheels;
    [SerializeField] Vector3 displacement = new Vector3(0, 0, 3);
    [SerializeField] float rotationSpeed = 90;
    [SerializeField] AXIS rotationAxis = AXIS.X;
    [SerializeField] float distanceToTravel = 5.5f;

    float distanceTraveled;
    float timeSinceLastPos = 0f;
    int rotationAngle = 90;
    Mesh[] meshes;
    Vector3[][] baseVertices;
    Vector3[][] newVertices;


    // Start is called before the first frame update
    void Start()
    {
        distanceTraveled = 0;
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
        // distanceTraveled += Time.deltaTime;
        // if (distanceTraveled < 2)
        DoTransform();
        // else
        //     ChangeRotation();

    }

    void DoTransform()
    {
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.time,
                                                        displacement.y * Time.time,
                                                        displacement.z * Time.time);

        Matrix4x4 rotate = HW_Transforms.RotateMat(rotationSpeed * Time.time, rotationAxis); ;

        for (int i = 0; i < meshes.Length; i++)
        {
            Matrix4x4 composite = move;
            if (i > 0)
                composite *= rotate;

            for (int j = 0; j < baseVertices[i].Length; j++)
            {
                Vector4 temp = new Vector4(baseVertices[i][j].x,
                                            baseVertices[i][j].y,
                                            baseVertices[i][j].z,
                                            1);

                newVertices[i][j] = composite * temp;
            }

            meshes[i].vertices = newVertices[i];
            baseVertices[i] = newVertices[i];
            meshes[i].RecalculateNormals();
        }

    }

    void ChangeRotation()
    {
        timeSinceLastPos = Time.time;
        for (int i = 0; i < meshes.Length; i++)
        {
            baseVertices[i] = meshes[i].vertices;
        }


        Matrix4x4 rotate = HW_Transforms.RotateMat(rotationAngle, AXIS.Y); ;
        Matrix4x4 composite = rotate;
        for (int i = 0; i < meshes.Length; i++)
        {

            for (int j = 0; j < baseVertices[i].Length; j++)
            {
                Vector4 temp = new Vector4(baseVertices[i][j].x,
                                            baseVertices[i][j].y,
                                            baseVertices[i][j].z,
                                            1);

                newVertices[i][j] = composite * temp;
            }

            meshes[i].vertices = newVertices[i];
            meshes[i].RecalculateNormals();
        }
        distanceTraveled = 0;
    }
}