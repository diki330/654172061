using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_224: SimTemplate //* Grim Necromancer
    {
        // Battlecry: Summon two 1/1 Skeletons.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_224t); //1/1 Skeleton

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos - 1, own.own); //1st left
            p.callKid(kid, own.zonepos, own.own);
        }
    }
}