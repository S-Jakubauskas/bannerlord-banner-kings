﻿using BannerKings.Managers.Titles;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using BannerKings.Managers.Titles.Governments;
using BannerKings.UI.Cutscenes;
using TaleWorlds.Core;

namespace BannerKings.Managers.Helpers
{
    public static class TitleGenerator
    {
        internal static FeudalTitle CreateKingdom(Hero deJure, Kingdom faction, TitleType type, List<FeudalTitle> vassals, FeudalContract contract, string stringId = null, TextObject name = null, TextObject fullName = null)
        {
            var title = new FeudalTitle(type, 
                null, 
                vassals, 
                deJure, 
                faction != null ? faction.Leader : null, 
                name != null ? name : faction.Name, 
                contract, 
                stringId, 
                fullName);;
            BannerKingsConfig.Instance.TitleManager.ExecuteAddTitle(title);
            BannerKingsConfig.Instance.TitleManager.Kingdoms[title] = faction;
            return title;
        }

        internal static FeudalTitle CreateEmpire(Hero deJure, Kingdom faction, List<FeudalTitle> vassals,
          FeudalContract contract, string stringId = null)
        {
            var title = new FeudalTitle(TitleType.Empire, null, vassals, deJure, faction.Leader,
                faction.Name, contract, stringId);
            BannerKingsConfig.Instance.TitleManager.ExecuteAddTitle(title);
            BannerKingsConfig.Instance.TitleManager.Kingdoms[title] = faction;

            return title;
        }

        public static void FoundKingdom(TitleAction action, FeudalContract contract = null)
        {
            FeudalTitle newTitle = CreateKingdom(action.ActionTaker, 
                action.ActionTaker.Clan.Kingdom, 
                TitleType.Kingdom, 
                new List<FeudalTitle>(action.Vassals),
                contract != null ? contract : action.Title.Contract);

            action.Title.DriftTitle(newTitle, false);
            foreach (var vassal in action.Vassals)
            {
                vassal.DriftTitle(newTitle);
            }

            TitleAction newAction = BannerKingsConfig.Instance.TitleModel.GetFoundKingdom(action.ActionTaker.Clan.Kingdom, 
                action.ActionTaker);
            newAction.SetTile(newTitle);

            BannerKingsConfig.Instance.TitleManager.CreateTitle(newAction);
        }

        public static void FoundEmpire(TitleAction action, TextObject factionName, string stringId = null, string contractType = null)
        {
            var kingdom = action.ActionTaker.Clan.Kingdom;
            kingdom.ChangeKingdomName(factionName, factionName);
            var newTitle = CreateEmpire(action.ActionTaker, kingdom, new List<FeudalTitle>(action.Vassals),
                GenerateContract(contractType), stringId);

            foreach (var vassal in action.Vassals)
            {
                vassal.DriftTitle(newTitle);
            }

            TitleAction newAction = BannerKingsConfig.Instance.TitleModel.GetFoundEmpire(action.ActionTaker.Clan.Kingdom,
                action.ActionTaker);
            newAction.SetTile(newTitle);

            BannerKingsConfig.Instance.TitleManager.CreateTitle(newAction);
            MBInformationManager.ShowSceneNotification(new EmpireFoundedScene(kingdom, newTitle));
        }

        private static Hero GetDeJure(string heroId, Settlement settlement)
        {
            var target = Hero.AllAliveHeroes.FirstOrDefault(x => x.StringId == heroId);
            if (target == null)
            {
                var hero1Dead = Hero.DeadOrDisabledHeroes.FirstOrDefault(x => x.StringId == heroId);
                if (hero1Dead != null)
                {
                    var clan = hero1Dead.Clan;
                    if (!clan.IsEliminated)
                    {
                        target = clan.Leader;
                    }
                    else if (clan.Kingdom != null)
                    {
                        target = clan.Kingdom.Leader;
                    }
                }
            }

            if (target == null && settlement != null)
            {
                target = settlement.Owner;
            }

            return target;
        }

        internal static FeudalContract GenerateContract(string type)
        {
            var contract = type switch
            {
                "feudal_elective" => new FeudalContract(
                    DefaultGovernments.Instance.Feudal, 
                    DefaultSuccessions.Instance.FeudalElective,
                    DefaultInheritances.Instance.Primogeniture,
                    DefaultGenderLaws.Instance.Agnatic),
                "imperial" => new FeudalContract(
                     DefaultGovernments.Instance.Feudal, DefaultSuccessions.Instance.FeudalElective,
                     DefaultInheritances.Instance.Primogeniture,
                     DefaultGenderLaws.Instance.Agnatic),
                "tribal" => new FeudalContract(
                     DefaultGovernments.Instance.Feudal, DefaultSuccessions.Instance.FeudalElective,
                     DefaultInheritances.Instance.Primogeniture,
                     DefaultGenderLaws.Instance.Agnatic),
                "republic" => new FeudalContract(
                     DefaultGovernments.Instance.Republic, DefaultSuccessions.Instance.Republic,
                     DefaultInheritances.Instance.Primogeniture,
                     DefaultGenderLaws.Instance.Agnatic),
                _ => new FeudalContract(
                     DefaultGovernments.Instance.Feudal, DefaultSuccessions.Instance.Hereditary,
                     DefaultInheritances.Instance.Primogeniture,
                     DefaultGenderLaws.Instance.Agnatic)
            };

            return contract;
        }

        public static FeudalContract GenerateContract(string government, string succession, string inheritance, string genderLaw)
        {
            return new FeudalContract(
                    DefaultGovernments.Instance.GetById(government), 
                    DefaultSuccessions.Instance.GetById(succession), 
                    DefaultInheritances.Instance.GetById(inheritance),
                    DefaultGenderLaws.Instance.GetById(genderLaw));
        }

        internal static FeudalTitle CreateUnlandedTitle(Hero deJure, TitleType type, List<FeudalTitle> vassals,
            TextObject name, FeudalContract contract, TextObject fullName = null)
        {
            var title = new FeudalTitle(type, null, vassals, deJure, deJure, name, contract, null, fullName);
            BannerKingsConfig.Instance.TitleManager.ExecuteAddTitle(title);
            return title;
        }

        internal static FeudalTitle CreateLandedTitle(Settlement settlement, Hero deJure, TitleType type,
            FeudalContract contract, List<FeudalTitle> vassals = null, TextObject fullName = null)
        {
            var deFacto = settlement.OwnerClan.Leader;
            if (deJure == null)
            {
                deJure = settlement.Owner;
            }

            if (vassals == null)
            {
                vassals = new List<FeudalTitle>();
            }

            if (settlement.BoundVillages != null)
            {
                foreach (var lordship in settlement.BoundVillages)
                {
                    var lordshipTitle = CreateLordship(lordship.Settlement, deJure, contract);
                    vassals.Add(lordshipTitle);
                    BannerKingsConfig.Instance.TitleManager.ExecuteAddTitle(lordshipTitle);
                }
            }

            var title = new FeudalTitle(type, settlement, vassals, deJure, deFacto, settlement.Name, contract, null, fullName);
            BannerKingsConfig.Instance.TitleManager.ExecuteAddTitle(title);
            return title;
        }

        internal static FeudalTitle CreateLordship(Settlement settlement, Hero deJure, FeudalContract contract)
        {
            return new FeudalTitle(TitleType.Lordship, settlement, null,
                deJure, settlement.Village.Bound.Owner, settlement.Name, contract);
        }

        internal static void GenerateDuchy(XmlNode duchy, List<FeudalTitle> vassalsKingdom, FeudalContract contract)
        {
            var vassalsDuchy = new List<FeudalTitle>();
            var dukedomName = new TextObject(duchy.Attributes["name"].Value);
            TextObject dukedomFullName = null;
            XmlAttribute dukedomFullNameAttribute = duchy.Attributes["fullName"];
            if (dukedomFullNameAttribute != null)
            {
                dukedomFullName = new TextObject(dukedomFullNameAttribute.Value);
            }

            string deJureName = null;
            XmlAttribute deJureNameAttribute = duchy.Attributes["deJure"];
            if (deJureNameAttribute != null) deJureName = deJureNameAttribute.Value;

            Hero deJureDuchy = null;
            if (deJureName != null) deJureDuchy = GetDeJure(deJureName, null);

            if (contract == null)
            {
                string government = duchy.Attributes["government"].Value;
                string succession = duchy.Attributes["succession"].Value;
                string inheritance = duchy.Attributes["inheritance"].Value;
                string genderLaw = duchy.Attributes["genderLaw"].Value;
                contract = GenerateContract(government, succession, inheritance, genderLaw);
            }

            if (duchy.ChildNodes != null)
            {
                foreach (XmlNode county in duchy.ChildNodes)
                {
                    if (county.Name != "county")
                    {
                        return;
                    }

                    var settlementNameCounty = county.Attributes["settlement"].Value;
                    var deJureNameCounty = county.Attributes["deJure"].Value;
                    TextObject countyName = null;
                    XmlAttribute countyFullNameAttribute = county.Attributes["fullName"];
                    if (countyFullNameAttribute != null)
                    {
                        countyName = new TextObject(countyFullNameAttribute.Value);
                    }

                    var settlementCounty = Settlement.All.FirstOrDefault(x => x.Name.ToString() == settlementNameCounty);
                    if (settlementCounty == null)
                    {
                        settlementCounty = Settlement.All.FirstOrDefault(x => x.StringId.ToString() == settlementNameCounty);
                    }

                    var deJureCounty = GetDeJure(deJureNameCounty, settlementCounty);
                    var vassalsCounty = new List<FeudalTitle>();

                    if (county.ChildNodes != null)
                    {
                        foreach (XmlNode barony in county.ChildNodes)
                        {
                            if (barony.Name != "barony")
                            {
                                return;
                            }

                            TextObject baronyName = null;
                            XmlAttribute barnyFullNameAttribute = barony.Attributes["fullName"];
                            if (barnyFullNameAttribute != null)
                            {
                                baronyName = new TextObject(barnyFullNameAttribute.Value);
                            }

                            var settlementNameBarony = barony.Attributes["settlement"].Value;
                            var deJureIdBarony = barony.Attributes["deJure"].Value;
                            var settlementBarony = Settlement.All.FirstOrDefault(x => x.Name.ToString() == settlementNameBarony);
                            if (settlementBarony == null)
                            {
                                settlementBarony = Settlement.All.FirstOrDefault(x => x.StringId.ToString() == settlementNameBarony);
                            }

                            var deJureBarony = GetDeJure(deJureIdBarony, settlementBarony);
                            if (settlementBarony != null)
                            {
                                vassalsCounty.Add(CreateLandedTitle(settlementBarony,
                                    deJureBarony,
                                    TitleType.Barony,
                                    contract,
                                    null,
                                    baronyName));
                            }
                        }
                    }

                    if (settlementCounty != null)
                    {
                        vassalsDuchy.Add(CreateLandedTitle(settlementCounty,
                            deJureCounty,
                            TitleType.County,
                            contract,
                            vassalsCounty,
                            countyName));
                    }
                }
            }

            var title = CreateUnlandedTitle(deJureDuchy, TitleType.Dukedom, vassalsDuchy, dukedomName, contract,
                dukedomFullName);
            if (vassalsKingdom != null) vassalsKingdom.Add(title);      
        }

        internal static void GenerateKingdom(XmlNode kingdom)
        {
            var vassalsKingdom = new List<FeudalTitle>();

            string factionName = null;
            XmlAttribute factionNameAttribute = kingdom.Attributes["faction"];
            if (factionNameAttribute != null) factionName = factionNameAttribute.Value;

            Kingdom faction = null;
            if (factionName != null)
            {
                faction = Kingdom.All.FirstOrDefault(x => x.Name.ToString() == factionName);
                if (faction == null)
                {
                    faction = Kingdom.All.FirstOrDefault(x => x.StringId.ToString() == factionName);
                }
            }

            string deJureName = null;
            XmlAttribute deJureNameAttribute = kingdom.Attributes["deJure"];
            if (deJureNameAttribute != null) deJureName = deJureNameAttribute.Value;    

            Hero deJureKingdom = null;
            if (deJureName != null) deJureKingdom = GetDeJure(deJureName, null);

            TextObject kingdomName = null;
            XmlAttribute kingdomNameAttribute = kingdom.Attributes["name"];
            if (kingdomNameAttribute != null) kingdomName = new TextObject(kingdomNameAttribute.Value);

            TextObject kingdomFullName = null;
            XmlAttribute kingdomFullNameAttribute = kingdom.Attributes["fullName"];
            if (kingdomFullNameAttribute != null) kingdomFullName = new TextObject(kingdomFullNameAttribute.Value);   

            string id = null;
            XmlAttribute idAttribute = kingdom.Attributes["id"];
            if (idAttribute != null) id = idAttribute.Value;  

            string government = kingdom.Attributes["government"].Value;
            string succession = kingdom.Attributes["succession"].Value;
            string inheritance = kingdom.Attributes["inheritance"].Value;
            string genderLaw = kingdom.Attributes["genderLaw"].Value;
            var contract = GenerateContract(government, succession, inheritance, genderLaw);
            if (contract == null) return;
            

            if (kingdom.ChildNodes != null)
            {
                foreach (XmlNode duchy in kingdom.ChildNodes)
                {
                    if (duchy.Name != "duchy") return;
                    GenerateDuchy(duchy, vassalsKingdom, contract);
                }
            }

            var sovereign = CreateKingdom(deJureKingdom, faction, TitleType.Kingdom, vassalsKingdom, contract, id, kingdomName, kingdomFullName);
            foreach (var duchy in vassalsKingdom)
            {
                duchy.SetSovereign(sovereign);
            }
        }

        internal static void InitializeTitles()
        {
            XmlDocument doc = Utils.Helpers.CreateDocumentFromXmlFile(BannerKingsConfig.Instance.TitlesGeneratorPath);
            var titlesNode = doc.ChildNodes[1].ChildNodes[0];
            var autoGenerate = bool.Parse(titlesNode.Attributes["autoGenerate"].Value);

            foreach (XmlNode node in titlesNode)
            {
                if (node.Name == "kingdom") GenerateKingdom(node);
                if (node.Name == "duchy") GenerateDuchy(node, null, null);
            }

            if (autoGenerate)
            {
                foreach (var settlement in Settlement.All)
                {
                    if (settlement.IsVillage)
                    {
                        continue;
                    }

                    if (settlement.OwnerClan is { Leader: { } } &&
                        (settlement.IsTown || settlement.IsCastle))
                    {
                        CreateLandedTitle(settlement,
                            settlement.Owner, settlement.IsTown ? TitleType.County : TitleType.Barony,
                            GenerateContract("feudal"));
                    }
                }
            }
        }
    }
}
