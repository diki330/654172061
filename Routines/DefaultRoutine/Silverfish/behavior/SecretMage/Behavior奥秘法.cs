using System.Collections.Generic;

namespace HREngine.Bots
{

    public class BehaviorSecretMage : Behavior
    {
        public override string BehaviorName() { return "奥秘法"; }


        PenalityManager penman = PenalityManager.Instance;

        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            int hpboarder = 10;
            int aggroboarder = 11;
            if (p.enemyHeroName == HeroEnum.pala) aggroboarder = 10;
            if (p.enemyHeroName == HeroEnum.shaman) aggroboarder = 10;
            if (p.enemyHeroName == HeroEnum.druid) aggroboarder = 10;
            if (p.enemyHeroName == HeroEnum.mage) aggroboarder = 15;
            if (p.enemyHeroName == HeroEnum.priest) aggroboarder = 20;
            if (p.enemyHeroName == HeroEnum.thief) aggroboarder = 15;
            if (p.enemyHeroName == HeroEnum.warrior) aggroboarder = 15;
            if (p.enemyHeroName == HeroEnum.warlock) aggroboarder = 22;
            if (p.enemyHeroName == HeroEnum.hunter) aggroboarder = 10;

            retval -= p.evaluatePenality;
            retval += p.owncards.Count * 3;

            if (p.ownHero.Hp + p.ownHero.armor > hpboarder)
            {
                retval += p.ownHero.Hp + p.ownHero.armor;
            }
            else
            {
                if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                else retval -= 2 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
            }

            if (p.enemyHero.Hp + p.enemyHero.armor > aggroboarder)
            {
                retval += -p.enemyHero.Hp - p.enemyHero.armor;
            }
            else
            {
                retval += 4 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
            }

            if (p.ownWeapon.name == CardDB.cardName.aluneth)
            {
                if (p.owncards.Count < 5) retval += 30;
            }

            //RR card draw value depending on the turn and distance to lethal
            //RR if lethal is close, carddraw value is increased
            if (p.lethalMissing() <= 5) //RR
            {
                retval += p.owncarddraw * 100;
            }
            if (p.ownMaxMana < 4)
            {
                retval += p.owncarddraw * 2;
            }
            else
            {
                retval += p.owncarddraw * 5;
            }

            //int owntaunt = 0;
            int readycount = 0;
            int ownMinionsCount = 0;
            foreach (Minion m in p.ownMinions)
            {
                retval += 5;
                retval += m.Hp * 2;
                retval += m.Angr * 2;
                if (!m.playedThisTurn && m.windfury) retval += m.Angr;
                if (m.divineshild) retval += 1;
                if (m.stealth) retval += 1;
                if ((m.handcard.card.name == CardDB.cardName.stargazerluna || m.handcard.card.name == CardDB.cardName.arcaneflakmage) && !m.silenced)
                {
                    retval += 10;
                }
                else
                {
                    if (m.Angr <= 2 && m.Hp <= 2 && !m.divineshild) retval -= 5;
                }
                if (m.poisonous) retval += 1;
                if (m.lifesteal) retval += m.Angr / 2;
                if (m.divineshild && m.taunt) retval += 4;
                if (p.ownMinions.Count > 2 && (m.handcard.card.name == CardDB.cardName.direwolfalpha || m.handcard.card.name == CardDB.cardName.flametonguetotem || m.handcard.card.name == CardDB.cardName.stormwindchampion || m.handcard.card.name == CardDB.cardName.raidleader)) retval += 10;
                if (m.handcard.card.name == CardDB.cardName.nerubianegg)
                {
                    if (m.Angr >= 1) retval += 2;
                    if ((!m.taunt && m.Angr == 0) && (m.divineshild || m.maxHp > 2)) retval -= 10;
                }
                if (m.Ready) readycount++;
                if (m.Hp <= 4 && (m.Angr > 2 || m.Hp > 3)) ownMinionsCount++;
            }
            retval += p.anzOwnExtraAngrHp - p.anzEnemyExtraAngrHp;

            bool useAbili = false;
            int usecoin = 0;
            //soulfire etc
            int deletecardsAtLast = 0;
            int wasCombo = 0;
            bool firstSpellToEnHero = false;
            int count = p.playactions.Count;
            int ownActCount = 0;
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    case actionEnum.attackWithHero:
                        if (p.enemyHero.Hp <= p.attackFaceHP) retval++;
                        continue;
                    case actionEnum.useHeroPower:
                        useAbili = true;
                        continue;
                    case actionEnum.playcard:
                        if (a.card.card.Secret) retval += 5;
                        break;
                    default:
                        continue;
                }
                switch (a.card.card.name)
                {
                    case CardDB.cardName.thecoin:
                        usecoin++;
                        if (i == count - 1) retval -= 10;
                        goto default;
                    default:
                        if (deletecardsAtLast == 1) retval -= 20;
                        break;
                }
                if (a.card.card.Combo && i > 0) wasCombo++;
                if (a.target == null) continue;
            }
            if (wasCombo > 0 && firstSpellToEnHero)
            {
                if (wasCombo + 1 == ownActCount) retval += 10;
            }
            if (usecoin > 0)
            {
                if (useAbili && p.ownMaxMana <= 2) retval -= 40;
                retval -= 10 * p.manaTurnEnd;
                if (p.manaTurnEnd + usecoin > 10) retval -= 10 * usecoin;
            }
            if (p.manaTurnEnd >= 2 && !useAbili && p.ownAbilityReady)
            {
                retval -= 10;
            }

            //bool hasTank = false;
            foreach (Minion m in p.enemyMinions)
            {
                retval -= this.getEnemyMinionValue(m, p);
                //hasTank = hasTank || m.taunt;
            }
            retval -= p.enemyMinions.Count * 2;
            /*foreach (SecretItem si in p.enemySecretList)
            {
                if (readycount >= 1 && !hasTank && si.canbeTriggeredWithAttackingHero)
                {
                    retval -= 100;
                }
                if (readycount >= 1 && p.enemyMinions.Count >= 1 && si.canbeTriggeredWithAttackingMinion)
                {
                    retval -= 100;
                }
                if (si.canbeTriggeredWithPlayingMinion && mobsInHand >= 1)
                {
                    retval -= 25;
                }
            }*/

            retval -= p.enemySecretCount;
            retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2

            if (p.enemyMinions.Count == 0) retval += 5;

            if (p.enemyHero.Hp <= 0)
            {
                retval += 10000;
                if (retval < 10000) retval = 10000;
            }

            if (p.enemyHero.Hp >= 1 && p.guessingHeroHP <= 0)
            {
                if (p.turnCounter < 2) retval += p.owncarddraw * 100;
                retval -= 1000;
            }
            if (p.ownHero.Hp <= 0) retval -= 10000;

            p.value = retval;
            return retval;
        }



        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            int retval = 5;
            retval += m.Hp * 2;
            if (!m.frozen && !(m.cantAttack && m.name != CardDB.cardName.argentwatchman))
            {
                retval += m.Angr * 2;
                if (m.windfury) retval += m.Angr * 2;
                //if (m.Angr >= 4) retval += 10;
                //if (m.Angr >= 7) retval += 50;
            }

            if (!m.handcard.card.isSpecialMinion)
            {
                if (m.Angr == 0) retval -= 7;
                else if (m.Angr <= 2 && m.Hp <= 2 && !m.divineshild) retval -= 5;
            }
            //else retval += m.handcard.card.rarity;

            if (m.taunt) retval += 5;
            if (m.divineshild) retval += m.Angr;
            if (m.lifesteal) retval += m.Angr;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 1;

            if (m.poisonous)
            {
                retval += 4;
                if (p.ownMinions.Count < p.enemyMinions.Count) retval += 10;
            }

            if (m.handcard.card.targetPriority >= 1 && !m.silenced)
            {
                retval += m.handcard.card.targetPriority;
            }
            if (m.name == CardDB.cardName.nerubianegg && m.Angr <= 3 && !m.taunt) retval = 0;
            if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) retval += 2;
            return retval;
        }


        public override int getSirFinleyPriority(List<Handmanager.Handcard> discoverCards)
        {
            int sirFinleyChoice = -1;
            int tmp = int.MinValue;
            for (int i = 0; i < discoverCards.Count; i++)
            {
                CardDB.cardName name = discoverCards[i].card.name;
                if (SirFinleyPriorityList.ContainsKey(name) && SirFinleyPriorityList[name] > tmp)
                {
                    tmp = SirFinleyPriorityList[name];
                    sirFinleyChoice = i;
                }
            }
            return sirFinleyChoice;
        }

        private Dictionary<CardDB.cardName, int> SirFinleyPriorityList = new Dictionary<CardDB.cardName, int>
        {
            //{HeroPowerName, Priority}, where 0-9 = manual priority
            { CardDB.cardName.lesserheal, 0 },
            { CardDB.cardName.shapeshift, 6 },
            { CardDB.cardName.fireblast, 7 },
            { CardDB.cardName.totemiccall, 1 },
            { CardDB.cardName.lifetap, 9 },
            { CardDB.cardName.daggermastery, 5 },
            { CardDB.cardName.reinforce, 4 },
            { CardDB.cardName.armorup, 2 },
            { CardDB.cardName.steadyshot, 8 }
        };

    }

}