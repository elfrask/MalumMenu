using HarmonyLib;
using System;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start
{
	// Postfix patch of HudManager.Start to give minimap access to impostors too
	public static void Postfix(HudManager __instance)
	{
		__instance.MapButton.OnClick.RemoveAllListeners(); // Remove previous OnClick action

		// Always open normal map when map button is clicked
		// To access sabotage map, sabotage button can be used
		__instance.MapButton.OnClick.AddListener((Action) (() =>
        {
			__instance.ToggleMapVisible(new MapOptions
			{
				Mode = MapOptions.Modes.Normal
			});

		}));
	}
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class HudManager_Update
{
    private static Vector3 originalMatchInfoPosition;
    private static bool isMatchInfoPositionStored;

    public static void Postfix(HudManager __instance)
    {
		__instance.ShadowQuad.gameObject.SetActive(!MalumESP.IsFullbrightActive()); // Fullbright

		if (Utils.IsChatUiActive()) // AlwaysChat
		{
			__instance.Chat.gameObject.SetActive(true);
		}
		else
		{
			Utils.CloseChat();
			__instance.Chat.gameObject.SetActive(false);
		}

		if (CheatToggles.enableChat)
		{
			// Keep the MatchInfo button clear of the chat button so they don't overlap
			var matchInfoButton = __instance.MatchInfoButton;
			var chatButton = __instance.Chat.chatButton;

			if (matchInfoButton && chatButton)
			{
				var matchInfoTransform = matchInfoButton.transform;

				if (!isMatchInfoPositionStored)
				{
					originalMatchInfoPosition = matchInfoTransform.position;
					isMatchInfoPositionStored = true;
				}

				var chatPosition = chatButton.transform.position;
				matchInfoTransform.position = new Vector3(chatPosition.x - 5f, chatPosition.y, chatPosition.z);
			}
		}
		else if (isMatchInfoPositionStored)
		{
			var matchInfoButton = __instance.MatchInfoButton;
			if (matchInfoButton)
			{
				matchInfoButton.transform.position = originalMatchInfoPosition;
			}

			isMatchInfoPositionStored = false;
		}

		MalumCheats.UseVentCheat(__instance);
		MalumESP.ZoomOut(__instance);
		MalumESP.FreecamCheat();

		// Close PlayerPickMenu if there is no PPM cheat enabled
		if (PlayerPickMenu.playerpickMenu != null && CheatToggles.ShouldPPMClose())
		{
            PlayerPickMenu.playerpickMenu.Close();
        }
    }
}
