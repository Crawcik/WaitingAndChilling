using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.Lang;
using Smod2.Piping;

namespace WaitingAndChilling
{
    [PluginDetails(
        author = "F4Fridey",
        name = "Waiting And Chilling",
        description = "A plugin that spawn players in during the Waiting For Players stage.",
        id = "f4fridey.waitingandchilling.plugin",
        configPrefix = "wac",
        langFile = "waitingandchilling",
        version = "1.3",
        SmodMajor = 3,
        SmodMinor = 8,
        SmodRevision = 4
        )]
    public class WaitingAndChilling : Plugin
    {
        [ConfigOption]
        public readonly bool enabled = true;

        [ConfigOption("roles")]
        public readonly string roleInt = "14";

        [ConfigOption("choose_one_role_per_round")]
        public readonly bool oneRolePerRound = true;

        [ConfigOption("cords")]
        public readonly string cords = "53,1020,-43";
        /*
        [ConfigOption("items")]
        public readonly int[] items = { 23 , 30 };*/

        public override void OnDisable()
        {
            Info("WAC Disabled.");
        }

        public override void OnEnable()
        {
            if (enabled)
            {
                Info("WAC Enabled.");
            }
            else
            {
                Info("WAC has not registered, wac_enabled is false!");
            }
        }

        public override void Register()
        {
            if (enabled)
            {
                AddEventHandlers(new EventHandler(this));
            }
        }
    }
}
