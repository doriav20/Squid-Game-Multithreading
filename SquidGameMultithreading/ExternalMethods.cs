using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SquidGameMultithreading
{
    public class ExternalMethods
    {
        private static int[] GetIDs(int quantity)
        {
            HashSet<int> set = new HashSet<int>(quantity);
            Random rand = new Random();
            while (set.Count < quantity)
            {
                int num = rand.Next(1, 1000);
                set.Add(num);
            }
            return set.ToArray();
        }

        private static string[] GetNames(int quantity)
        {
            string[] names = File.ReadAllLines(@"../../names.txt");
            int N = names.Length;
            HashSet<string> set = new HashSet<string>(quantity);
            Random rand = new Random();
            while (set.Count < quantity)
            {
                int index = rand.Next(0, N);
                set.Add(names[index]);
            }
            return set.ToArray();
        }

        private static int[] GetFacesID(int quantity)
        {
            HashSet<int> set = new HashSet<int>(quantity);
            Random rand = new Random();
            while (set.Count < quantity)
            {
                int num = rand.Next(0, 300);
                set.Add(num);
            }
            return set.ToArray();
        }

        public static Person[] InitializePeople(int quantity)
        {
            Person[] people = new Person[quantity];
            int[] IDs = GetIDs(quantity);
            string[] names = GetNames(quantity);
            int[] facesIDs = GetFacesID(quantity);
            Random rand = new Random();
            for (int i = 0; i < quantity; i++)
            {
                int age = rand.Next(15, 30 + 1);
                people[i] = new Person(IDs[i].ToString().PadLeft(3, '0'), names[i], facesIDs[i], age);
            }
            return people;
        }

    }
}
