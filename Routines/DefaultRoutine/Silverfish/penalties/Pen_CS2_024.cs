using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Pen_CS2_024 : PenTemplate //frostbolt
    {

        //    fügt einem charakter $3 schaden zu und friert/ ihn ein.
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            return 0;
        }

    }
}