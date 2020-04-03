using ColossalFramework.UI;
using Harmony;
using ICities;
using System;
using System.Reflection;
using UnityEngine;

namespace CGWRailwayCustomizations
{

    public class ModLoading : LoadingExtensionBase
    {

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            var harmony = HarmonyInstance.Create("cgameworld.railwaycustom");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
        }

    }

    //runs customizations at after LoadingWrapper instead at OnLevelLoaded  - thanks kian.zarrin for the tip!
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public class Patch
    {

        static void Postfix()
        {
            Replacer.Run();
        }
    }

    public class Replacer
    {
        public static void Run()
        {
            ModConfiguration config = Configuration<ModConfiguration>.Load();
            ReplaceTracks tracks = new ReplaceTracks();
            tracks.GetItems(config.BVUGauntletTracksRemove);
            if (config.ShnGSleeperReplace)
            {
                Debug.Log("shn sleepers replaced");
                tracks.ReplaceToShnGSleepers();
            }
        }
    }


    public class ReplaceTracks
    {
        private Mesh replaceToMesh1T;
        private Material replaceToMaterial1T;

        private Mesh replaceToMeshNode1;
        private Material replaceToMaterialNode1;
        private Mesh replaceToMeshNode2;
        private Material replaceToMaterialNode2;

        private Mesh replaceToMesh2T;
        private Material replaceToMaterial2T;


        string[] tobeReplacedNames1T = { "r-sleepers-s", "r-sleepersw_0.023" };
        string[] tobeReplacedNames2T = { "r-sleepers", "r-sleepersw_0.008" };

        public void GetItems(bool removeGauntletTracks)
        {
            foreach (var prefab in Resources.FindObjectsOfTypeAll<NetInfo>())
            {
                //for sleeper replacement 4 meshes in total are needed
                if (prefab.name == "1779369015.R69Railway ShnG 1x2_Data")
                {

                    replaceToMesh1T = prefab.m_segments[1].m_segmentMesh;
                    replaceToMaterial1T = prefab.m_segments[1].m_segmentMaterial;

                    //for transition from 1 Track to 2 Track 
                    replaceToMeshNode1 = prefab.m_nodes[5].m_nodeMesh;
                    replaceToMaterialNode1 = prefab.m_nodes[5].m_nodeMaterial;

                    replaceToMeshNode2 = prefab.m_nodes[7].m_nodeMesh;
                    replaceToMaterialNode2 = prefab.m_nodes[7].m_nodeMaterial;
                }
                if (prefab.name == "1779369015.R69Railway ShnG 2x2_Data")
                {
                    replaceToMesh2T = prefab.m_segments[1].m_segmentMesh;
                    replaceToMaterial2T = prefab.m_segments[1].m_segmentMaterial;
                }

                if (removeGauntletTracks)
                {
                    //get and remove inner gauntlet tracks on bridge BVU tracks
                    if (prefab.name == "1847646595.R69Railway W GrCo 1x2 B0" || prefab.name == "1847646595.R69Railway W GrCo 1x2 B1")
                    {
                        prefab.m_segments[7].m_segmentMesh = null;
                    }

                    if (prefab.name == "1847646595.R69Railway W GrCo 2x2 B0" || prefab.name == "1847646595.R69Railway W GrCo 2x2 B1")
                    {
                        prefab.m_segments[7].m_segmentMesh = null;
                    }

                }
                //nodeless tracks?
            }
        }

        public void ReplaceToShnGSleepers()
        {

            foreach (var prefab in Resources.FindObjectsOfTypeAll<NetInfo>())
            {
                //split up because of nodeless tracks!
                if (prefab.m_segments.Length >= 2)
                {
                    Debug.Log("Segments > 2" + prefab.name);
                    foreach (var oldname in tobeReplacedNames1T)
                    {
                        if (prefab.m_segments[1].m_segmentMesh.name == oldname)
                        {
                            prefab.m_segments[1].m_segmentMesh = replaceToMesh1T;
                            prefab.m_segments[1].m_segmentMaterial = replaceToMaterial1T;
                        }
                    }

                    foreach (var oldname2 in tobeReplacedNames2T)
                    {
                        if (prefab.m_segments[1].m_segmentMesh.name == oldname2)
                        {
                            prefab.m_segments[1].m_segmentMesh = replaceToMesh2T;
                            prefab.m_segments[1].m_segmentMaterial = replaceToMaterial2T;
                        }
                    }
                }

                if (prefab.m_nodes.Length > 2)
                {
                    Debug.Log("Nodes > 2" + prefab.name);

                    foreach (var oldname in tobeReplacedNames1T)
                    {
                        if (prefab.m_nodes[1].m_nodeMesh.name == oldname)
                        {
                            prefab.m_nodes[1].m_nodeMesh = replaceToMesh1T;
                            prefab.m_nodes[1].m_nodeMaterial = replaceToMaterial1T;
                        }
                    }

                    foreach (var oldname2 in tobeReplacedNames2T)
                    {
                        if (prefab.m_nodes[1].m_nodeMesh.name == oldname2)
                        {
                            prefab.m_nodes[1].m_nodeMesh = replaceToMesh2T;
                            prefab.m_nodes[1].m_nodeMaterial = replaceToMaterial2T;
                        }
                    }

                    //case for 1 to 2 track node transition

                    for (int i = 2; i < prefab.m_nodes.Length; i++)
                    {

                        if (prefab.m_nodes[i].m_nodeMesh.name == "r-sleepers-s-node1" ||
                            prefab.m_nodes[i].m_nodeMesh.name == "r-sleepers-s-node1f")
                        {
                            prefab.m_nodes[i].m_nodeMesh = replaceToMeshNode1;
                            prefab.m_nodes[i].m_nodeMaterial = replaceToMaterialNode1;
                        }

                        if (prefab.m_nodes[i].m_nodeMesh.name == "r-sleepers-s-node2")
                        {
                            prefab.m_nodes[i].m_nodeMesh = replaceToMeshNode2;
                            prefab.m_nodes[i].m_nodeMaterial = replaceToMaterialNode2;
                        }

                        if (prefab.m_nodes[i].m_nodeMesh.name == "r-sleepers-s-nodef" ||
                            prefab.m_nodes[i].m_nodeMesh.name == "r-sleepersw_0.017")
                        {
                            prefab.m_nodes[i].m_nodeMesh = replaceToMesh1T;
                            prefab.m_nodes[i].m_nodeMaterial = replaceToMaterial1T;
                        }

                    }

                }

            }
        }
    }
}