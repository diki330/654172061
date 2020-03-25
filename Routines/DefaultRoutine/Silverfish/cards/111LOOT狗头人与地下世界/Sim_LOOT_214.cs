using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_214 : SimTemplate //闪避
    {
        //todo secret
        //    geheimnis:/ wenn euer held tödlichen schaden erleidet, wird dieser verhindert und der held wird immun/ in diesem zug.
        public override void onSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            if (p.ownHero.anzGotDmg > 0)
            {
                target.immune = true;
            }
        }

    }

}