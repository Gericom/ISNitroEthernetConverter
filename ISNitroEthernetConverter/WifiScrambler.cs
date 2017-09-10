using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNitroEthernetConverter
{
    public class WifiScrambler
    {
        private int mStates;

        public WifiScrambler(int initialState)
        {
            mStates = initialState;
        }

        public int Scramble(int bit)
        {
            int save_state_4 = (mStates >> 3) & 1;
            int save_state_7 = (mStates >> 6) & 1;
            int output_bit = bit ^ (save_state_4 ^ save_state_7);
            mStates = ((mStates << 1) | output_bit) & 0x7F;
            return output_bit;
        }
    }
}
