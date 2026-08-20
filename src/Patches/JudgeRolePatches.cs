using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(JudgeRole), nameof(JudgeRole.IsBlockedByTasks))]
public static class JudgeRole_IsBlockedByTasks
{
    // Prefix patch of JudgeRole.IsBlockedByTasks to bypass the task requirement for the overrule ability
    public static bool Prefix(ref bool __result)
    {
        if (!CheatToggles.forceJudgeAbilities) return true;

        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(JudgeRole), nameof(JudgeRole.HasAnOverruleUse), MethodType.Getter)]
public static class JudgeRole_HasAnOverruleUse
{
    // Prefix patch of JudgeRole.HasAnOverruleUse to allow unlimited overrule uses per meeting
    public static bool Prefix(ref bool __result)
    {
        if (!CheatToggles.forceJudgeAbilities) return true;

        __result = true;
        return false;
    }
}