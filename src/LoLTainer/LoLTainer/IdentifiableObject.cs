using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer
{
    public abstract class IdentifiableObject
    {
        private const int ID_LENGTH = 6;

        private string _id;
        protected string Id { get => _id; }
        public IdentifiableObject()
        {
            _id = RandomString(ID_LENGTH);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
