
using System;
using System.Diagnostics;

class Program
{
    // ============================================================
    // INSTRUCCIÓN 1: IMPLEMENTAR UNA BÚSQUEDA LINEAL
    // Recorre el arreglo elemento por elemento hasta encontrar
    // el valor buscado.
    // Devuelve true si lo encuentra y false si no existe.
    // Complejidad temporal: O(n).
    // ============================================================
    static bool BuscarLineal(int[] datos, int buscado)
    {
        foreach (int elemento in datos)
        {
            if (elemento == buscado)
                return true;
        }

        return false;
    }

    static void Main()
    {
        // ============================================================
        // CONFIGURACIÓN DEL EXPERIMENTO
        // n: tamaño inicial del arreglo.
        // 4n: cuatro veces el tamaño inicial.
        // 16n: dieciséis veces el tamaño inicial.
        // ============================================================

        int n = 250_000;

        int[] tamanos =
        {
            n,
            n * 4,
            n * 16
        };

        // Cinco corridas independientes por cada tamaño.
        int corridas = 5;

        // Repeticiones internas para estabilizar cada medición.
        int repeticiones = 50;

        // Guardará el promedio anterior para calcular los ratios.
        double? promedioAnterior = null;

        Console.WriteLine("==============================================");
        Console.WriteLine("       EXPERIMENTO: BUSQUEDA LINEAL");
        Console.WriteLine("==============================================");
        Console.WriteLine($"Tamaño inicial (n): {n:N0}");
        Console.WriteLine($"Corridas por tamaño: {corridas}");
        Console.WriteLine($"Repeticiones por corrida: {repeticiones}");
        Console.WriteLine();

        // ============================================================
        // INSTRUCCIÓN 2: MEDIR CON n, 4n Y 16n
        // El foreach ejecuta automáticamente el experimento
        // con los tres tamaños definidos anteriormente.
        // ============================================================

        Console.WriteLine("TABLA DE MEDICIONES");
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine(
            $"{"Tamaño",12} | {"Promedio (ms)",14} | {"Ratio",10}");
        Console.WriteLine("------------------------------------------------------------");

        foreach (int cantidad in tamanos)
        {
            // Crear el arreglo fuera de la medición.
            // Así no contabilizamos el tiempo de creación del arreglo.
            int[] datos = new int[cantidad];

            // Llenar el arreglo con valores consecutivos.
            for (int i = 0; i < cantidad; i++)
            {
                datos[i] = i;
            }

            // Buscamos -1, que no existe en el arreglo.
            // Esto obliga a recorrer todos los elementos:
            // estamos midiendo el peor caso de la búsqueda lineal.
            int buscado = -1;

            // ========================================================
            // INSTRUCCIÓN 3: CALENTAMIENTO (WARM-UP)
            // Ejecutamos una búsqueda antes de medir.
            // Esta ejecución NO se incluye en los resultados.
            // ========================================================

            BuscarLineal(datos, buscado);

            double tiempoTotal = 0;

            // ========================================================
            // INSTRUCCIÓN 4: REALIZAR CINCO CORRIDAS
            // Cada corrida mide 50 búsquedas consecutivas.
            // Después dividimos el tiempo entre 50 para obtener
            // el tiempo promedio de una sola búsqueda.
            // ========================================================

            for (int corrida = 0; corrida < corridas; corrida++)
            {
                Stopwatch reloj = Stopwatch.StartNew();

                bool resultado = false;

                for (int r = 0; r < repeticiones; r++)
                {
                    resultado = BuscarLineal(datos, buscado);
                }

                reloj.Stop();

                // Evita que el resultado de las búsquedas
                // sea considerado innecesario por el optimizador.
                GC.KeepAlive(resultado);

                // Tiempo promedio de una búsqueda en esta corrida.
                double tiempoPorBusqueda =
                    reloj.Elapsed.TotalMilliseconds / repeticiones;

                // Acumulamos los tiempos de las cinco corridas.
                tiempoTotal += tiempoPorBusqueda;

                // Mostrar el resultado individual de cada corrida.
                Console.WriteLine(
                    $"  Corrida {corrida + 1}: {tiempoPorBusqueda:F4} ms");
            }

            // ========================================================
            // INSTRUCCIÓN 5: CALCULAR EL PROMEDIO
            // Sumamos los tiempos de las cinco corridas y dividimos
            // entre cinco para obtener el promedio final.
            // ========================================================

            double promedio = tiempoTotal / corridas;

            // ========================================================
            // INSTRUCCIÓN 6: CALCULAR LOS RATIOS
            // Comparamos el promedio actual con el anterior.
            //
            // Ratio = Tiempo actual / Tiempo anterior
            //
            // Si el algoritmo es O(n), esperamos ratios cercanos
            // a x4 al pasar de n a 4n y de 4n a 16n.
            // ========================================================

            string ratio = promedioAnterior.HasValue
                ? $"x{promedio / promedioAnterior.Value:F2}"
                : "---";

            // Mostrar el resultado de este tamaño en la tabla.
            Console.WriteLine("------------------------------------------------------------");

            Console.WriteLine(
                $"{cantidad,12:N0} | {promedio,14:F4} | {ratio,10}");

            // Guardar el promedio para la siguiente comparación.
            promedioAnterior = promedio;

            Console.WriteLine();
        }

        Console.WriteLine("==============================================");
        Console.WriteLine("FIN DEL EXPERIMENTO");
        Console.WriteLine("==============================================");
    }
}
