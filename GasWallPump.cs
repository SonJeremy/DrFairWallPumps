﻿using UnityEngine;
using TUNING;
using FairONI;
using STRINGS;

namespace WallPumps
{
    public class GasWallPump : IBuildingConfig
    {
        public const string ID = "FairGasWallPump";
        
        public static void Setup()
        {
            AddBuilding.AddStrings(ID, "Gas Wall Pump", "A gas pump that's also a wall", "Pumps out " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from a room");
        }

        public static void AddToMenus()
        {
            if (WallPumpsConfig.GetConfig().gasWallPump.enabled)
            {
                AddBuilding.AddBuildingToPlanScreen("HVAC", ID, "GasPump");
                AddBuilding.IntoTechTree("ImprovedGasPiping", ID);
            }
        }

        public override BuildingDef CreateBuildingDef()
        {
            string[] constructionMats = { WallPumps.WallMachineRefinedMetals.Name };
            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                1,
                "fairgaswallpump_kanim",
                30,
                30f,
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
                constructionMats,
                1600f,
                BuildLocationRule.Tile,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NOISY.TIER2,
                0.2f);
            BuildingTemplates.CreateFoundationTileDef(def);

            def.RequiresPowerInput = true;
            def.EnergyConsumptionWhenActive = WallPumpsConfig.GetConfig().gasWallPump.energyConsumption;
            def.ExhaustKilowattsWhenActive = 0f;
            def.SelfHeatKilowattsWhenActive = 0f;
            def.OutputConduitType = ConduitType.Gas;
            def.Floodable = false;
            def.ViewMode = OverlayModes.GasConduits.ID;
            def.AudioCategory = "Metal";
            def.PowerInputOffset = new CellOffset(0, 0);
            def.UtilityOutputOffset = new CellOffset(0, 0);
            def.PermittedRotations = PermittedRotations.R360;
            // Tile properties
            def.ThermalConductivity = WallPumpsConfig.GetConfig().gasWallPump.thermalConductivity;
            def.UseStructureTemperature = false;
            def.Entombable = false;
            def.BaseTimeUntilRepair = -1f;
            def.ObjectLayer = ObjectLayer.Building;
            def.SceneLayer = Grid.SceneLayer.TileMain;
            def.ForegroundLayer = Grid.SceneLayer.TileMain;
            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<Insulator>();
            go.AddOrGet<TileTemperature>();
            BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
            buildingHP.destroyOnDamaged = true;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            AddVisualizer(go, true);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            AddVisualizer(go, false);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            //go.AddOrGetDef<StorageController.Def>();

            GeneratedBuildings.RegisterSingleLogicInputPort(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<RotatablePump>();
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = WallPumpsConfig.GetConfig().gasWallPump.pumpRate * 2;
            RotatableElementConsumer elementConsumer = go.AddOrGet<RotatableElementConsumer>();
            elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
            elementConsumer.consumptionRate = WallPumpsConfig.GetConfig().gasWallPump.pumpRate;
            elementConsumer.storeOnConsume = true;
            elementConsumer.showInStatusPanel = false;
            elementConsumer.rotatableCellOffset = new Vector3(0, 1);
            elementConsumer.consumptionRadius = 2;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = null;
            go.AddOrGetDef<OperationalController.Def>();
            AddVisualizer(go, false);

            GeneratedBuildings.RemoveLoopingSounds(go);
        }

        private static void AddVisualizer(GameObject go, bool movable)
        {
            RangeVisualizer _RangeVisualizer = go.AddOrGet<RangeVisualizer>();

            _RangeVisualizer.OriginOffset = new Vector2I(0, 1);
            _RangeVisualizer.RangeMin.x = 0;
            _RangeVisualizer.RangeMin.y = 0;
            _RangeVisualizer.RangeMax.x = 0;
            _RangeVisualizer.RangeMax.y = 0;
        }
    }
}
