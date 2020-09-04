using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace WaitingAndChilling
{
    class EventHandler : IEventHandlerFixedUpdate, IEventHandlerRoundRestart, IEventHandlerRoundStart
    {
        private readonly WaitingAndChilling plugin;

        bool waitingForPlayers = true;

        float time = 1;
        float timer = 0;

        public EventHandler(WaitingAndChilling plugin)
        {
            this.plugin = plugin;
        }

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (waitingForPlayers)
            {
                try { GameObject.Find("StartRound").transform.localScale = Vector3.zero; }
                catch { plugin.Debug("StartRound not found... If you see this because you just started the server, ignore this."); }

                if (timer < time)
                {
                    timer += Time.deltaTime;
                }
                else if (plugin.Server.GetPlayers().Count > 0)
                {
                    timer = 0;
                    string str = "<color=#fffbad>WAITING FOR PLAYERS</color>\n";
                    short waitingTimer = GameCore.RoundStart.singleton.Timer;
                    if (waitingTimer == -2)
                    {
                        str += "<color=#ff7d92>paused</color>";
                    }
                    else if (waitingTimer == -1)
                    {
                        return;
                    }
                    else
                    {
                        str += "<color=#7dff8a>" + waitingTimer.ToString("0") + "</color> seconds left";
                    }

                    foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
                    {
                        if (referenceHub.isDedicatedServer)
                            continue;
                        if (referenceHub.characterClassManager.CurClass != RoleType.Tutorial)
                        {
                            referenceHub.characterClassManager.SetPlayersClass(RoleType.Tutorial, referenceHub.gameObject);
                            referenceHub.playerMovementSync.OverridePosition(new Vector3(53, 1020, -43), 0);
                        }  
                        referenceHub.hints.Show(new Hints.TextHint(str, new Hints.HintParameter[] { new Hints.StringHintParameter(string.Empty) }, Hints.HintEffectPresets.FadeInAndOut(0f), 1f));
                    }
                }
                else
                {
                    timer = 0;
                }
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            waitingForPlayers = true;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            waitingForPlayers = false;
        }
    }
}
