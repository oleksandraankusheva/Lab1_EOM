using System;

namespace ClosedQueueingNetwork
{
    class Program
    {
        static void Main()
        {
            int n = 3;   // кількість вузлів
            int N = 6;   // кількість заявок

            // Матриця переходів
            double[,] P =
            {
                { 0.1, 0.3, 0.6 },
                { 1.0, 0.0, 0.0 },
                { 1.0, 0.0, 0.0 }
            };

            // Інтенсивності обслуговування μ
            double[] mu =
            {
                1.0 / 0.8,
                1.0 / 0.3,
                1.0 / 1.2
            };

            // ===============================
            // 1. Visit ratios e_i
            // ===============================
            double[] e = CalculateVisitRatios(P, n);

            Console.WriteLine("Відносні відвідування:");
            for (int i = 0; i < n; i++)
                Console.WriteLine($"e{i + 1} = {e[i]:F4}");

            // ===============================
            // 2. Bottleneck → throughput
            // ===============================
            double X = double.MaxValue;
            for (int i = 0; i < n; i++)
                X = Math.Min(X, mu[i] / e[i]);

            X = Math.Min(X, N); // обмеження замкненої мережі

            // ===============================
            // 3. λ, ρ, L
            // ===============================
            double[] lambda = new double[n];
            double[] rho = new double[n];
            double[] L = new double[n];

            for (int i = 0; i < n; i++)
            {
                lambda[i] = X * e[i];
                rho[i] = lambda[i] / mu[i];
                L[i] = rho[i] / (1.0 - rho[i]);
            }

            // ===============================
            // Вивід
            // ===============================
            Console.WriteLine("\n--- Результати ---");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Вузол K{i + 1}");
                Console.WriteLine($"  μ = {mu[i]:F4}");
                Console.WriteLine($"  λ = {lambda[i]:F4}");
                Console.WriteLine($"  ρ = {rho[i]:F4}");
                Console.WriteLine($"  L = {L[i]:F4}");
                Console.WriteLine();
            }
        }

        // Ітераційний підрахунок visit ratios
        static double[] CalculateVisitRatios(double[,] P, int n)
        {
            double[] e = new double[n];
            e[0] = 1.0;

            for (int iter = 0; iter < 100; iter++)
            {
                double[] next = new double[n];
                for (int j = 0; j < n; j++)
                    for (int i = 0; i < n; i++)
                        next[j] += e[i] * P[i, j];

                double norm = next[0];
                for (int j = 0; j < n; j++)
                    next[j] /= norm;

                e = next;
            }

            return e;
        }
    }
}
