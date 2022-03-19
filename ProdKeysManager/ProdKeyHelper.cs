using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdKeysManager
{
    public static class ProdKeyHelper
    {
        public static IList<ProdKeyItem> LoadProdKeys(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            return LoadProdKeys(lines);
        }

        public static IList<ProdKeyItem> LoadProdKeys(string[] lines)
        {
            List<ProdKeyItem> prodKeys = new List<ProdKeyItem>();
            foreach (string line in lines)
            {
                string[] keyValue = line.Split('=');
                if (keyValue.Length == 2)
                {
                    prodKeys.Add(new ProdKeyItem(keyValue[0].Trim(), keyValue[1].Trim()));
                }
            }
            return prodKeys;
        }
    }
}
