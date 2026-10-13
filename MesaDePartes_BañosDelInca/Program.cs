using System.IO;
using System;

namespace MesaDePartes_BañosDelInca
{
    class Program
    {
        const int MAX_EXPEDIENTES = 100;
        static string[] codigos = new string[MAX_EXPEDIENTES];
        static string[] ciudadanos = new string[MAX_EXPEDIENTES];
        static string[] asuntos = new string[MAX_EXPEDIENTES];
        static int totalExpedientes = 0;
        static string FILE_PATH = "expedientes_banos.txt";

        static void Main(string[] args)
        {
            // CARGA AUTOMÁTICA: Lee el archivo de texto antes de abrir el menú
            CargarDatosDesdeArchivo();

            int opcion;
            do
            {
                Console.Clear();
                Console.WriteLine("====================================================");
                Console.WriteLine("     MUNICIPALIDAD DISTRITAL DE BAÑOS DEL INCA      ");
                Console.WriteLine("         SISTEMA DE MESA DE PARTES DIGITAL          ");
                Console.WriteLine("====================================================");
                Console.WriteLine(" 1. Registrar nuevo expediente");
                Console.WriteLine(" 2. Buscar expediente por código");
                Console.WriteLine(" 3. Ordenar y mostrar expedientes");
                Console.WriteLine(" 4. Salir de la aplicación");
                Console.WriteLine("====================================================");
                Console.Write("Seleccione una opción (1-4): ");

                if (!int.TryParse(Console.ReadLine(), out opcion))
                    opcion = 0;

                switch (opcion)
                {
                    case 1:
                        RegistrarExpediente();
                        break;
                    case 2:
                        BuscarExpediente();
                        break;
                    case 3:
                        OrdenarYMostrarExpedientes();
                        break;
                    case 4:
                        Console.WriteLine("\nCerrando el sistema... ¡Hasta luego!");
                        break;
                }
            } while (opcion != 4);
        }

        static void RegistrarExpediente()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRO DE NUEVO EXPEDIENTE ---");

            if (totalExpedientes >= MAX_EXPEDIENTES)
            {
                Console.WriteLine("⚠ Alerta: Capacidad máxima de la memoria llena.");
                Console.ReadKey();
                return;
            }

            string codigo = "";
            while (true)
            {
                Console.Write("Ingrese código del expediente (ej. MDBI-001): ");
                codigo = Console.ReadLine().Trim().ToUpper();
                if (!string.IsNullOrEmpty(codigo)) break;
                Console.WriteLine("❌ Error: El código de identificación es obligatorio.");
            }

            Console.Write("Ingrese nombre completo del ciudadano solicitante: ");
            string ciudadano = Console.ReadLine().Trim();

            Console.Write("Ingrese el asunto o descripción del trámite: ");
            string asunto = Console.ReadLine().Trim();

            // Guardado en arreglos paralelos
            codigos[totalExpedientes] = codigo;
            ciudadanos[totalExpedientes] = ciudadano;
            asuntos[totalExpedientes] = asunto;
            totalExpedientes++;

            // GUARDADO AUTOMÁTICO: Respalda en el archivo de texto tras cada registro exitoso
            GuardarDatosEnArchivo();

            Console.WriteLine("\n✔ El expediente se ha registrado con éxito y se guardó en el archivo.");
            Console.ReadKey();
        }

        static void BuscarExpediente()
        {
            Console.Clear();
            Console.WriteLine("--- BÚSQUEDA DE TRÁMITES EN MESA DE PARTES ---");
            Console.Write("Ingrese el código exacto a buscar: ");
            string busqueda = Console.ReadLine().Trim().ToUpper();

            int indiceEncontrado = -1;

            for (int i = 0; i < totalExpedientes; i++)
            {
                if (codigos[i].Equals(busqueda, StringComparison.OrdinalIgnoreCase))
                {
                    indiceEncontrado = i;
                    break;
                }
            }

            if (indiceEncontrado != -1)
            {
                Console.WriteLine("\n📌 EXPEDIENTE ENCONTRADO EN BASE DE DATOS:");
                Console.WriteLine($"Código: {codigos[indiceEncontrado]}");
                Console.WriteLine($"Ciudadano: {ciudadanos[indiceEncontrado]}");
                Console.WriteLine($"Asunto: {asuntos[indiceEncontrado]}");
            }
            else
            {
                Console.WriteLine("\n❌ Alerta: No se encontró ningún expediente con el código ingresado.");
            }
            Console.ReadKey();
        }

        static void OrdenarYMostrarExpedientes()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO DE EXPEDIENTES REGISTRADOS ---");

            if (totalExpedientes == 0)
            {
                Console.WriteLine("No existen registros procesados en el sistema.");
                Console.ReadKey();
                return;
            }

            // Algoritmo de ordenamiento Burbuja (Bubble Sort)
            for (int i = 0; i < totalExpedientes - 1; i++)
            {
                for (int j = 0; j < totalExpedientes - i - 1; j++)
                {
                    if (string.Compare(codigos[j], codigos[j + 1], StringComparison.Ordinal) > 0)
                    {
                        // Intercambio de posiciones (SWAP)
                        string tempCod = codigos[j];
                        codigos[j] = codigos[j + 1];
                        codigos[j + 1] = tempCod;

                        string tempCiu = ciudadanos[j];
                        ciudadanos[j] = ciudadanos[j + 1];
                        ciudadanos[j + 1] = tempCiu;

                        string tempAsu = asuntos[j];
                        asuntos[j] = asuntos[j + 1];
                        asuntos[j + 1] = tempAsu;
                    }
                }
            }

            // Tras ordenar internamente los arreglos, actualizamos el archivo físico para mantener sincronía
            GuardarDatosEnArchivo();

            // Despliegue en pantalla
            for (int i = 0; i < totalExpedientes; i++)
            {
                Console.WriteLine($"[{codigos[i]}] Ciudadano: {ciudadanos[i]} | Asunto: {asuntos[i]}");
                Console.WriteLine("----------------------------------------------------");
            }
            Console.ReadKey();
        }

        static void GuardarDatosEnArchivo()
        {
            using (StreamWriter sw = new StreamWriter(FILE_PATH, false))
            {
                for (int i = 0; i < totalExpedientes; i++)
                {
                    sw.WriteLine($"{codigos[i]}|{ciudadanos[i]}|{asuntos[i]}");
                }
            }
        }

        static void CargarDatosDesdeArchivo()
        {
            if (!File.Exists(FILE_PATH)) return;

            using (StreamReader sr = new StreamReader(FILE_PATH))
            {
                string linea;
                totalExpedientes = 0;

                while ((linea = sr.ReadLine()) != null && totalExpedientes < MAX_EXPEDIENTES)
                {
                    string[] campos = linea.Split('|');
                    if (campos.Length == 3)
                    {
                        // CORRECCIÓN: Asignar el índice correspondiente de la matriz generada por Split
                        codigos[totalExpedientes] = campos[0];
                        ciudadanos[totalExpedientes] = campos[1];
                        asuntos[totalExpedientes] = campos[2];
                        totalExpedientes++;
                    }
                }
            }
        }
    }
}
