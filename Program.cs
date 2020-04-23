using System;
using System.Collections.Generic;
using System.IO;


namespace ASLecture4
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("************* Лабораторные работы по 5 лекции *************\n" +
                "************* Реализация алгоритмов Бойера-Мура и Карпа-Рабина *************\n");

            // считываем исходный текст из файла в переменную TextInFile

            Console.WriteLine("Введите название файла в котором будет происходить поиск: ");


            try
            {   // чтение данных из файла
                using (StreamReader sr = new StreamReader(Console.ReadLine()))
                {
                    string TextInFile = sr.ReadToEnd();
                    Console.WriteLine("Текст файла: ");
                    Console.WriteLine(TextInFile);
                    // метод, который демонстрирует работу алгоритмов
                    Realization(TextInFile);

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine("Файл не найден");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Если хотите ввести текст вручную, напишите 1");

                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Введите текст для работы с алгоритмами");
                    string Text = Console.ReadLine();
                    Realization(Text);
                }


            }





            Console.Read();
        }




        public static void Realization(string Text)
        {
            string SearchingText;

            Console.WriteLine("Полученный текст: " + Text);
            Console.WriteLine("Введите текст, которые хотите искать в " + Text);
            SearchingText = Console.ReadLine();

            Console.WriteLine("Демонстрация работы алгоритма Бойера-Мура");
            BM(SearchingText, Text, false);

            Console.WriteLine("Демонстрация работы алгоритма Карпа-Рабина");
            СarpRabin(Text, SearchingText, 8);

            Console.WriteLine();
        }


        // Метод(вспомогательный) для вывода на экран содержимого массива массива граней и т.п.
        public static void Show(int[] Array)
        {
            for (int i = 0; i < Array.Length; ++i)
            {
                Console.Write(Array[i] + " ");
            }
            Console.WriteLine();
        }

        

        // Формирование словаря списков позиций
        // SearchingTextLength - длина входного образца (SearchingText)
        // DictionaryOfCharEntered - параметр, содержащий для каждого символа алфавита список позиций его вхождений в образец
        public static Dictionary<char, List<int>> PositionList(string SearchingText)
        {
            Dictionary<char, List<int>> DictionaryOfCharEntered = new Dictionary<char, List<int>>();

            int SearchingTextLength = SearchingText.Length;
            for (int k = SearchingTextLength - 1; k >= 0; --k)
            {
                try
                {
                    DictionaryOfCharEntered[SearchingText[k]].Add(k);
                }
                catch
                {
                    DictionaryOfCharEntered[SearchingText[k]] = new List<int>();
                    DictionaryOfCharEntered[SearchingText[k]].Add(k);
                }

            }
            return DictionaryOfCharEntered;
        }


        // Вычисление сдвига по плохому символу
        // DictionaryOfCharEntered - словарь, содержащий для каждого символа алфавита список позиций его вхождений в образец
        // CharBad - символ, предшествующий не совпавшему
        // PosBad - индекс, на котором начало образца
        // ListOfSpecificCharEntered - список позиций данного символа CharBad
        // nPos - искомая позиция слева от плохого символа
        public static int BadCharShift(Dictionary<char, List<int>> DictionaryOfCharEntered, char CharBad, int PosBad)
        {
            if (PosBad < 0)
                return 1; // Образец совпал – сдвиг на 1
            int nPos = -1;

            if (DictionaryOfCharEntered.ContainsKey(CharBad))
            {   // Список не пуст
                List<int> ListOfSpecificCharEntered = DictionaryOfCharEntered[CharBad];
                // Ищем элемент, меньший чем плохая позиция
                foreach (int objList in ListOfSpecificCharEntered)
                {
                    if (objList < PosBad)
                    {
                        nPos = objList;
                        break;
                    }
                }
            }
            return PosBad - nPos;
        }

        // Метод, который реализует алгоритм Бойера-Мура
        // OriginalText - входной текст 
        // SearchingTexth - искомая подстрока
        // RightBorder - правая граница «прикладывания» образца
        // DictionaryOfCharEntered - словарь, содержащий для каждого символа алфавита список позиций его вхождений в образец
        public static void BM(string SearchingText, string OriginalText, bool h)
        {
            int count = 0;
            Dictionary<char, List<int>> pl = PositionList(SearchingText);
            // Длина искомой строки
            int SearchingTextLength = SearchingText.Length;
            // Длина входного текста 
            int OriginalTextLength = OriginalText.Length;

            int[] bs = new int[SearchingTextLength];
            int[] ns = new int[SearchingTextLength];
            int[] br = new int[SearchingTextLength];
            SuffixBorderArray(SearchingText, bs);
            BSToBR(bs, br, SearchingTextLength);
            
           
            int[] bxc = new int[SearchingTextLength];
            if(h)
            { 
                BSToBSM(bs, bs, SearchingTextLength); 
            } 
            BSToNS(bs, ns, SearchingTextLength);
            // Поиск вхождений
            int RightBorder = SearchingTextLength;
            while (RightBorder <= OriginalTextLength)
            { // Сравнение образца с текстом справа налево
                // Итератор по искомой строке
                int IterOfSearchingText = SearchingTextLength - 1;
                // Итератор по подстроке входного текста
                int IterOfSubOriginalText = RightBorder - 1;
                for (; IterOfSearchingText >= 0; --IterOfSearchingText, --IterOfSubOriginalText)
                    if (SearchingText[IterOfSearchingText] != OriginalText[IterOfSubOriginalText])
                        break;// OriginalText[i] – плохой символ
                              // Результаты сравнения


                if (IterOfSearchingText < 0)
                {
                    Console.WriteLine("Точка вхождения " + (IterOfSubOriginalText + 1));
                    count++;
                }
            
                RightBorder += Math.Max(
                   BadCharShift(pl, OriginalText[Math.Max(0, IterOfSubOriginalText)], IterOfSearchingText),
                   GoodSuffixShift(ns, br, IterOfSearchingText, SearchingTextLength));

            }
            if (count == 0)
            {
                Console.WriteLine("Вхождений искомой подстроки нет");
            }
        }


        // Ближайшие суффиксы (nearest suffix)
        // массив ближайших повторных вхождений суффиксов
        private static void BSToNS(int[] bs, int[] ns, int SearchingTextLength)
        {

            for (int i = 0; i < SearchingTextLength; i++)
                ns[i] = -1; // Фиктивное значение

            for (int j = 0; j < SearchingTextLength - 1; j++)
                if (bs[j] != 0)
                    ns[SearchingTextLength - bs[j] - 1] = j;

        }
        // borders restricted, наибольшая грань подстроки, не превосходящая m-k-1
        private static void BSToBR(int[] bs, int[] br, int SearchingTextLength)
        {
            int currBorder = bs[0], k = 0;
            while (currBorder != 0)
            {
                for (; k < SearchingTextLength - currBorder; k++)
                    br[k] = currBorder;

                currBorder = bs[k];
            }

            for (; k < SearchingTextLength; k++)
                br[k] = 0;

        }

        // Cдвиг по правилу хорошего суффикса
        private static int GoodSuffixShift(int[] ns, int[] br, int posBad, int SearchingTextLength)
        {
            if (posBad == SearchingTextLength - 1)
                return 1;               // Хорошего суффикса нет

            if (posBad < 0)
                return SearchingTextLength - br[0];   // Образец совпал – сдвиг по наиб. грани

            int copyPos = ns[posBad];  // Вхождение левой копии суффикса
            return copyPos >= 0 ? posBad - copyPos + 1 : SearchingTextLength - br[posBad];     // Cдвиг по ограниченной наибольшей грани
        }

        // алгоритм построения массива суффиксов
        static void SuffixBorderArray(string TextInFile, int[] bs)
        {
            int n = TextInFile.Length;
            bs[n - 1] = 0;
            for (int i = n - 2; i >= 0; --i)
            {
                int bsLeft = bs[i + 1]; // Позиция с конца слева от предыдущей грани
                while (bsLeft != 0 && (TextInFile[i] != TextInFile[n - bsLeft - 1]))
                    bsLeft = bs[n - bsLeft];
                // Длина на 1 больше, чем позиция
                if (TextInFile[i] == TextInFile[n - bsLeft - 1])
                    bs[i] = bsLeft + 1;
                else bs[i] = 0;
            }
        }
        // алгоритм построения модифицированного массива суффиксов
        // при построение не используется исходный текст
        static void BSToBSM(int[] bs, int[] bsm, int n)
        {
            bsm[n - 1] = 0; bsm[0] = bs[0];
            for (int i = n - 2; i > 0; --i)
            {
                // Проверка совпадения предыдущих символов
                if (bs[i] != 0 && (bs[i] + 1 == bs[i - 1])) bsm[i] = bsm[n - bs[i]];
                else bsm[i] = bs[i];
            }
        }




        //________________Алгоритм Карпа-Рабина______________


        public static void СarpRabin(string OriginalText, string SearchingText, int primeNumber)
        {
            // инициализация
            int OriginalTextLength = OriginalText.Length;
            int SearchingTextLength = SearchingText.Length;
            int p2m = 1;

            for (int i = 0; i < SearchingTextLength - 1; i++)
                p2m = (p2m * 2) % primeNumber;

            // вычисление хэша от подстроки и куска строки
            int HashSearchingText = gorner2mod(SearchingText, SearchingTextLength, primeNumber);
            int HashOriginalText = gorner2mod(OriginalText, SearchingTextLength, primeNumber);


            // поиск вхождений
            for (int j = 0; j <= OriginalTextLength - SearchingTextLength; j++)
            {
                if (HashOriginalText == HashSearchingText)
                { // проверка действительно ли совпали строки (вдруг коллизия)
                    int k = 0;
                    // наивное сравнение строк
                    while (k < SearchingTextLength && SearchingText[k] == OriginalText[j + k]) 
                        k++;

                    if (k == SearchingTextLength)
                        Console.WriteLine("Совпадение с позиции: " + j);
                }

                // чтобы не выйти за границы строки
                if (j < OriginalTextLength - SearchingTextLength)
                {
                    // хэш = хэш - первый символ + последний символ по формуле
                    HashOriginalText = ((HashOriginalText - p2m * OriginalText[j]) * 2 + OriginalText[j + SearchingTextLength]) % primeNumber;
                    if (HashOriginalText < 0) // конвертация в положительное если вышло < 0
                        HashOriginalText += primeNumber;
                }
            }

        }

        // вычисляет по схеме Горнера значение многочлена степени m с коэффициентами S[0..m-1] по модулю q
        private static int gorner2mod(String str, int m, int q)
        {
            int res = 0;
            for (int i = 0; i < m; i++)
                res = (res * 2 + str[i]) % q;

            return res;
        }






    }
}