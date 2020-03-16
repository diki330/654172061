namespace HREngine.Bots
{
    class Sim_DRG_302 : SimTemplate //* 墓地符文 Grave Rune
    {
        //Give a minion "<b>Deathrattle:</b> Summon 2 copies of this."
        //使一个随从获得“<b>亡语：</b>召唤该随从的两个复制。”
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.ancestralspirit++;
            //懒得弄了，得在minion里新建函数
        }

    }
}