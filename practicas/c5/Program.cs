
using System;
using System.Diagnostics;

namespace C5.TorneoOrdenamientos
{
    internal class Program
    {
        // ============================================================
        // C5 - TORNEO DE ALGORITMOS DE ORDENAMIENTO
        //
        // OBJETIVO:
        // Comparar experimentalmente cuatro algoritmos de ordenamiento
        // utilizando cantidades crecientes de datos.
        //
        // COMPETIDORES:
        // 1. Insertion Sort: O(n²).
        // 2. MergeSort:      O(n log n).
        // 3. QuickSort:      O(n log n) promedio, O(n²) peor caso.
        // 4. Array.Sort:     Ordenamiento nativo de .NET.
        //
        // RÚBRICA:
        // - Comparar un algoritmo cuadrático contra algoritmos O(n log n).
        // - Probar cantidades de 10.000, 20.000, 40.000 y 80.000.
        // - Ejecutar una corrida de calentamiento.
        // - Realizar tres corridas medidas.
        // - Calcular el promedio de los tiempos.
        // - Verificar que cada algoritmo deje el arreglo ordenado.
        // - Presentar tabla de tiempos y conclusiones.
        // ============================================================

        // RÚBRICA: cantidades crecientes de datos.
        private static readonly int[] Cantidades =
        {
            10_000,
            20_000,
            40_000,
            80_000
        };

        // RÚBRICA: tres corridas medidas por algoritmo y tamaño.
        private const int NumeroCorridas = 3;

        // Semilla fija para reproducir los mismos datos en cada ejecución.
        private const int Semilla = 2026;

        // Rango de valores aleatorios: 0 hasta 999.999.
        private const int ValorMaximo = 1_000_000;

        // ============================================================
        // MÉTODO PRINCIPAL
        // Coordina la ejecución del torneo y presenta los resultados.
        // ============================================================

        static void Main(string[] args)
        {
            Console.WriteLine("==============================================================");
            Console.WriteLine("           C5 - TORNEO DE ALGORITMOS DE ORDENAMIENTO");
            Console.WriteLine("==============================================================");

            Console.WriteLine();
            Console.WriteLine("COMPETIDORES:");
            Console.WriteLine("1. Insertion Sort - O(n²)");
            Console.WriteLine("2. MergeSort      - O(n log n)");
            Console.WriteLine("3. QuickSort      - O(n log n) promedio");
            Console.WriteLine("4. Array.Sort     - Ordenamiento nativo de .NET");

            Console.WriteLine();
            Console.WriteLine("PROTOCOLO DE MEDICIÓN:");
            Console.WriteLine("- 1 corrida de calentamiento.");
            Console.WriteLine("- 3 corridas medidas.");
            Console.WriteLine("- Cálculo del promedio.");
            Console.WriteLine("- Verificación del arreglo ordenado.");
            Console.WriteLine();

            // RÚBRICA: encabezado de la tabla comparativa.
            Console.WriteLine("--------------------------------------------------------------------------");

            Console.WriteLine(
                $"{"n",10} " +
                $"{"Insertion",14} " +
                $"{"MergeSort",14} " +
                $"{"QuickSort",14} " +
                $"{"Array.Sort",14}"
            );

            Console.WriteLine("--------------------------------------------------------------------------");

            // Ejecutamos el torneo para cada cantidad de elementos.
            foreach (int cantidad in Cantidades)
            {
                // Generamos los datos originales una sola vez.
                // Todos los competidores recibirán copias idénticas.
                int[] datosOriginales = GenerarDatos(cantidad);

                // COMPETIDOR 1: INSERTION SORT.
                double tiempoInsertion = MedirPromedio(
                    "Insertion Sort",
                    InsertionSort,
                    datosOriginales
                );

                // COMPETIDOR 2: MERGESORT.
                double tiempoMerge = MedirPromedio(
                    "MergeSort",
                    MergeSort,
                    datosOriginales
                );

                // COMPETIDOR 3: QUICKSORT.
                double tiempoQuick = MedirPromedio(
                    "QuickSort",
                    QuickSort,
                    datosOriginales
                );

                // COMPETIDOR 4: ARRAY.SORT.
                // Utilizamos el método nativo de ordenamiento de .NET.
                double tiempoNativo = MedirPromedio(
                    "Array.Sort",
                    arreglo => Array.Sort(arreglo),
                    datosOriginales
                );

                // RÚBRICA: presentación de los tiempos promedio.
                Console.WriteLine(
                    $"{cantidad,10:N0} " +
                    $"{tiempoInsertion,11:F3} ms " +
                    $"{tiempoMerge,11:F3} ms " +
                    $"{tiempoQuick,11:F3} ms " +
                    $"{tiempoNativo,11:F3} ms"
                );
            }

            Console.WriteLine("--------------------------------------------------------------------------");

            Console.WriteLine();
            Console.WriteLine("TORNEO FINALIZADO.");
            Console.WriteLine("Los cuatro algoritmos superaron las verificaciones.");
            Console.WriteLine();
        }

        // ============================================================
        // GENERACIÓN DE DATOS
        // ============================================================

        // RÚBRICA:
        // Generar arreglos con las cantidades crecientes solicitadas.
        //
        // Utilizamos una semilla fija para que los datos sean reproducibles.
        // De esta manera, cada algoritmo recibe exactamente los mismos datos.
        private static int[] GenerarDatos(int cantidad)
        {
            Random random = new Random(Semilla);

            int[] datos = new int[cantidad];

            for (int i = 0; i < cantidad; i++)
            {
                datos[i] = random.Next(0, ValorMaximo);
            }

            return datos;
        }

        // ============================================================
        // MEDICIÓN DEL RENDIMIENTO
        // ============================================================

        // RÚBRICA:
        // 1. Ejecutar una corrida de calentamiento.
        // 2. Ejecutar tres corridas medidas.
        // 3. Calcular el promedio.
        // 4. Verificar que el arreglo quede correctamente ordenado.
        //
        // Action<int[]> permite recibir cualquier método que ordene
        // un arreglo de enteros.
        private static double MedirPromedio(
            string nombre,
            Action<int[]> algoritmo,
            int[] datosOriginales)
        {
            // --------------------------------------------------------
            // PASO 1: CALENTAMIENTO
            // --------------------------------------------------------

            // Creamos una copia para no modificar los datos originales.
            // Esta ejecución no se incluye en el promedio.
            int[] arregloCalentamiento = (int[])datosOriginales.Clone();

            algoritmo(arregloCalentamiento);

            // Verificamos que el algoritmo funcione correctamente.
            VerificarOrdenamiento(arregloCalentamiento, nombre);

            // --------------------------------------------------------
            // PASO 2: TRES CORRIDAS MEDIDAS
            // --------------------------------------------------------

            double tiempoTotal = 0;

            for (int corrida = 1; corrida <= NumeroCorridas; corrida++)
            {
                // IMPORTANTE:
                // Clonamos el arreglo ANTES de iniciar el cronómetro.
                // Así no contabilizamos el tiempo de copiar los datos.
                int[] arreglo = (int[])datosOriginales.Clone();

                Stopwatch cronometro = Stopwatch.StartNew();

                // Medimos exclusivamente la ejecución del algoritmo.
                algoritmo(arreglo);

                cronometro.Stop();

                // RÚBRICA:
                // Verificamos el resultado después de detener el cronómetro.
                // La validación no afecta el tiempo medido.
                VerificarOrdenamiento(arreglo, nombre);

                // Acumulamos el tiempo de esta corrida.
                tiempoTotal += cronometro.Elapsed.TotalMilliseconds;
            }

            // --------------------------------------------------------
            // PASO 3: PROMEDIO
            // --------------------------------------------------------

            // Fórmula:
            // Promedio = (Tiempo 1 + Tiempo 2 + Tiempo 3) / 3.
            return tiempoTotal / NumeroCorridas;
        }

        // ============================================================
        // COMPETIDOR 1: INSERTION SORT
        // COMPLEJIDAD: O(n²) en promedio y peor caso.
        // ============================================================

        // Ordena insertando cada elemento en su posición correcta
        // dentro de la parte del arreglo que ya está ordenada.
        //
        // Funcionamiento:
        // 1. Considera el primer elemento como ordenado.
        // 2. Toma el siguiente elemento.
        // 3. Desplaza hacia la derecha los elementos mayores.
        // 4. Inserta el elemento en su posición correcta.
        private static void InsertionSort(int[] arreglo)
        {
            // Comenzamos desde la segunda posición.
            for (int i = 1; i < arreglo.Length; i++)
            {
                // Guardamos el elemento que vamos a insertar.
                int actual = arreglo[i];

                // Comenzamos a comparar desde el elemento anterior.
                int j = i - 1;

                // Desplazamos los elementos mayores hacia la derecha.
                while (j >= 0 && arreglo[j] > actual)
                {
                    arreglo[j + 1] = arreglo[j];

                    j--;
                }

                // Insertamos el elemento en su posición correcta.
                arreglo[j + 1] = actual;
            }
        }

        // ============================================================
        // COMPETIDOR 2: MERGESORT
        // COMPLEJIDAD: O(n log n).
        // ESTRATEGIA: DIVIDIR - RESOLVER - COMBINAR.
        // ============================================================

        // Método principal de MergeSort.
        //
        // Crea un arreglo auxiliar que se utiliza durante las mezclas.
        // Después inicia el proceso recursivo.
        private static void MergeSort(int[] arreglo)
        {
            int[] auxiliar = new int[arreglo.Length];

            DividirYOrdenar(
                arreglo,
                auxiliar,
                0,
                arreglo.Length - 1
            );
        }

        // DIVIDIR Y RESOLVER:
        //
        // Divide el arreglo en dos mitades.
        // Ordena recursivamente cada mitad.
        // Finalmente combina las dos mitades ordenadas.
        private static void DividirYOrdenar(
            int[] arreglo,
            int[] auxiliar,
            int inicio,
            int fin)
        {
            // CASO BASE:
            // Si hay cero o un elemento, ya está ordenado.
            // Esta condición detiene la recursividad.
            if (inicio >= fin)
                return;

            // Calculamos el punto medio para dividir el arreglo.
            int medio = inicio + (fin - inicio) / 2;

            // PRIMERA MITAD:
            // Ordenamos desde inicio hasta medio.
            DividirYOrdenar(
                arreglo,
                auxiliar,
                inicio,
                medio
            );

            // SEGUNDA MITAD:
            // Ordenamos desde medio + 1 hasta fin.
            DividirYOrdenar(
                arreglo,
                auxiliar,
                medio + 1,
                fin
            );

            // COMBINAR:
            // Las dos mitades ya están ordenadas.
            // Ahora las mezclamos en un único segmento ordenado.
            Mezclar(
                arreglo,
                auxiliar,
                inicio,
                medio,
                fin
            );
        }

        // COMBINAR:
        //
        // Recibe dos mitades previamente ordenadas y las mezcla.
        // Compara el primer elemento disponible de cada mitad
        // y coloca el menor en el arreglo auxiliar.
        //
        // El proceso avanza hasta completar todos los elementos.
        private static void Mezclar(
            int[] arreglo,
            int[] auxiliar,
            int inicio,
            int medio,
            int fin)
        {
            int i = inicio;       // Posición actual de la primera mitad.
            int j = medio + 1;    // Posición actual de la segunda mitad.
            int k = inicio;       // Posición de escritura en auxiliar.

            // Mientras ambas mitades tengan elementos disponibles,
            // comparamos sus primeros elementos y elegimos el menor.
            while (i <= medio && j <= fin)
            {
                if (arreglo[i] <= arreglo[j])
                {
                    auxiliar[k] = arreglo[i];
                    i++;
                }
                else
                {
                    auxiliar[k] = arreglo[j];
                    j++;
                }

                k++;
            }

            // Si quedaron elementos en la primera mitad,
            // los copiamos directamente.
            while (i <= medio)
            {
                auxiliar[k] = arreglo[i];

                i++;
                k++;
            }

            // Si quedaron elementos en la segunda mitad,
            // también los copiamos directamente.
            while (j <= fin)
            {
                auxiliar[k] = arreglo[j];

                j++;
                k++;
            }

            // Copiamos el segmento ya ordenado desde auxiliar
            // hacia el arreglo original.
            for (int posicion = inicio; posicion <= fin; posicion++)
            {
                arreglo[posicion] = auxiliar[posicion];
            }
        }

        // ============================================================
        // COMPETIDOR 3: QUICKSORT
        // COMPLEJIDAD:
        // - Promedio: O(n log n).
        // - Peor caso: O(n²).
        //
        // ESTRATEGIA:
        // Elegir un pivote, repartir los elementos y ordenar
        // recursivamente las particiones.
        // ============================================================

        // Método principal de QuickSort.
        //
        // Inicia el proceso recursivo utilizando todo el arreglo.
        private static void QuickSort(int[] arreglo)
        {
            Rapido(
                arreglo,
                0,
                arreglo.Length - 1
            );
        }

        // PARTICIONAR Y RESOLVER:
        //
        // Utiliza el último elemento como pivote.
        // Los elementos menores quedan a la izquierda.
        // Los mayores o iguales quedan a la derecha.
        //
        // El pivote termina en su posición definitiva.
        private static void Rapido(
            int[] arreglo,
            int inicio,
            int fin)
        {
            // CASO BASE:
            // Si hay cero o un elemento, no necesitamos ordenar.
            if (inicio >= fin)
                return;

            // Elegimos el último elemento como pivote,
            // siguiendo la implementación estudiada en clase.
            int pivote = arreglo[fin];

            // i representa la frontera de los elementos menores
            // que el pivote.
            int i = inicio;

            // Recorremos los elementos anteriores al pivote.
            for (int j = inicio; j < fin; j++)
            {
                // Si encontramos un elemento menor al pivote,
                // lo colocamos en la parte izquierda.
                if (arreglo[j] < pivote)
                {
                    (arreglo[i], arreglo[j]) = (arreglo[j], arreglo[i]);

                    i++;
                }
            }

            // Colocamos el pivote en su posición definitiva.
            (arreglo[i], arreglo[fin]) = (arreglo[fin], arreglo[i]);

            // DIVIDIR Y RESOLVER:
            // Ordenamos recursivamente la parte izquierda.
            Rapido(
                arreglo,
                inicio,
                i - 1
            );

            // Ordenamos recursivamente la parte derecha.
            Rapido(
                arreglo,
                i + 1,
                fin
            );
        }

        // ============================================================
        // COMPETIDOR 4: ARRAY.SORT
        // ORDENAMIENTO NATIVO DE .NET.
        // ============================================================

        // Este competidor utiliza la implementación optimizada
        // proporcionada por la biblioteca estándar de .NET.
        //
        // Se incluye como referencia para comparar nuestros algoritmos
        // con una solución disponible en el propio lenguaje.
        //
        // Su implementación interna puede utilizar diferentes
        // estrategias de ordenamiento.
        //
        // No implementamos el algoritmo internamente porque precisamente
        // queremos evaluar el método que ofrece .NET.
        //
        // La llamada se realiza desde Main mediante:
        // arreglo => Array.Sort(arreglo)

        // ============================================================
        // VERIFICACIÓN DEL RESULTADO
        // ============================================================

        // RÚBRICA:
        // Verificar que cada algoritmo deje el arreglo ordenado.
        //
        // Recorremos el arreglo comparando cada elemento con el anterior.
        // Si encontramos un elemento menor que el anterior,
        // significa que el algoritmo no ordenó correctamente.
        private static void VerificarOrdenamiento(
            int[] arreglo,
            string nombreAlgoritmo)
        {
            for (int i = 1; i < arreglo.Length; i++)
            {
                if (arreglo[i - 1] > arreglo[i])
                {
                    throw new Exception(
                        $"{nombreAlgoritmo} no ordenó correctamente " +
                        $"el arreglo en la posición {i}."
                    );
                }
            }
        }
    }
}

