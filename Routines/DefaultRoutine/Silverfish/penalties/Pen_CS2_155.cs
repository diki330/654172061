using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Pen_CS2_155 : PenTemplate //archmage
    {

        //    zauberschaden +1/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            return 0;
        }

    }
}