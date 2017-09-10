using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNitroEthernetConverter
{
    class Program
    {
        private static readonly int[] sPhaseTable = { 0,2,1,3,1,0,3,2,2,3,0,1,3,1,2,0 };
        private const uint sBarkerCode = 0x5B8;

        static void Main(string[] args)
        {
            string data = File.ReadAllText(args[0]);
            WifiDescrambler ds = new WifiDescrambler(0x1B);
            List<byte> result = new List<byte>();
            byte b = 0;
            int bo = 0;
            const int offset = 2;
            int bitCounter = 0;
            int val = 1;
            for (int i = offset; i + 10 < data.Length; i += 11)
            {
                //barkercode to bits
                int result2 = 0;
                for (int j = 0; j < 11; j++)
                    if (((data[i + j] >> 1) & 1) == ((sBarkerCode >> (10 - j)) & 1))
                        result2++;
                    else
                        result2--;
                int result3 = 0;
                for (int j = 0; j < 11; j++)
                    if ((data[i + j] & 1) == ((sBarkerCode >> (10 - j)) & 1))
                        result3++;
                    else
                        result3--;
                int inbit0 = (result2 < 0 ? 0 : 1);
                int inbit1 = (result3 < 0 ? 0 : 1);
                if (bitCounter >= 73)
                {
                    //it's 2 mbps from here
                    if (bitCounter == 73)
                        val = 0;
                    int curPhase = (inbit0 << 1) | inbit1;

                    int actualVal = sPhaseTable[(val << 2) | curPhase];
                    val = curPhase;

                    int bit0 = ds.Scramble(actualVal >> 1);
                    int bit1 = ds.Scramble(actualVal & 1);

                    b |= (byte)(bit0 << bo++);
                    b |= (byte)(bit1 << bo++);
                }
                else
                {
                    //first part is 1 mbps
                    int bit = ds.Scramble(val ^ inbit0);
                    val = inbit0;
                    if (bitCounter == 0)
                    {
                        bitCounter++;
                        continue;
                    }
                    b |= (byte)(bit << bo++);
                }
                if (bo == 8)
                {
                    result.Add(b);
                    b = 0;
                    bo = 0;
                }
                bitCounter++;
            }
            File.Create(args[1]).Close();
            File.WriteAllBytes(args[1], result.ToArray());
        }
    }
}
