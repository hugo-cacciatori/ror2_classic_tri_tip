using System.Collections;
using RoR2;
using Ror2Mods.GateProbe;

// In-game scenarios run by the boot gate in a real run (host, no local user). Spawned bodies have no AI.
namespace Ror2Mods.ClassicTriTip.Scenarios
{
    public static class TriTipScenarios
    {
        private static CharacterBody Attacker(ScenarioContext ctx, int daggers, int shatterspleens)
        {
            var body = ctx.Spawn("CommandoMonsterMaster", TeamIndex.Player);
            if (daggers > 0) body.master.inventory.GiveItemPermanent(RoR2Content.Items.BleedOnHit, daggers);
            if (shatterspleens > 0) body.master.inventory.GiveItemPermanent(RoR2Content.Items.BleedOnHitAndExplode, shatterspleens);
            return body;
        }

        // A crit hit with proc coefficient 1, through vanilla's on-hit entry point.
        private static void CritHit(CharacterBody attacker, CharacterBody victim) =>
            GlobalEventManager.instance.OnHitEnemy(new DamageInfo
            {
                attacker = attacker.gameObject,
                inflictor = attacker.gameObject,
                damage = 1f,
                crit = true,
                procCoefficient = 1f,
                position = victim.corePosition,
                damageType = DamageType.Generic,
            }, victim.gameObject);

        private static int Bleeds(CharacterBody body) => body ? body.GetBuffCount(RoR2Content.Buffs.Bleeding) : -1;

        private static IEnumerator ExpectBleeds(ScenarioContext ctx, int daggers, int shatterspleens, int expected)
        {
            var attacker = Attacker(ctx, daggers, shatterspleens);
            var golem = ctx.Spawn("GolemMaster", TeamIndex.Monster);
            yield return ctx.WaitUntil(() => attacker.bleedChance >= daggers * 15f, 3f);
            CritHit(attacker, golem);
            yield return ctx.WaitUntil(() => Bleeds(golem) >= expected, 1f);
            yield return ctx.Wait(0.5f);
            ctx.Assert(Bleeds(golem) == expected,
                $"{daggers} dagger(s) and {shatterspleens} Shatterspleen(s): {Bleeds(golem)} Bleed stack(s) after one crit hit, expected {expected}");
        }

        [Scenario("each Tri-Tip Dagger gives 15% bleed chance")]
        public static IEnumerator FifteenPercentPerDagger(ScenarioContext ctx)
        {
            var attacker = Attacker(ctx, 1, 0);
            yield return ctx.WaitUntil(() => attacker.bleedChance > 0f, 3f);
            ctx.Assert(System.Math.Abs(attacker.bleedChance - 15f) < 0.01f, $"1 dagger gives {attacker.bleedChance}% bleed chance, expected 15%");
            attacker.master.inventory.GiveItemPermanent(RoR2Content.Items.BleedOnHit, 1);
            yield return ctx.WaitUntil(() => attacker.bleedChance > 15.01f, 3f);
            ctx.Assert(System.Math.Abs(attacker.bleedChance - 30f) < 0.01f, $"2 daggers give {attacker.bleedChance}% bleed chance, expected 30%");
        }

        [Scenario("the dagger rolls its own stack on a Shatterspleen crit")]
        public static IEnumerator DaggerAndShatterspleenStack(ScenarioContext ctx) => ExpectBleeds(ctx, daggers: 7, shatterspleens: 1, expected: 2);

        [Scenario("without Shatterspleen a dagger hit bleeds once")]
        public static IEnumerator DaggerAloneBleedsOnce(ScenarioContext ctx) => ExpectBleeds(ctx, daggers: 7, shatterspleens: 0, expected: 1);

        [Scenario("Shatterspleen alone stays vanilla")]
        public static IEnumerator ShatterspleenAloneBleedsOnce(ScenarioContext ctx) => ExpectBleeds(ctx, daggers: 0, shatterspleens: 1, expected: 1);

        [Scenario("the dagger's text shows 15%")]
        public static IEnumerator TextShowsFifteen(ScenarioContext ctx)
        {
            yield return null;
            var language = Language.currentLanguage;
            ctx.Assert(language != null, "no current language");
            foreach (var token in new[] { "ITEM_BLEEDONHIT_PICKUP", "ITEM_BLEEDONHIT_DESC" })
            {
                var text = language.GetLocalizedStringByToken(token);
                ctx.Assert(text.Contains("15%") && !text.Contains("10%"), $"{token} reads: {text}");
            }
        }
    }
}
