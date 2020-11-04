﻿using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace VRP
{

    public static class vech
    {
        //public const int smallVechNum = 10;
        public const int bigVechNum = 2;

       public const double sMaxWeig = 8000;
       public const double sMaxLeng = 7.8;

       public const double bMaxWeig = 24000;
       public const double bMaxLeng = 16.6;
    }

 
    class Program
    {

        static Stack VRP(List<List<double>> citiesTab, List<List<double>> data, int hub, int citiesNum)
        {
            double maxDist = 1;
            double minDist = Int32.MaxValue;
            //List<List<double>> citiesTabHelp = citiesTab;   
            Stack perm = new Stack();   //permutacja odwiedzonych miast
            double[,] vechNum = new double[citiesNum, 2];   //tablica zawierajaca informacje na temat aktualnego ladunku poszczegolnych pojazdow

            int k = 0;  //pojazdy
            bool limit = true;
            int index = 0;
            int minDistIndx = 0;

            double vechWeig = vech.bMaxWeig;    //ladownosc aktulnego typu pojazdu
            double vechLeng = vech.bMaxLeng;

            perm.Push(hub);

            for(int i = 0; i < citiesNum; i++)
            {
                vechNum[i, 0] = 0;
                vechNum[i, 1] = 0;
            }

            for (int b = 0; b < citiesNum; b++) 
            while(maxDist != 0)
            {
                maxDist = 0;
                for (int i = 0; i < citiesNum; i++)
                {
                    
                    if (maxDist < citiesTab[hub][i])    //wybieramy najbardziej oddalone miasto od huba
                    {
                    maxDist = citiesTab[hub][i];
                    index = i;
                    }
                }

                if (maxDist != 0)   //jezeli maxDist = 0 to oznacza to, ze skonczyly sie miasta w zbiorze dostepnych miast
                {
                    perm.Push(index);
                    citiesTab[hub][index] = 0;
                    citiesTab[index][hub] = 0;
                    vechNum[k, 0] = data[index][1];
                    vechNum[k, 1] = data[index][2];
                    
                    while (limit == true)    //wybieramy miasta najblizej poprzednio wybranego miasta i wliczamy je jezeli mozemy pomiescic towar
                    {
                        for(int i = 0; i < citiesNum; i++)
                        {
                            if( minDist > citiesTab[i][index] && citiesTab[i][hub] != 0)
                            {
                                minDist = citiesTab[i][index];
                                minDistIndx = i;
                            }
                        }
                        minDist = Int32.MaxValue;

                        if ((vechNum[k, 0] + data[minDistIndx][1]) <= vechLeng && (vechNum[k, 1] + data[minDistIndx][2]) <= vechWeig && citiesTab[minDistIndx][hub] != 0)   //jezeli towar spelnia wymogi i miasto nie zostalo wczesniej odwiedzone to jest wrzucane do permutacji
                        {
                            perm.Push(minDistIndx);
                            citiesTab[minDistIndx][hub] = 0;
                            citiesTab[hub][minDistIndx] = 0;
                            vechNum[k, 0] = vechNum[k, 0] + data[minDistIndx][1];
                            vechNum[k, 1] = vechNum[k, 1] + data[minDistIndx][2];
                        }
                        else
                        {
                                //Uwzglednienie mozliwosci odwiedzenia innych miast, ktorych towar zmiesci sie jeszcze do pojazdu
                                
                                while (limit == true)
                                {
                                    for (int i = 0; i < citiesNum; i++)
                                    {
                                        if ((vechNum[k, 0] + data[i][1]) <= vechLeng && (vechNum[k, 1] + data[i][2]) <= vechWeig && citiesTab[i][hub] != 0 && minDist > citiesTab[minDistIndx][i] && citiesTab[minDistIndx][i] < citiesTab[minDistIndx][hub])
                                        {
                                            minDistIndx = i;
                                            minDist = citiesTab[i][hub];
                                            
                                        }
                                    }
                                    if (minDist == Int32.MaxValue)
                                    {
                                        limit = false;
                                    }
                                    else
                                    {
                                        perm.Push(minDistIndx);
                                        vechNum[k, 0] += data[minDistIndx][1];
                                        vechNum[k, 1] += data[minDistIndx][2];
                                        citiesTab[minDistIndx][hub] = 0;
                                        minDist = Int32.MaxValue;
                                    }                                 
                                }
                                
                                limit = false;
                            }
                    }

                    minDist = Int32.MaxValue;
                    k++;    //jezeli pojazd k nie moze pomiescic wiecej towaru, wybieramy kolejny pojazd
                    if(k > vech.bigVechNum) //jezeli skoncza sie juz tiry to wybierane sa ciezarowki
                        {
                            vechWeig = vech.sMaxWeig;
                            vechLeng = vech.sMaxLeng;
                        }

                    limit = true;
                    perm.Push(hub);
                }
            }
            return perm;
        }

        static List<List<double>> readFileFunc(string fileName, int startLine)  //funkcja do szczytywania plikow excelowych
        {
            List<string> fileHelper = new List<string>();
            List<List<double>> data = new List<List<double>>();
            string line;
            String[] citiesS;
            int citiesNumb;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);


            fileHelper.Clear();
            data.Clear();

            if(fileHelper.Count != 0)
            {
                Console.WriteLine("Not empty!");
                fileHelper.Clear();
            }
            if(data.Count != 0)
            {
                Console.WriteLine("Not empty!");
                data.Clear();
            }

            while ((line = file.ReadLine()) != null)
            {
               fileHelper.Add(line);
            }

            citiesS = fileHelper[0].Split(';');
            citiesNumb = Convert.ToInt32(citiesS[0]);

            for (int i = startLine; i < citiesNumb+startLine; i++)
            {
                List<double> pom = new List<double>();
                String[] elements = fileHelper[i].Split(';');
                foreach (var element in elements)
                {
                    pom.Add(Convert.ToDouble(element));
                }
                data.Add(pom);
            }
            file.Close();

            return data;
        }


        static double wholeDist(Stack perm, List<List<double>> Odleglosci, int hub)
        {
            int prevCity = hub;
            double wDist = 0;
            int elemTemp = 0;
            foreach (int elem in perm)
            {
                elemTemp = elem;
                //Console.Write(Odleglosci[prevCity][elemTemp]);
                //Console.Write("({0} + {1}) ", prevCity, elemTemp);
                wDist += Odleglosci[prevCity][elemTemp];
                prevCity = elemTemp;
            }
            return wDist;
        }

        static void Main(string[] args)
        {
            List<string> Plik = new List<string>();
            List<List<double>> Odleglosci = new List<List<double>>();
            List<List<double>> data = new List<List<double>>();
            Stack perm = new Stack();
            string line;
            int rozmiar;
            int hub = 0;
            double wholeDistance = 0;

            double bestWholeDistance = Int32.MaxValue;
            double besthub = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\PL.csv");
            line = file.ReadLine();
            Plik.Add(line);
            String[] rozmiar_st = Plik[0].Split(';');
            rozmiar = Convert.ToInt32(rozmiar_st[0]);
            file.Close();

            data = readFileFunc(@"C:\Users\barte\Downloads\PLdata.csv", 1);
            Odleglosci = readFileFunc(@"C:\PL.csv", 2);
            perm = VRP(Odleglosci, data, hub, rozmiar);
            Odleglosci = readFileFunc(@"C:\PL.csv", 2);
            wholeDistance = wholeDist(perm, Odleglosci, hub);

            //Console.WriteLine(wholeDistance);

            for(int i = 0; i < rozmiar; i++)    //wybieramy miasto, w którym najlepiej jest ustalic huba
            {
                perm = VRP(Odleglosci, data, i, rozmiar);
                Odleglosci = readFileFunc(@"C:\PL.csv", 2);
                wholeDistance = wholeDist(perm, Odleglosci, i);
                if(wholeDistance < bestWholeDistance)
                {
                    bestWholeDistance = wholeDistance;
                    besthub = i;
                }

            }


            Console.WriteLine("Best hub is: {0} and best distance is: {1}", besthub, bestWholeDistance);

            //Console.Write("{");
            //foreach (int elem in perm)
            //{
             //   Console.Write(elem + 1);
             //  Console.Write("; ");
            //}
            //Console.Write("}");

            //Console.WriteLine(Odleglosci[0][1]);        
        }
    }
}
