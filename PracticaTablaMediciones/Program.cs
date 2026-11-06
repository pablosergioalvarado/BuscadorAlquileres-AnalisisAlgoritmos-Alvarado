using System;
using System.Diagnostics;

class Program
{
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
        int n = 250_000;

        int[] tamanos =
        {
            n,
            n * 4,
            n * 16
        };

        int repeticiones = 50;
        int corridas = 5;

        double? promedioAnterior = null;

        Console.WriteLine("BUSQUEDA LINEAL - MEDICIONES");
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine("Datos\t\tPromedio (ms)\tRatio vs anterior");

        foreach (int cantidad in tamanos)
        {
            // Preparar los datos fuera de la medición
            int[] datos = new int[cantidad];

            for (int i = 0; i < cantidad; i++)
                datos[i] = i;

            // El -1 no existe: obliga a recorrer todo el arreglo
            int buscado = -1;

            // Calentamiento: primera ejecución no contabilizada
            BuscarLineal(datos, buscado);

            double tiempoTotal = 0;

            // Cinco corridas medidas
            for (int corrida = 0; corrida < corridas; corrida++)
            {
                Stopwatch reloj = Stopwatch.StartNew();

                bool resultado = false;

                // Repetimos la búsqueda para obtener una medición estable
                for (int r = 0; r < repeticiones; r++)
                {
                    resultado = BuscarLineal(datos, buscado);
                }

                reloj.Stop();

                GC.KeepAlive(resultado);

                // Tiempo promedio de una búsqueda
                double tiempoPorBusqueda =
                    reloj.Elapsed.TotalMilliseconds / repeticiones;

                tiempoTotal += tiempoPorBusqueda;
            }

            // Promedio de las cinco corridas
            double promedio = tiempoTotal / corridas;

            string ratio = promedioAnterior.HasValue
                ? $"x{promedio / promedioAnterior.Value:F2}"
                : "---";

            Console.WriteLine(
                $"{cantidad:N0}\t\t{promedio:F4}\t\t{ratio}");

            promedioAnterior = promedio;
        }
    }
}
