using BepInEx;
using HarmonyLib;
using System.Linq;
using System.Text.RegularExpressions;

[BepInPlugin("ant2357.healer_plus", "Healer Plus", "1.0.0")]
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

        // 既に変異治癒のポーションが存在するか確認
        bool hasPotion = chestCard.things.Any(item => Regex.IsMatch(item.NameSimple, "mutation|変異治癒|变异"));

        // 既に変異治癒のポーションが存在する場合は処理を終了
        if (hasPotion) return;

        // 変異治癒のポーションを追加
        Thing cureMutationPotion = ThingGen.CreatePotion(8480).Identify(false, (IDTSource)1);
        cureMutationPotion.SetNum(10);
        chestCard.AddThing(cureMutationPotion, true, -1, -1);
    }
}
