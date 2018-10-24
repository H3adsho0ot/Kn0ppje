using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundControlServer
{
    public static class Serial
    {
        public static void Write(object module)
        {
            Console.WriteLine(JSON.serialize(module));
        }
    }
}
