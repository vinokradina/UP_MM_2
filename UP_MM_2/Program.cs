using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_MM_2
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите количество переменных:");
            int numVariables = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите количество ограничений:");
            int numConstraints = int.Parse(Console.ReadLine());

            double[] objectiveCoefficients = new double[numVariables];
            double[,] constraintsCoefficients = new double[numConstraints, numVariables];
            double[] rhs = new double[numConstraints];

            Console.WriteLine("Введите коэффициенты целевой функции (через пробел):");
            string[] objectiveInput = Console.ReadLine().Split(' ');
            for (int i = 0; i < numVariables; i++)
            {
                objectiveCoefficients[i] = double.Parse(objectiveInput[i]);
            }

            Console.WriteLine("Введите коэффициенты ограничений (построчно, через пробел):");
            for (int i = 0; i < numConstraints; i++)
            {
                string[] constraintInput = Console.ReadLine().Split(' ');
                for (int j = 0; j < numVariables; j++)
                {
                    constraintsCoefficients[i, j] = double.Parse(constraintInput[j]);
                }
            }

            Console.WriteLine("Введите правые части ограничений (через пробел):");
            string[] rhsInput = Console.ReadLine().Split(' ');
            for (int i = 0; i < numConstraints; i++)
            {
                rhs[i] = double.Parse(rhsInput[i]);
            }

            Simplex(objectiveCoefficients, constraintsCoefficients, rhs);

            Console.ReadLine();
        }

        static void Simplex(double[] objectiveCoefficients, double[,] constraintsCoefficients, double[] rhs)
        {
            int numVariables = objectiveCoefficients.Length;
            int numConstraints = rhs.Length;

            // Создаем симплекс-таблицу
            double[,] tableau = new double[numConstraints + 1, numVariables + numConstraints + 1];

            // Заполняем симплекс-таблицу
            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numVariables; j++)
                {
                    tableau[i, j] = constraintsCoefficients[i, j];
                }
                tableau[i, numVariables + i] = 1;
                tableau[i, numVariables + numConstraints] = rhs[i];
            }

            for (int j = 0; j < numVariables; j++)
            {
                tableau[numConstraints, j] = -objectiveCoefficients[j];
            }

            while (true)
            {
                // Находим входящий столбец (наименьший отрицательный элемент в последней строке)
                int enteringColumn = -1;
                double minValue = 0;
                for (int j = 0; j < numVariables + numConstraints; j++)
                {
                    if (tableau[numConstraints, j] < minValue)
                    {
                        minValue = tableau[numConstraints, j];
                        enteringColumn = j;
                    }
                }

                if (enteringColumn == -1)
                {
                    break; // Оптимальное решение найдено
                }

                // Находим выходящую строку (минимальное симплексное отношение)
                int leavingRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (tableau[i, enteringColumn] > 0)
                    {
                        double ratio = tableau[i, numVariables + numConstraints] / tableau[i, enteringColumn];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            leavingRow = i;
                        }
                    }
                }

                if (leavingRow == -1)
                {
                    Console.WriteLine("Задача не ограничена.");
                    return;
                }

                // Выполняем исключение Гаусса
                double pivot = tableau[leavingRow, enteringColumn];
                for (int j = 0; j <= numVariables + numConstraints; j++)
                {
                    tableau[leavingRow, j] /= pivot;
                }

                for (int i = 0; i <= numConstraints; i++)
                {
                    if (i != leavingRow)
                    {
                        double factor = tableau[i, enteringColumn];
                        for (int j = 0; j <= numVariables + numConstraints; j++)
                        {
                            tableau[i, j] -= factor * tableau[leavingRow, j];
                        }
                    }
                }
            }

            // Выводим результат
            Console.WriteLine("Оптимальное решение:");
            for (int j = 0; j < numVariables; j++)
            {
                bool isBasic = true;
                int basicRow = -1;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (tableau[i, j] == 1)
                    {
                        if (basicRow == -1)
                        {
                            basicRow = i;
                        }
                        else
                        {
                            isBasic = false;
                            break;
                        }
                    }
                    else if (tableau[i, j] != 0)
                    {
                        isBasic = false;
                        break;
                    }
                }

                if (isBasic && basicRow != -1)
                {
                    Console.WriteLine($"x{j + 1} = {Math.Round(tableau[basicRow, numVariables + numConstraints])}");
                }
                else
                {
                    Console.WriteLine($"x{j + 1} = 0");
                }
            }

            Console.WriteLine($"Максимальное значение целевой функции: {Math.Round(tableau[numConstraints, numVariables + numConstraints])}");
        }
    }
}
