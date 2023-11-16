# Autor: Sebastian Moncada - A01027028

# Programa para crear el modelo de una rueda teniendo como parámetros el número
# de lados, el radio y el ancho

import math

archivo = open("./Assets/Models/wheel.obj", "w")

# Parámetros del programa
numLados = 8
radio = 0.3
anchoIn = 0.4
# numLados = int(input("Número de lados = "))
# radio = float(input("Radio = "))
# anchoIn = float(input("Ancho = "))


# Calcular el vector normal entre tres vértices usando los índices que tienen estos en el vector de vertices
def calculateNormal(i1, i2, i3):
    u = [vertices[i2][0] - vertices[i1][0],
         vertices[i2][1] - vertices[i1][1],
         vertices[i2][2] - vertices[i1][2]]

    v = [vertices[i3][0] - vertices[i1][0],
         vertices[i3][1] - vertices[i1][1],
         vertices[i3][2] - vertices[i1][2]]

    w = [u[1] * v[2] - u[2] * v[1],
         u[2] * v[0] - u[0] * v[2],
         u[0] * v[1] - u[1] * v[0]]

    return [round(w[0], 4), round(w[1], 4), round(w[2], 4)]


vertices = []
# Hacer ambas caras de la rueda (por eso 2)
for i in range(2):
    # La primera cara se genera en el eje x positivo y la segunda en el negativo
    if i == 1:
        ancho = round(-anchoIn / 2, 4)
    else:
        ancho = round(anchoIn / 2, 4)

    vertices.append([ancho, 0, 0])  # Origen de la cara (0,0)

    # Calcular los vértices usando el ángulo
    for j in range(numLados):
        y = round(radio * math.sin(math.radians(360 / numLados * (j + 1))), 4)
        z = round(radio * math.cos(math.radians(360 / numLados * (j + 1))), 4)

        vertices.append([ancho,  y,  z])


normals = []
# Obtener las normals. Se sigue el mismo patrón que para obtener las faces pero como ahora son los índices del vector de vertices, se le resta 1 a todo lo del patrón de faces
for i in range(numLados):
    if (i < numLados - 1):
        normals.append(calculateNormal(0, i + 2, i + 1))
        normals.append(calculateNormal(
            numLados + 1, numLados + 2 + i, numLados + 3 + i))
        normals.append(calculateNormal(1 + i, 2 + i, numLados + 2 + i))
        normals.append(calculateNormal(
            2 + i, numLados + 3 + i, numLados + 2 + i))
    else:
        normals.append(calculateNormal(0, 1, 1 + i))
        normals.append(calculateNormal(
            numLados + 1, numLados + 2 + i, numLados + 2))
        normals.append(calculateNormal(1 + i, 1, numLados + 2 + i))
        normals.append(calculateNormal(1, numLados + 2, numLados + 2 + i))


faces = []
# Obtener las faces. Esto es siguiendo un patrón
for i in range(numLados):
    if (i < numLados - 1):
        # Faces de las caras circulares
        faces.append([1, 3 + i, 2 + i])
        faces.append([numLados + 2, numLados + 3 + i, numLados + 4 + i])
        # Faces de los lados de la rueda
        faces.append([2 + i, 3 + i, numLados + 3 + i])
        faces.append([3 + i, numLados + 4 + i, numLados + 3 + i])
    # Cuando es el último elemento se tiene que dar la vuelta a los índices
    else:
        faces.append([1, 2, 2 + i])
        faces.append([numLados + 2, numLados + 3 + i, numLados + 3])
        faces.append([2 + i, 2, numLados + 3 + i])
        faces.append([2, numLados + 3, numLados + 3 + i])


# Agregar los vertices, normals y faces al output
output = ["Vertices:\n\n"]
for vertice in vertices:
    output.append("v " +
                  str(vertice[0]) + " " +
                  str(vertice[1]) + " " +
                  str(vertice[2]) + "\n")

output.append("\nNormals:\n\n")
for normal in normals:
    output.append("vn " +
                  str(normal[0]) + " " +
                  str(normal[1]) + " " +
                  str(normal[2]) + "\n")

output.append("\nFaces:\n\n")
for i in range(len(faces)):
    output.append("f " +
                  str(faces[i][0]) + "//" + str(i + 1) + " " +
                  str(faces[i][1]) + "//" + str(i + 1) + " " +
                  str(faces[i][2]) + "//" + str(i + 1) + "\n")

# Escribir todo el output en el archivo abierto
archivo.writelines(output)
archivo.close()
