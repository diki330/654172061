using System;
using System.Collections.Generic;
using System.Text;
//ģ��Ⱦ����أ��д�ģ������д�
namespace HREngine.Bots
{
    class Sim_DRG_209 : SimTemplate//* Ť������������ Zzeraku the Warped
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_209t);
        //ÿ�����Ӣ���ܵ��˺����ٻ�һ��6/6�����������

        public override void onMinionGotDmgTrigger(Playfield p, Minion triggerEffectMinion, int anzOwnMinionsGotDmg, int anzEnemyMinionsGotDmg, int anzOwnHeroGotDmg, int anzEnemyHeroGotDmg)
        {
            if (p.ownHero.anzGotDmg > 0)
            {
                p.callKid(kid, triggerEffectMinion.zonepos, triggerEffectMinion.own);
            }
        }
    }
}
