using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CGWRailwayCustomizations
{
    public class ModInfo : IUserMod
    {
        public string Name
        {
            get { return "CGW Railway Customizations"; }
        }

        public string Description
        {
            get { return "Applies Customizations to RWY/BVU tracks"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            // Load the configuration
            ModConfiguration config = Configuration<ModConfiguration>.Load();
            ModConfiguration loadinginstuc = new ModConfiguration();

            helper.AddCheckbox("Use shinkansen sleepers on concrete RWY based tracks", config.ShnGSleeperReplace, sel =>
            {
                config.ShnGSleeperReplace = sel;
                Configuration<ModConfiguration>.Save();
            });

            helper.AddCheckbox("Remove inner gauntlet tracks from bridge elevation BVU tracks", config.BVUGauntletTracksRemove, sel =>
            {
                config.BVUGauntletTracksRemove = sel;
                Configuration<ModConfiguration>.Save();
            });

            helper.AddGroup("\nChanges require a game restart to apply");

        }

    }


    [ConfigurationPath("CGWRailwayCustomizations.xml")]
    public class ModConfiguration
    {
        public bool ShnGSleeperReplace { get; set; } = true;
        public bool BVUGauntletTracksRemove { get; set; } = true;
    }

    public class ModThreading : ThreadingExtensionBase
    {

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {

            if (Input.GetKey(KeyCode.F2))
            {

            }
        }
    }
}
