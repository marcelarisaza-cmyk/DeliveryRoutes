# DeliveryRoutes - Ruta más corta para un domiciliario

***TALLER 5 (ARCHIVO PLANOS Y GRAFICOS / ANGHY MARCELAW REGUILLO ISAZA***

# ¿de qué trata este taller? 

El taller trata sobre una empresa de mensajería  que tiene un solo domiciliario. el cual: 

-Cada día recibe dos archivos: el mapa de la ciudad ´´city.txt´´(CARASTERISTICAS: con 100 ubicaciones y 142 calles, unas de doble sentido y otras de un solo sentido).

-la lista de tareas ´´packages.txt'', con 30 paquetes que hay que recoger en un punto y entregar en otro.


*Hice un programa de:*

consola en C# / .NET que lee los dos archivos, 
los valida y arma la ruta más corta que encuentra para recoger y entregar todo
mostrando paso a paso a dónde ir
qué hacer en cada punto
por qué calles pasar 
cuántos kilómetros recorre en total

# ¿cómo jecutarlo?

Necesitas el SDK de .NET 10.

***bash, dotnet build, dotnet run -- city.txt packages.txt D``` 
osea los tres argumentos son:

- Archivo del mapa
- el archivo de paquetes
- la posición inicial del domiciliario.

Para probar con el ejemplo pequeño del enunciado:

***bash, dotnet run -- city_mini.txt Y packages_mini.txt A

# ¿ Cómo está organizado el proyecto'

Seguí la organización que pedía el taller: el código vive en la carpeta
- Backend 
- una clase por archivo
- Program.cs 
- 

 Archivo / Para qué sirve:  EN CADA UNO DE ELLOS DEJE UN MENSAJE DE SU FUNCION.

-´´Program.cs´´ / Lee los argumentos que pide por medio de la consola luego llama al backend e imprime. 
- ´´Backend/Edge.cs´´ y  ´´Backend/Graph.cs´´ es el grafo dirigido con lista de adyacencia. 
- ´´Backend/SimpleTextFile.cs´´ se encarga de leer las líneas de un archivo de texto. 
- ´´Backend/CityFile.cs´´  Carga y valida city.txt y arma el grafo. 
- ´´Backend/Package.cs´´ y  ´´Backend/PackageFile.cs´´ Cargan y validan packages.txt y agrupan las asignaciones o tareas por paquetes. 
- ´´Backend/ShortestPaths.cs´´  Dijkstra, es la reconstrucción del camino y validación de alcance. 
- ´´Backend/Stop.cs´´ Una parada que seria punto, tipo que seria recoger o entregar y al final el paquete. 
- ´´Backend/RoutePlanner.cs´´  la heurística que ordena las paradas. 
- ´´Backend/RoutePrinter.cs´´ es quien se encarga de armar la salida paso a paso. 

LOS HICE PARA PROBAR ERRORES Y VALIDACIONES, Y PARA PROBAR EL EJEMPLO PEQUEÑO DEL ENUNCIADO.

- Los archivos ´´city_mini.txt´´, ´´packages_mini.txt´´:  son el ejemplo pequeño. 
- Los ´´city_bad.txt´´, ´´packages_bad.txt´´, ´´city_trap.txt´´ y ´´packages_trap.txt´´ .

# ¿ Cómo resolví el problema?

mi estrategia fue primero hacer una parte exacta *Dijkstra y luego una heurística *vecino más cercano, inserción, reubicación y reinicios*.

# Por qué no se pueden probar todas las combinaciones

Con 30 paquetes hay 60 paradas (30 recogidas y 30 entregas). Probar todos los órdenes posibles, aun respetando que cada paquete se recoge antes de entregarlo, asi daria un numero de 73 cifras.
Este problema es una variante del problema del agente viajero (recogida y entrega). Por eso no busqué la ruta perfecta, 
sino que usé una heurística: un método que no garantiza el óptimo, pero encuentra una ruta muy buena en pocos segundos.

# paso a paso de la solución 

*Distancia: la ciudad como un grafo dirigido.
*Una calle *OneWay* es una sola arista
*una *TwoWay* son dos, una en cada sentido, así el domiciliario nunca va en contravía.
Después corrí Dijkstra con ´´PriorityQueue´´ desde el punto inicial y desde cada punto de recogida o entrega, y guardé la distancia mínima y los predecesores para poder reconstruir las calles.
Con eso el resto del algoritmo trabaja con una tabla de distancias y ya no con el mapa. 
Algo que me llamó la atención es que, como hay calles de un solo sentido, la distancia de *AQ* a *BS* es 250 m, pero la de *BS* a *AQ* es 1630 m.

*segundo: Estando en un punto, voy a la parada válida más cercana: recoger un paquete que no he recogido o entregar uno que ya llevo.
es facil y siempre da una ruta valida, pero es mala (40.52 km) porque deja paradas lejanas para el final y obliga a cruzar la ciudad varias veces.

*terceero* se realizo las 3 : 

- Insersion: armo la ruta paquete por paquete. Para cada paquete pruebo todas las posiciones donde caben su recogida y su entrega y elijo la que menos alarga la ruta.
- Reubicacion: saco un paquete de la ruta y lo vuelvo a meter en su mejor posición, lo repito con todos los paquetes mientras la ruta siga mejorando.
- Reinicios el resultado depende del orden en que entran los paquetes, así que repito todo 100 veces con los paquetes barajados y me quedo con la ruta más corta. 
- 
# ¿ Cómo fue mejorando la distancia

- Estrategia - Recorrido total 

 Vecino más cercano - 40.52 km 
 + Inserción - 36.07 km  + Reubicación - 34.78 km + 100 reinicios con semilla fija para un total de 30.77 km en unos 4 segundos.
	+ La ruta es válida (60 tareas, 46 paradas porque las tareas en un mismo punto se agrupan) y el resultado es el mismo en cada ejecución.

#  ¿Cómo compruebo que la ruta es correcta?

- con  ´´IsValid´´ que  revisa que cada paquete se recoja exactamente una vez y se entregue exactamente una vez, y que la entrega sea después de la recogida.
- ef total se calcula sumando los mismos tramos que se imprimen, así que siempre coincide con lo quese ve en pantalla.
- las calles se recorren solo en su sentido permitido, porque el grafo es dirigido.

# Validaciones y pruebas

Antes de calcular nada, el programa valida los archivos y muestra mensajes claros (con número de línea cuando aplica) si:

- una línea no tiene el formato esperado o la distancia no es un número positivo;
- el sentido de una calle no es ´´TwoWay´´ ni ´´OneWay´´, o una calle está repetida ´´A,B´´ y ´´B,A´´ son la misma);
- una ubicación no existe en el mapa o la posición inicial no existe;
- la acción no es ´´Pickup´´ ni ´´Delivery´´, o el número de paquete no es un entero mayor que 0;
- un paquete tiene dos recogidas, dos entregas, o le falta una;
- un punto no es alcanzable desde la posición inicial, o desde un punto no se puede seguir hacia los demás.

Probé estos casos con archivos hechos a propósito:

- Prueba - Resultado 

///quise dejar cada prueba que realice del program pero quedaba muy desordenado. 
- 
- ´´city_bad.txt´´ muestra los 4 errores
- ´´packages_bad.txt´´ nuestra los 6 errores de paquetes.
- Posición inicial ´´Z´´ ´´La posición inicial 'Z' no existe en el mapa´´
- Archivo que no existe Mensaje claro. 
- ´´city_trap.txt´´ y ´´packages_trap.txt´´ ´´desde el punto C no se puede llegar a: A´´

# Dificultades que tuve

- me faltaron calles al copiar el mapa se me perdió una línea y el programa daba 253 aristas en vez de 254. Lo encontré contando las calles de cada tipo (112 de doble sentido y 30 de un solo sentido) y viendo que me faltaba una (OneWay).
- tuve que acordarme de que la distancia de ida y la de vuelta pueden ser distintas, y por eso calculo y guardo las dos con Dijkstra.

# Resultado

Con ´´city.txt´´ y ´´packages.txt´´  y posición inicial (D) el programa encuentra una ruta de (30,77 KM) en 4 S.
La ruta es válida (60 tareas, 46 paradas porque las tareas en un mismo punto se agrupan) y el resultado es el mismo en cada ejecución.

# Salida completa (mapa real)

```
POSICION INICIAL: D
Paquetes: 30  Ubicaciones: 100  Calles: 142 (30 UN SOLO SENTIDO)

1. En D: recoger paquete 18.
2. Ir de D a BE (620 m): recoger paquete 7. Camino: D -> BT -> BE
3. Ir de BE a X (490 m): recoger paquete 12. Camino: BE -> E -> X
4. Ir de X a BU (560 m): recoger paquetes 28 y 8. Camino: X -> B -> BU
5. Ir de BU a CN (430 m): recoger paquetes 2, 3 y 1. Camino: BU -> CN
6. Ir de CN a AM (780 m): recoger paquete 30. Camino: CN -> BU -> AM
7. Ir de AM a AQ (440 m): recoger paquete 9. Camino: AM -> AK -> AX -> AQ
8. Ir de AQ a CS (560 m): recoger paquete 19. Camino: AQ -> BS -> CS
9. Ir de CS a BS (310 m): recoger paquete 25. Camino: CS -> BS
10. Ir de BS a CH (460 m): recoger paquete 10. Camino: BS -> CT -> CJ -> CH
11. Ir de CH a CV (510 m): entregar paquete 28. Camino: CH -> CJ -> CV
12. Ir de CV a CU (540 m): recoger paquete 15. Camino: CV -> BX -> CU
13. Ir de CU a M (1060 m): entregar paquete 1. Camino: CU -> CV -> W -> CM -> M
14. Ir de M a AP (800 m): recoger paquete 5. Camino: M -> CM -> AE -> AP
15. Ir de AP a AT (350 m): recoger paquete 27. Camino: AP -> AT
16. Ir de AT a AX (370 m): recoger paquete 17. Camino: AT -> AQ -> AX
17. Ir de AX a BV (630 m): entregar paquete 19. Camino: AX -> F -> BV
18. Ir de BV a AB (470 m): recoger paquete 4. Camino: BV -> AB
19. Ir de AB a BW (2360 m): entregar paquete 30. Camino: AB -> BV -> F -> AX -> S -> K -> N -> BW
20. Ir de BW a CL (270 m): recoger paquete 14. Camino: BW -> CL
21. Ir de CL a R (360 m): recoger paquetes 6 y 21. Camino: CL -> R
22. Ir de R a BN (810 m): entregar paquete 7. Camino: R -> CL -> BN
23. Ir de BN a AI (1130 m): recoger paquete 24. Camino: BN -> Y -> AV -> CG -> AI
24. Ir de AI a AO (170 m): entregar paquete 3 y recoger paquete 11. Camino: AI -> AO
25. Ir de AO a BM (720 m): entregar paquetes 2 y 10. Camino: AO -> AI -> AS -> BM
26. Ir de BM a BI (1750 m): recoger paquete 13. Camino: BM -> AS -> BH -> BL -> CO -> BI
27. Ir de BI a Q (1380 m): recoger paquetes 22 y 29. Camino: BI -> CO -> AZ -> V -> O -> BY -> Q
28. Ir de Q a AA (1450 m): entregar paquete 12. Camino: Q -> BJ -> CI -> Z -> AA
29. Ir de AA a T (1630 m): entregar paquetes 27 y 24. Camino: AA -> Z -> CA -> BT -> BE -> T
30. Ir de T a BE (440 m): entregar paquete 21. Camino: T -> BE
31. Ir de BE a CP (460 m): entregar paquete 9. Camino: BE -> BT -> CP
32. Ir de CP a AD (670 m): entregar paquete 5. Camino: CP -> BT -> P -> CB -> AD
33. Ir de AD a AR (280 m): entregar paquete 8 y recoger paquete 20. Camino: AD -> AR
34. Ir de AR a BB (150 m): recoger paquete 16. Camino: AR -> BB
35. Ir de BB a AJ (400 m): entregar paquete 29 y recoger paquetes 23 y 26. Camino: BB -> AJ
36. Ir de AJ a AD (830 m): entregar paquete 13. Camino: AJ -> BB -> AR -> AD
37. Ir de AD a AC (1200 m): entregar paquete 17. Camino: AD -> CB -> P -> BT -> D -> AC
38. Ir de AC a CD (130 m): entregar paquete 25. Camino: AC -> CD
39. Ir de CD a BY (810 m): entregar paquete 16. Camino: CD -> BA -> BG -> BY
40. Ir de BY a BF (1170 m): entregar paquetes 18 y 23. Camino: BY -> O -> I -> BG -> BA -> AG -> BF
41. Ir de BF a AP (760 m): entregar paquetes 22 y 11. Camino: BF -> X -> B -> AP
42. Ir de AP a AE (370 m): entregar paquete 26. Camino: AP -> AE
43. Ir de AE a AT (720 m): entregar paquetes 15 y 14. Camino: AE -> AP -> AT
44. Ir de AT a AQ (200 m): entregar paquete 6. Camino: AT -> AQ
45. Ir de AQ a CT (350 m): entregar paquete 4. Camino: AQ -> BS -> CT
46. Ir de CT a CR (420 m): entregar paquete 20. Camino: CT -> CR

Todos los paquetes fueron recogidos y entregados.
Recorrido total: 30.77 km
```

# Salida del ejemplo pequeño

Este es el caso de la sección 6 del enunciado ´´city_mini.txt´´ Y  ´´packages_mini.txt´´  ,  posición inicial (A)
dio (2,44 KM) que seria el óptimo conocido.teniendo la misma distancia total del enuncaido de ZULU.


OTRO EJEMPLO DE EJECUCION 

POSICION INICIAL: A
Paquetes: 3  Ubicaciones: 7  Calles: 7 (1 UN SOLO SENTIDO)

1. Ir de A a C (400 m): recoger paquete 1. Camino: A -> B -> C
2. Ir de C a E (350 m): recoger paquete 2. Camino: C -> B -> E
3. Ir de E a G (270 m): entregar paquete 1 y recoger paquete 3. Camino: E -> F -> G
4. Ir de G a A (820 m): entregar paquete 3. Camino: G -> F -> E -> B -> A
5. Ir de A a D (600 m): entregar paquete 2. Camino: A -> B -> C -> D

Todos los paquetes fueron recogidos y entregados.
Recorrido total: 2.44 km


# Commits

Fui haciendo un commit por cada paso del taller de manera ordenanda (creación del proyecto, clases, lectura de archivos, validaciones, armado del
grafo, carga del mapa, carga de paquetes, Dijkstra, heurísticas, salida, validaciones y este README). asi se fue descartando errores y revisando logros. 