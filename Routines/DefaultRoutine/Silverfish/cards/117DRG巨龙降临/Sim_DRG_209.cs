using System;
using System.Collections.Generic;
using System.Text;
//Ä£·ÂÈ¾²¡µÄØ£ðÕÐ´µÄ£¬¿ÉÄÜÓÐ´í
namespace HREngine.Bots
{
    class Sim_DRG_209 : SimTemplate//* Å¤Çú¾ÞÁúÔóÀ­¿â Zzeraku the Warped
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_209t);
        //Ã¿µ±ÄãµÄÓ¢ÐÛÊÜµ½ÉËº¦£¬ÕÙ»½Ò»Ìõ6/6µÄÐé¿ÕÓ×Áú¡£

        public override void onMinionGotDmgTrigger(Playfield p, Minion triggerEffectMinion, int anzOwnMinionsGotDmg, int anzEnemyMinionsGotDmg, int anzOwnHeroGotDmg, int anzEnemyHeroGotDmg)
        {
            if (p.ownHero.anzGotDmg > 0)
            {
                p.callKid(kid, triggerEffectMinion.zonepos, triggerEffectMinion.own);
            }
        }
    }
}
