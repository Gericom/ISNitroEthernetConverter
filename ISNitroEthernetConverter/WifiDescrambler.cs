using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNitroEthernetConverter
{
    public class WifiDescrambler
    {
        private int mStates;

        public WifiDescrambler(int initialState)
        {
            mStates = initialState;
        }

        public int Scramble(int bit)
        {
            int output_bit = bit ^ ((mStates >> 3) & 1) ^ ((mStates >> 6) & 1);
            mStates = ((mStates << 1) | bit) & 0x7F;
            return output_bit;
        }
    }
}
