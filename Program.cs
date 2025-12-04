using System;

namespace CafeteriaReservas
{
    class Program
    {
        // Constantes
        const int TURNOS = 2; // 0 = Mañana, 1 = Tarde
        const int MAX_RESERVAS = 20;

        // Menú de combos (nombre, precio)
        static string[] nombresCombos = { "Combo 1: Café + Pan", "Combo 2: Jugo + Sándwich", "Combo 3: Té + Muffin" };
        static double[] preciosCombos = { 4.00, 8.00, 1.20 };

        // Matriz de reservas: [turno, posición] -> (nombreEstudiante, comboIndex)
        static string[,] reservasEstudiantes = new string[TURNOS, MAX_RESERVAS]; // nombres
        static int[,] reservasCombos = new int[TURNOS, MAX_RESERVAS]; // índices de combo (-1 = vacío)

        static void Main(string[] args)
        {
            InicializarReservas();
            bool salir = false;

            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarReserva();
                        break;
                    case "2":
                        CancelarReserva();
                        break;
                    case "3":
                        ListarReservasPorTurno();
                        break;
                    case "4":
                        GenerarReportes();
                        break;
                    case "5":
                        BuscarReserva();
                        break;
                    case "6":
                        Console.WriteLine("¡Gracias por usar el sistema de reservas!");
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        break;
                }
            }
        }

        static void InicializarReservas()
        {
            for (int t = 0; t < TURNOS; t++)
            {
                for (int r = 0; r < MAX_RESERVAS; r++)
                {
                    reservasEstudiantes[t, r] = "";
                    reservasCombos[t, r] = -1;
                }
            }
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔═════════════════════════════════════╗");
            Console.WriteLine("║       CAFETERÍA UPN - RESERVAS    ║");
            Console.WriteLine("╚═════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine("1. Registrar reserva");
            Console.WriteLine("2. Cancelar reserva");
            Console.WriteLine("3. Listar reservas por turno");
            Console.WriteLine("4. Reportes (ingresos)");
            Console.WriteLine("5. Buscar reserva por nombre");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");


        }

        static void MostrarMenuCombos()
        {
            Console.WriteLine("\n--- Menú de Combos ---");
            for (int i = 0; i < nombresCombos.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {nombresCombos[i]} - ${preciosCombos[i]:F2}");
            }
        }

        static int ObtenerTurno()
        {
            Console.WriteLine("Turnos disponibles:");
            Console.WriteLine("1. Mañana");
            Console.WriteLine("2. Tarde");
            Console.Write("Elija turno (1 o 2): ");
            string input = Console.ReadLine();
            return input == "1" ? 0 : (input == "2" ? 1 : -1);
        }

        static int ObtenerCombo()
        {
            MostrarMenuCombos();
            Console.Write("Seleccione un combo (1-3): ");
            if (int.TryParse(Console.ReadLine(), out int combo) && combo >= 1 && combo <= nombresCombos.Length)
            {
                return combo - 1;
            }
            else
            {
                Console.WriteLine("Combo inválido.");
                return -1;
            }
        }

        static void RegistrarReserva()
        {
            Console.Clear();
            Console.WriteLine("=== Registrar Reserva ===");

            int turno = ObtenerTurno();
            if (turno == -1)
            {
                Console.WriteLine("Turno inválido.");
                Pausa();
                return;
            }

            // Verificar cupo
            int posicion = ObtenerPrimeraPosicionLibre(turno);
            if (posicion == -1)
            {
                Console.WriteLine("No hay cupo disponible en este turno.");
                Pausa();
                return;
            }

            Console.Write("Ingrese su nombre: ");
            string nombre = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("Nombre inválido.");
                Pausa();
                return;
            }

            int combo = ObtenerCombo();
            if (combo == -1) return;

            // Registrar
            reservasEstudiantes[turno, posicion] = nombre;
            reservasCombos[turno, posicion] = combo;
            Console.WriteLine($" Reserva registrada con éxito en {(turno == 0 ? "mañana" : "tarde")}.");
            Pausa();
        }

        static int ObtenerPrimeraPosicionLibre(int turno)
        {
            for (int i = 0; i < MAX_RESERVAS; i++)
            {
                if (reservasCombos[turno, i] == -1)
                    return i;
            }
            return -1; // lleno
        }

        static void CancelarReserva()
        {
            Console.Clear();
            Console.WriteLine("=== Cancelar Reserva ===");

            Console.Write("Ingrese su nombre: ");
            string nombre = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nombre)) return;

            bool encontrado = false;
            for (int t = 0; t < TURNOS && !encontrado; t++)
            {
                for (int r = 0; r < MAX_RESERVAS; r++)
                {
                    if (reservasEstudiantes[t, r] == nombre)
                    {
                        reservasEstudiantes[t, r] = "";
                        reservasCombos[t, r] = -1;
                        Console.WriteLine($"Reserva cancelada en {(t == 0 ? "mañana" : "tarde")}.");
                        encontrado = true;
                        break;
                    }
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("No se encontró una reserva con ese nombre.");
            }
            Pausa();
        }

        static void ListarReservasPorTurno()
        {
            Console.Clear();
            Console.WriteLine("=== Listar Reservas por Turno ===");

            int turno = ObtenerTurno();
            if (turno == -1) return;

            Console.WriteLine($"\nReservas en {(turno == 0 ? "Mañana" : "Tarde")}:\n");

            bool hayReservas = false;
            for (int r = 0; r < MAX_RESERVAS; r++)
            {
                if (reservasCombos[turno, r] != -1)
                {
                    string nombre = reservasEstudiantes[turno, r];
                    string combo = nombresCombos[reservasCombos[turno, r]];
                    Console.WriteLine("{r + 1}. {nombre} - {combo}");
                    hayReservas = true;
                }
            }

            if (!hayReservas)
            {
                Console.WriteLine("No hay reservas en este turno.");
            }
            Pausa();
        }

        static void GenerarReportes()
        {
            Console.Clear();
            Console.WriteLine("=== Reporte de Ingresos ===");

            double[] ingresos = new double[TURNOS];

            for (int t = 0; t < TURNOS; t++)
            {
                double total = 0;
                for (int r = 0; r < MAX_RESERVAS; r++)
                {
                    if (reservasCombos[t, r] != -1)
                    {
                        total += preciosCombos[reservasCombos[t, r]];
                    }
                }
                ingresos[t] = total;
                Console.WriteLine($"{(t == 0 ? "Mañana" : "Tarde")}: ${total:F2}");
            }

            double totalGeneral = ingresos[0] + ingresos[1];
            Console.WriteLine($"TOTAL DEL DÍA: ${totalGeneral:F2}");
            Pausa();
        }

        static void BuscarReserva()
        {
            Console.Clear();
            Console.WriteLine("=== Buscar Reserva por Nombre ===");
            Console.Write("Ingrese el nombre del estudiante: ");
            string nombre = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("Nombre inválido.");
                Pausa();
                return;
            }

            bool encontrado = false;
            for (int t = 0; t < TURNOS && !encontrado; t++)
            {
                for (int r = 0; r < MAX_RESERVAS; r++)
                {
                    if (reservasEstudiantes[t, r] == nombre)
                    {
                        string turnoStr = t == 0 ? "Mañana" : "Tarde";
                        string combo = nombresCombos[reservasCombos[t, r]];
                        Console.WriteLine($" Reserva encontrada:");
                        Console.WriteLine($"   Turno: {turnoStr}");
                        Console.WriteLine($"   Combo: {combo}");
                        encontrado = true;
                        break;
                    }
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("No se encontró ninguna reserva con ese nombre.");
            }
            Pausa();
        }

        static void Pausa()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}