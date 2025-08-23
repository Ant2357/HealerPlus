using BepInEx;
using HarmonyLib;
using System.Linq;
using System.Text.RegularExpressions;

[BepInPlugin("ant2357.healer_plus", "Healer Plus", "1.1.0")]
public class Plugin : BaseUnityPlugin
{
    public void OnStartCore()
    {
        var harmony = new Harmony("ant2357.healer_plus");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Trait), "OnBarter")]
public static class HealerPlus
{
    [HarmonyPostfix]
    public static void Postfix(ref Trait __instance)
    {
        // 癒し手のみ対象とする
        if (__instance.owner == null || __instance.owner.id != "healer") return;

        // 在庫補充時のみ処理を行う
        if (!__instance.owner.isRestocking) return;


        // 商人のチェストを取得または作成
        Thing chest = __instance.owner.things.Find("chest_merchant", -1, -1);
        if (chest == null)
        {
            chest = ThingGen.Create("chest_merchant", -1, -1);
            __instance.owner.AddThing(chest, true, -1, -1);
        }
        Card chestCard = (Card)chest;

        // 変異治癒のポーションが存在しない場合のみに追加処理を行う
        bool hasPotion = chestCard.things.Any(item => Regex.IsMatch(item.NameSimple, "mutation|変異治癒|变异"));
        if (!hasPotion)
        {
            // 変異治癒のポーションを追加
            Thing cureMutationPotion = ThingGen.CreatePotion(8480).Identify(false, (IDTSource)1);
            cureMutationPotion.SetNum(10);
            chestCard.AddThing(cureMutationPotion, true, -1, -1);
        }

        // 致命傷治癒の杖が存在しない場合のみに追加処理を行う
        bool hasCriticalHealRod = chestCard.things.Any(item => Regex.IsMatch(item.NameOne, "rod of cure critical wound|致命傷治癒の杖|致命伤治疗之杖"));
        if (!hasCriticalHealRod)
        {
            // 致命傷治癒の杖を追加
            Thing criticalHealRod = ThingGen.CreateRod(8402).Identify(false, (IDTSource)1);
            criticalHealRod.SetNum(1);
            criticalHealRod.ModCharge(3);
            chestCard.AddThing(criticalHealRod, true, -1, -1);
        }

        // 致命傷治癒の魔法書が存在しない場合のみに追加処理を行う
        bool hasCriticalHealBook = chestCard.things.Any(item => Regex.IsMatch(item.NameOne, "spellbook of cure critical wound|致命傷治癒の魔法書|致命伤治疗魔法书"));
        if (!hasCriticalHealBook)
        {
            // 致命傷治癒の魔法書を追加
            Thing criticalHealBook = ThingGen.CreateSpellbook(8402).Identify(false, (IDTSource)1);
            criticalHealBook.ModCharge(3);
            chestCard.AddThing(criticalHealBook, true, -1, -1);
        }
    }
}
