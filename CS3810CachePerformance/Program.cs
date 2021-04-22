using System;
using System.Collections.Generic;

namespace CS3810CachePerformance
{
    class Program
    {
        private static int[] decMemLocations = new int[] { 4, 8, 12, 16, 20, 36, 40, 44, 20, 36, 40, 44, 64, 68,
                4, 8,12, 92, 96, 100, 104, 108, 112, 100, 112, 116, 120, 128, 140, 144 };


        private static string[] tags;
        private static int[] valids;
        private static List<int> decLru;

        private static string[,] setAssocTags;
        private static int[,] setAssocValids;
        private static List<int>[] setAssocDecLru;

        private const int directMapRows = 4;
        private const int fullyAssocRows = 6;
        private const int setAssocRows = 2;
        private const int setAssocWays = 3;

        private static int hit = 0;
        private static int miss = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("+---------------------+");
            Console.WriteLine("| Direct Mapped Cache |");
            Console.WriteLine("+---------------------+\n");

            // set up variables
            tags = new string[directMapRows];
            valids = new int[] { 0, 1, 2, 3 };
            decLru = new List<int>() { 0, 1, 2 ,3 };

            Console.WriteLine("First Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 10);
                string row = binMemLocation.Substring(10, 2);
                string offset = binMemLocation.Substring(12, 4);

                DirectMappedCacheAnalysis(decMemLocations[i], tag, row);
            }
            PrintDirectMappedTable(); // non dynamic
            
            // reset
            hit = 0;
            miss = 0;

            Console.WriteLine("Second Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 10);
                string row = binMemLocation.Substring(10, 2);
                string offset = binMemLocation.Substring(12, 4);

                DirectMappedCacheAnalysis(decMemLocations[i], tag, row);
            }
            PrintDirectMappedTable(); // non dynamic

            // print analysis
            Console.WriteLine("Direct mapped cache performance analysis:");
            Console.WriteLine("Hit time: " + hit + " + " + "Miss time: " + (miss * (hit + 10 + 3 * 4)));
            Console.WriteLine("Total time: " + (hit + (miss * (hit + 10 + 3 * 4))) + "\n");

            

            Console.WriteLine("+-------------------------+");
            Console.WriteLine("| Fully Associative Cache |");
            Console.WriteLine("+-------------------------+\n");

            // set up variables
            tags = new string[fullyAssocRows];
            valids = new int[] { 0, 1, 2, 3, 4, 5 };
            decLru = new List<int>() { 0, 1, 2, 3, 4, 5 };
            hit = 0;
            miss = 0;

            Console.WriteLine("First Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 12);
                string offset = binMemLocation.Substring(12, 4);

                FullyAssociativeCacheAnalysis(decMemLocations[i], tag);
            }
            PrintFullyAssociativeTable(); // non dynamic

            // reset
            hit = 0;
            miss = 0;

            Console.WriteLine("Second Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 12);
                string offset = binMemLocation.Substring(12, 4);

                FullyAssociativeCacheAnalysis(decMemLocations[i], tag);
            }
            PrintFullyAssociativeTable(); // non dynamic

            // print analysis
            Console.WriteLine("Fully associative cache performance analysis:");
            Console.WriteLine("Hit time: " + hit + " + " + "Miss time: " + (miss * (hit + 10 + 3 * 4)));
            Console.WriteLine("Total time: " + (hit + (miss * (hit + 10 + 3 * 4))) + "\n");

            

            Console.WriteLine("+-----------------------+");
            Console.WriteLine("| Set Associative Cache |");
            Console.WriteLine("+-----------------------+\n");

            // set up variables
            setAssocTags = new string[setAssocRows, setAssocWays];
            setAssocValids = new int[setAssocRows, setAssocWays] 
            { 
                { 0, 0, 0 }, 
                { 0, 0, 0 }
            };
            setAssocDecLru = new List<int>[setAssocRows] 
            {
                new List<int>{ 0, 1, 2 }, 
                new List<int>{ 0, 1, 2 } 
            };
            hit = 0;
            miss = 0;

            Console.WriteLine("First Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 11);
                string row = binMemLocation.Substring(11, 1);
                string offset = binMemLocation.Substring(12, 4);

                SetAssociativeCacheAnalysis(decMemLocations[i], tag, row);
            }
            PrintSetAssociativeTable(); // non dynamic

            // reset
            hit = 0;
            miss = 0;

            Console.WriteLine("Second Run:");
            for (int i = 0; i < decMemLocations.Length; i++)
            {
                string binMemLocation = Convert.ToString(decMemLocations[i], 2).PadLeft(16, '0');
                string tag = binMemLocation.Substring(0, 11);
                string row = binMemLocation.Substring(11, 1);
                string offset = binMemLocation.Substring(12, 4);

                SetAssociativeCacheAnalysis(decMemLocations[i], tag, row);
            }
            PrintSetAssociativeTable(); // non dynamic

            // print analysis
            Console.WriteLine("Set associative cache performance analysis:");
            Console.WriteLine("Hit time: " + hit + " + " + "Miss time: " + (miss * (hit + 10 + 3 * 4)));
            Console.WriteLine("Total time: " + (hit + (miss * (hit + 10 + 3 * 4))) + "\n");

            Console.ReadLine();
        }
        
        private static void DirectMappedCacheAnalysis(int memLocation, string tag, string row)
        {
            // each row
            for (int i = 0; i < directMapRows; i++)
            {
                if (row.Equals(Convert.ToString(i, 2).PadLeft(2, '0')))
                {
                    if (tag.Equals(tags[i]) && valids[i] == 1)
                    {
                        hit++;
                        Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Hit from row " + i);
                    }
                    else
                    {
                        tags[i] = tag;
                        valids[i] = 1;
                        miss++;
                        Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Miss - cached to row " + i);
                    }
                }
            }
        }

        private static void FullyAssociativeCacheAnalysis(int memLocation, string tag)
        {
            bool missed = true;
            // each row
            for(int i = 0; i < fullyAssocRows; i++)
            {
                if (tag.Equals(tags[i]) && valids[i] == 1)
                {
                    hit++;
                    decLru.Remove(i);
                    decLru.Add(i);

                    missed = false;
                    Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Hit from row " + i);
                }
                
            }
            if (missed)
            {
                miss++;

                decLru.Add(decLru[0]);
                decLru.RemoveAt(0);

                tags[decLru[fullyAssocRows-1]] = tag;
                valids[decLru[fullyAssocRows - 1]] = 1;
                Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Miss - cached to row " + decLru[fullyAssocRows - 1]);
            }
        }

        private static void SetAssociativeCacheAnalysis(int memLocation, string tag, string row)
        {
            // each row
            for (int i = 0; i < setAssocRows; i++)
            {
                if (row.Equals(i.ToString()))
                {
                    bool missed = true;
                    // each set
                    for (int j = 0; j < setAssocWays; j++)
                    {
                        if (tag.Equals(setAssocTags[i, j]) && setAssocValids[i, j] == 1)
                        {
                            hit++;
                            setAssocDecLru[i].Remove(j);
                            setAssocDecLru[i].Add(j);

                            missed = false;
                            Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Hit from row " + i + " set " + j);
                        }

                    }
                    if (missed)
                    {
                        miss++;

                        setAssocDecLru[i].Add(setAssocDecLru[i][0]);
                        setAssocDecLru[i].RemoveAt(0);

                        setAssocTags[i, setAssocDecLru[i][setAssocWays - 1]] = tag;
                        setAssocValids[i, setAssocDecLru[i][setAssocWays - 1]] = 1;
                        Console.WriteLine("Accessing " + memLocation + " (tag " + tag + "): " + "Miss - cached to row " + i + " set " + setAssocDecLru[i][setAssocWays - 1]);


                    }
                }
            }
        }

        private static void PrintDirectMappedTable() // non dynamic
        {
            Console.WriteLine("\nHits: " + hit + " Misses: " + miss);
            Console.WriteLine("\tTAG" + "\t\tVALID");
            Console.WriteLine("Row 0:\t" + tags[0] + "\t" + valids[0]);
            Console.WriteLine("Row 1:\t" + tags[1] + "\t" + valids[1]);
            Console.WriteLine("Row 2:\t" + tags[2] + "\t" + valids[2]);
            Console.WriteLine("Row 3:\t" + tags[3] + "\t" + valids[3] + "\n");
        }

        private static void PrintFullyAssociativeTable() // non dynamic
        {
            Console.WriteLine("\nHits: " + hit + " Misses: " + miss);
            Console.WriteLine("\tVALID" + "\tTAG" + "\t\tLRU (lowest values are LRU for each row)");
            Console.WriteLine("Row 0:\t" + valids[0] + "\t" + tags[0] + "\t" + decLru[0]);
            Console.WriteLine("Row 1:\t" + valids[1] + "\t" + tags[1] + "\t" + decLru[1]);
            Console.WriteLine("Row 2:\t" + valids[2] + "\t" + tags[2] + "\t" + decLru[2]);
            Console.WriteLine("Row 3:\t" + valids[3] + "\t" + tags[3] + "\t" + decLru[3]);
            Console.WriteLine("Row 4:\t" + valids[4] + "\t" + tags[4] + "\t" + decLru[4]);
            Console.WriteLine("Row 5:\t" + valids[5] + "\t" + tags[5] + "\t" + decLru[5] + "\n");
        }

        private static void PrintSetAssociativeTable() // non dynamic
        {
            Console.WriteLine("\nHits: " + hit + " Misses: " + miss + "\t\t\t(lowest values are LRU for each set)");
            Console.WriteLine("\tTAG" + "\t\tVALID" + "\tLRU" + "  |  TAG" + "\t\tVALID" + "\tLRU" + "  |  TAG" + "\t\tVALID" + "\tLRU");
            Console.WriteLine("Row 0:\t" + setAssocTags[0, 0] + "\t" + setAssocValids[0, 0] + "\t" + setAssocDecLru[0][0] + "    |  " +
                                           setAssocTags[0, 1] + "\t" + setAssocValids[0, 1] + "\t" + setAssocDecLru[0][1] + "    |  " +
                                           setAssocTags[0, 2] + "\t" + setAssocValids[0, 2] + "\t" + setAssocDecLru[0][2]);

            Console.WriteLine("Row 1:\t" + setAssocTags[1, 0] + "\t" + setAssocValids[1, 0] + "\t" + setAssocDecLru[1][0] + "    |  " +
                                           setAssocTags[1, 1] + "\t" + setAssocValids[1, 1] + "\t" + setAssocDecLru[1][1] + "    |  " +
                                           setAssocTags[1, 2] + "\t" + setAssocValids[1, 2] + "\t" + setAssocDecLru[1][2] + "\n");
        }

    }
}
