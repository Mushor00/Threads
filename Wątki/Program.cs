using System;
using System.Threading;

class Program
{
    // Zmienna współdzielona
    private static int? sharedVariable = 0;
    private static int sum = 0;
    private static readonly object lockObject = new object();

    static void Main()
    {
        // Ustawienie zakończenia programu po 30 sekundach
        Timer timer = new Timer(_ => Environment.Exit(0), null, 30000, Timeout.Infinite);

        // Tworzenie wątków z odpowiednimi nazwami
        Thread t1 = new Thread(() => Writer(21, 37)) { Name = "114734#Writer#1" };
        Thread t2 = new Thread(() => Writer(1337, 4200)) { Name = "114734#Writer#2" };
        Thread t3 = new Thread(Reader) { Name = "114734#Reader#1" };
        Thread t4 = new Thread(Monitor) { Name = "114734#Monitor#1" };

        t4.IsBackground = true;

        // Uruchomienie wątków
        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();

        // Oczekiwanie na zakończenie wątków
        t1.Join();
        t2.Join();
        t3.Join();
    }

    // Wątek zapisujący losową liczbę do zmiennej współdzielonej
    static void Writer(int min, int max)
    {
        Random random = new Random();
        while (true)
        {
            lock (lockObject)
            {
                if (sharedVariable == null)
                {
                    sharedVariable = random.Next(min, max + 1);
                    Console.WriteLine($"{Thread.CurrentThread.Name} zapisuje wartość {sharedVariable}");
                }
            }
            Thread.Sleep(500);
        }
    }

    // Wątek odczytujący wartość zmiennej współdzielonej i sumujący wartości
    static void Reader()
    {
        while (true)
        {
            lock (lockObject)
            {
                if (sharedVariable != null)
                {
                    sum += sharedVariable.Value;
                    Console.WriteLine($"{Thread.CurrentThread.Name} odczytuje wartość {sharedVariable}, suma: {sum}");
                    sharedVariable = null;
                }
            }
            Thread.Sleep(1000);
        }
    }

    // Wątek monitorujący stan zmiennej współdzielonej i innych wątków
    static void Monitor()
    {
        while (true)
        {
            lock (lockObject)
            {
                string state = sharedVariable.HasValue ? sharedVariable.ToString() : "null";
                Console.WriteLine($"{Thread.CurrentThread.Name} monitoruje stan: sharedVariable = {state}");
            }
            Thread.Sleep(1000);
        }
    }
}
