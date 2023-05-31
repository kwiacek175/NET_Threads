using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppThreads
{
    internal class Program
    {
        static int a_row, a_column, b_row, b_column, i, j;
        static int[,] Matrix_a, Matrix_b, Matrix_c;

        static void GenerateMatrices()
        {
            Console.WriteLine("\nPodaj liczbe wierszy macierzy A: ");
            a_row = int.Parse(Console.ReadLine());

            Console.WriteLine("\nPodaj liczbe kolumn macierzy A: ");
            a_column = int.Parse(Console.ReadLine());

            Console.WriteLine("\nPodaj liczbe wierszy macierzy B: ");
            b_row = int.Parse(Console.ReadLine());

            Console.WriteLine("\nPodaj liczbe kolumn macierzy B: ");
            b_column = int.Parse(Console.ReadLine());

            if (a_column != b_row)
            {
                Console.Write("Mnozenie takich macierzy jest niemozliwe. Podaj rozmiary w postaci: A: mxn, B: nxl\n");
                GenerateMatrices();
            }
            else
            {
                Matrix_a = new int[a_row, a_column];
                Matrix_b = new int[b_row, b_column];
                Matrix_c = new int[a_row, b_column];

                for (i = 0; i < a_row; i++)
                {
                    for (j = 0; j < a_column; j++)
                    {
                        Matrix_a[i, j] = j + 1 + i;           // Kazdy element macierzy wynosi: numer kolumny elementu + 1 + numer wierszu, w ktorym znajduje sie element,
                    }                                         // przy czym numerowanie kolumn i wierszy zaczyna sie od 0!!!
                }

                Console.WriteLine();
                //Console.WriteLine("Macierz A:\n");          //Wyświetlanie macierzy działa, lecz dla dużych macierzy trwało by to zbyt długo 
                //PrintMatrix(Matrix_a);

                for (i = 0; i < b_row; i++)
                {
                    for (j = 0; j < b_column; j++)
                    {
                        Matrix_b[i, j] = j + 1 + i;           // Kazdy element macierzy wynosi: numer kolumny elementu + 1 + numer wierszu, w ktorym znajduje sie element,
                    }                                         // przy czym numerowanie kolumn i wierszy zaczyna sie od 0!!!
                }

                //Console.WriteLine("Macierz B:\n");          //Wyświetlanie macierzy działa, lecz dla dużych macierzy trwało by to zbyt długo
                //PrintMatrix(Matrix_b);

            }
        }

        static void PrintMatrix(int[,] matrix)
        {
            for (i = 0; i < matrix.GetLength(0); i++)
            {
                for (j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }


        static void Main(string[] args)
        {

            GenerateMatrices();
            Test test = new Test();

            //---------------------------------------------------------------Pomiar mnożenia macierzy bez wątków------------------------------------------------------------
            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            //for(i =0; i < a_row; i++)
            //{
            //    test.matrix_multiplication(i);
            //}
            test.matrix_multiplication(0, a_row);                                                  //druga metoda 
            watch1.Stop();
            var elapsedMs1 = watch1.ElapsedMilliseconds;


            //---------------------------------------------------------------Pomiar mnożenia macierzy z wątkami--------------------------------------------------------------
            //Thread[] threads = new Thread[a_row];
            Thread[] threads = new Thread[4];                                                     //druga metoda
            int start = 0;                                                                        //druga metoda
            int size = a_row / threads.Length;                                                    //druga metoda                                              

            for (i = 0; i < threads.Length; i++)                                                  //druga metoda
            //for (i = 0; i < a_row; i++)
            {
                int tmp = start;
                threads[i] = new Thread(() => test.matrix_multiplication(tmp, tmp + size));      //druga metoda
                //threads[i] = new Thread(() => test.matrix_multiplication(i));
                threads[i].Start();
                start += size;
            }
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            //for (i = 0; i < a_row; i++)
            //    threads[i].Start();
            for (i = 0; i < threads.Length; i++)      //druga metoda
                threads[i].Join();
           // for (i = 0; i < a_row; i++)
           //     threads[i].Join();
            watch2.Stop();
            var elapsedMs2 = watch2.ElapsedMilliseconds;

            //Console.WriteLine("Macierz wynikowa C:\n");                                       //Wyświetlanie macierzy działa, lecz dla dużych macierzy trwało by to zbyt długo
            //PrintMatrix(Matrix_c);
            Console.WriteLine("Czas operacji bez użycia wątków: " + elapsedMs1 + " ms");
            Console.WriteLine("czas operacji z użyciem wątków: " + elapsedMs2 + " ms");
            Console.WriteLine("Operacja wykonana z pomocą wątków wykonała się około {0} razy szybciej", (elapsedMs1 / elapsedMs2));                         //Brak zaokrąglenia 
            Console.Read();

        }
        public class Test
        {

            //Metoda 1 -> każdy wątek liczy swój wiersz, dla dużych macierzy jest nieefektywna, gdyż system potrzebuje wiecej czasu na zarzadzanie wątkami,komkurencja wątków o zasoby
            public void matrix_multiplication(int j)
            {
                for (int i = 0; i < b_column; i++)
                {
                    Matrix_c[j, i] = 0;
                    for (int k = 0; k < a_column; k++)
                    {
                        Matrix_c[j, i] += Matrix_a[j, k] * Matrix_b[k, i];
                    }
                }
            }

            public void matrix_multiplication(int start_row, int end_row)  //Metoda 2 -> Wątki dzielą się po równo wierszami w tym przypadku będą tylko 4 wątki
            {
                for (int j = start_row; j < end_row; j++)
                {
                    for (int i = 0; i < b_column; i++)
                    {
                        Matrix_c[j, i] = 0;
                        for (int k = 0; k < a_column; k++)
                        {
                            Matrix_c[j, i] += Matrix_a[j, k] * Matrix_b[k, i];
                        }
                    }
                }
            }
        }
    }
}