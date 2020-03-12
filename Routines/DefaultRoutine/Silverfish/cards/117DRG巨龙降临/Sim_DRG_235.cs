using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots //by Summer Mate
{
    class Sim_DRG_235 : SimTemplate 
    {
       public override void onDeathrattle(Playfield p, Minion m) 	
	   {
		 if(m.own)  
		 {
			 foreach (Handmanager.Handcard hc in p.owncards)
			 {
				 if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
				 {
					 hc.addHp += 3;
					 hc.addattack += 3;
					 return;
				 }
			 }
		 }
	   }
		
    }
}
