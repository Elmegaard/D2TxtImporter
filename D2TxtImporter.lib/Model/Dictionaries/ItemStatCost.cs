using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace D2TxtImporter.lib.Model
{
    public class ItemStatCost
    {
        [JsonIgnore]
        public string Stat { get; set; }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int? Op { get; set; }

        [JsonIgnore]
        public int? OpParam { get; set; }

        [JsonIgnore]
        public int? DescriptionPriority { get; set; }

        [JsonIgnore]
        public int? DescriptionFunction { get; set; }

        [JsonIgnore]
        public int? DescriptionValue { get; set; }

        [JsonIgnore]
        public string DescriptonStringPositive { get; set; }

        [JsonIgnore]
        public string DescriptionStringNegative { get; set; }

        [JsonIgnore]
        public string DescriptionString2 { get; set; }

        [JsonIgnore]
        public int? GroupDescription { get; set; }

        [JsonIgnore]
        public int? GroupDescriptionFunction { get; set; }

        [JsonIgnore]
        public int? GroupDescriptionValue { get; set; }

        [JsonIgnore]
        public string GroupDescriptonStringPositive { get; set; }

        [JsonIgnore]
        public string GroupDescriptionStringNegative { get; set; }

        [JsonIgnore]
        public string GroupDescriptionString2 { get; set; }

        [JsonIgnore]
        public static Dictionary<string, ItemStatCost> ItemStatCosts;

        public static void Import(string excelFolder)
        {
            ItemStatCosts = new Dictionary<string, ItemStatCost>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/ItemStatCost.txt");

            foreach (var row in table)
            {
                var itemStatCost = new ItemStatCost
                {

                    Stat = row["Stat"],
                    Id = int.Parse(row["ID"]),
                    Op = Utility.ToNullableInt(row["op"]),
                    OpParam = Utility.ToNullableInt(row["op param"]),
                    DescriptionPriority = Utility.ToNullableInt(row["descpriority"]),
                    DescriptionFunction = Utility.ToNullableInt(row["descfunc"]),
                    DescriptionValue = Utility.ToNullableInt(row["descval"]),
                    DescriptonStringPositive = Table.GetValue(row["descstrpos"]),
                    DescriptionStringNegative = Table.GetValue(row["descstrneg"]),
                    DescriptionString2 = Table.GetValue(row["descstr2"]),
                    GroupDescription = Utility.ToNullableInt(row["dgrp"]),
                    GroupDescriptionFunction = Utility.ToNullableInt(row["dgrpfunc"]),
                    GroupDescriptionValue = Utility.ToNullableInt(row["dgrpval"]),
                    GroupDescriptonStringPositive = Table.GetValue(row["dgrpstrpos"]),
                    GroupDescriptionStringNegative = Table.GetValue(row["dgrpstrneg"]),
                    GroupDescriptionString2 = Table.GetValue(row["dgrpstr2"])
                };

                ItemStatCosts[itemStatCost.Stat] = itemStatCost;
            }

            HardcodedTableStats();
            FixBrokenEntries();
        }

        public override string ToString()
        {
            return Stat;
        }

        public static void HardcodedTableStats()
        {
            var enhancedDamage = new ItemStatCost
            {
                Stat = "dmg%",
                DescriptionPriority = 144, // 1 below attack speed (seems right)
                DescriptionFunction = 4, // +val%
                DescriptonStringPositive = "Enhanced Damage",
                DescriptionStringNegative = "Enhanced Damage",
                DescriptionValue = 1 // Add value before
            };

            ItemStatCosts[enhancedDamage.Stat] = enhancedDamage;

            var ethereal = new ItemStatCost
            {
                Stat = "ethereal",
                DescriptionPriority = 1, // Min priority
                DescriptionFunction = 1, // lstValue
                DescriptonStringPositive = "Ethereal (Cannot Be Repaired)",
                DescriptionStringNegative = "Ethereal (Cannot Be Repaired)",
                DescriptionValue = 0 // Do not add value
            };

            ItemStatCosts[ethereal.Stat] = ethereal;

            var eledam = new ItemStatCost
            {
                Stat = "eledam",
                DescriptionPriority = ItemStatCosts["firemindam"].DescriptionPriority,
                DescriptionFunction = 30,
                DescriptonStringPositive = "Adds %d %s damage",
                DescriptionValue = 3
            };

            ItemStatCosts["eledam"] = eledam;
        }

        public static void FixBrokenEntries()
        {
            var sockets = ItemStatCosts["item_numsockets"];
            sockets.DescriptionPriority = 1;
            sockets.DescriptionFunction = 29;
            sockets.DescriptonStringPositive = "Socketed";
            sockets.GroupDescriptionStringNegative = "Socketed";
            sockets.DescriptionValue = 3; // Use value as is
        }

        public string PropertyString(int? value, int? value2, string parameter, int itemLevel)
        {
            string lstValue;
            var valueString = GetValueString(value, value2);

            if (DescriptonStringPositive == null)
            {
                lstValue = Stat;
            }
            else if (value2.HasValue && value2.Value < 0)
            {
                lstValue = DescriptionStringNegative;
            }
            else
            {
                lstValue = DescriptonStringPositive;
            }

            if (DescriptionFunction.HasValue)
            {
                if (DescriptionFunction.Value >= 1 && DescriptionFunction.Value <= 4 && lstValue.Contains("%d"))
                {
                    valueString = lstValue.Replace("%d", valueString);
                    DescriptionValue = 3;
                }
                else
                {
                    switch (DescriptionFunction.Value)
                    {
                        case 1:
                            valueString = $"+{valueString}";
                            break;
                        case 2:
                            valueString = $"{valueString}%";
                            break;
                        case 3:
                            valueString = $"{valueString}";
                            break;
                        case 4:
                            valueString = $"+{valueString}%";
                            break;
                        case 5:
                            if (value.HasValue)
                            {
                                value = value * 100 / 128;
                            }
                            if (value2.HasValue)
                            {
                                value2 = value2 * 100 / 128;
                            }
                            valueString = GetValueString(value, value2);
                            valueString = $"{valueString}%";
                            break;
                        case 6:
                            double val1 = 0;
                            double val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} (Based on Character Level)";
                            break;
                        case 7:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"({valueString}% Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)}% {lstValue} (Based on Character Level)";
                            break;
                        case 8:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} (Based on Character Level)";
                            break;
                        case 9:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"{lstValue} {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} ({valueString} Per Character Level)";
                            break;
                        case 11:
                            valueString = lstValue.Replace("%d", $"{((double)(Utility.ToNullableInt(parameter).Value / 100f)).ToString(CultureInfo.InvariantCulture)}");
                            DescriptionValue = 3;
                            break;
                        case 12:
                            valueString = $"+{valueString}";
                            break;
                        case 13:
                            var classReplace = "";
                            valueString = $"+{valueString}";

                            var regex = Regex.Match(parameter, @"randclassskill(\d+)"); // Work with custom randclasskill(digit) 
                            if (regex.Success)
                            {
                                classReplace = "(Random Class)";
                                valueString = $"+{regex.Groups[1].Value}";
                            }
                            else if (parameter == "randclassskill")
                            {
                                classReplace = "(Random Class)";
                                valueString = "+3";
                            }
                            else
                            {
                                if (!CharStat.CharStats.ContainsKey(parameter))
                                {
                                    throw new Exception($"Could not find character class '{parameter}'\nNote: if you have made a custom version of 'randclassskill' to support different amount of skills change them to 'randclassskill<d>' for example 'randclassskill5' is supported.");
                                }
                                classReplace = CharStat.CharStats[parameter].Class;
                            }
                            lstValue = lstValue.Replace("%d", classReplace);
                            break;
                        case 14:
                            var par = Utility.ToNullableInt(parameter);
                            if (!par.HasValue)
                            {
                                throw new Exception($"Could not convert parameter '{parameter}' to a valid integer");
                            }

                            if (!CharStat.SkillTabs.ContainsKey(par.Value))
                            {
                                throw new Exception($"Could not find skill tab with id {par.Value}");
                            }

                            var skillTab = CharStat.SkillTabs[par.Value];

                            var className = CharStat.CharStats.Values.First(x => x.StrSkillTab1 == skillTab || x.StrSkillTab2 == skillTab || x.StrSkillTab3 == skillTab).Class;

                            if (!Table.Tables.ContainsKey(skillTab))
                            {
                                throw new Exception($"Could not find translation key '{skillTab}' in any .tbl file");
                            }

                            lstValue = Table.Tables[skillTab];
                            valueString = $"{lstValue.Replace("%d", valueString)} ({className} only)";
                            break;
                        case 15:
                            valueString = value2.Value.ToString();
                            var skill = Skill.GetSkill(parameter);

                            if (value2.Value == 0)
                            {
                                val1 = Math.Min(Math.Ceiling((itemLevel - (skill.RequiredLevel - 1)) / 3.9), 20);
                                val2 = Math.Min(Math.Round((99 - (skill.RequiredLevel - 1)) / 3.9), 20);

                                valueString = GetValueString(val1, val2);
                            }

                            valueString = lstValue.Replace("%d%", value.Value.ToString())
                                                     .Replace("%d", valueString)
                                                     .Replace("%s", skill.SkillDesc);
                            break;
                        case 16:
                            valueString = lstValue.Replace("%d", valueString)
                                                  .Replace("%s", Skill.GetSkill(parameter).Name);
                            DescriptionValue = 3;
                            break;
                        case 20:
                            valueString = $"{GetValueString(value * -1, value2 * -1)}%";
                            break;
                        case 23:
                            if (!MonStat.MonStats.ContainsKey(parameter))
                            {
                                throw new Exception($"Could not find monster with id '{parameter}' in MonStats.txt");
                            }
                            valueString = $"{valueString}% {lstValue} {MonStat.MonStats[parameter].NameStr}";
                            DescriptionValue = 3;
                            break;
                        case 24:
                            valueString = $"Level {value2} {Skill.GetSkill(parameter).Name} ({value} Charges)";
                            break;
                        case 27:
                            var charClass = Skill.GetSkill(parameter).CharClass;
                            var reqString = "";
                            if (!string.IsNullOrEmpty(charClass))
                            {
                                // Add requirement if one is there
                                if (!CharStat.CharStats.ContainsKey(Skill.GetSkill(parameter).CharClass))
                                {
                                    throw new Exception($"Could not find character skill tab '{Skill.GetSkill(parameter).CharClass}' property");
                                }
                                reqString = $" ({CharStat.CharStats[Skill.GetSkill(parameter).CharClass].Class} Only)";
                            }
                            valueString = $"+{valueString} to {Skill.GetSkill(parameter).Name}{reqString}";
                            break;
                        case 28:
                            valueString = $"+{valueString} to {Skill.GetSkill(parameter).Name}";
                            break;
                        case 29: // Custom for sockets
                            if (string.IsNullOrEmpty(valueString))
                            {
                                valueString = parameter;
                            }

                            valueString = $"{lstValue} ({valueString})";
                            break;
                        case 30: // Custom for elemental damage
                            valueString = lstValue.Replace("%d", valueString).Replace("%s", parameter);
                            break;
                        default:
                            // Not implemented function
                            valueString = UnimplementedFunction(value, value2, parameter, Op, OpParam, DescriptionFunction.Value);
                            DescriptionValue = 3;
                            break;
                    }
                }
            }

            // Remove +- in case it happens
            valueString = valueString.Replace("+-", "-");

            if (DescriptionValue.HasValue && !string.IsNullOrEmpty(lstValue))
            {
                switch (DescriptionValue.Value)
                {
                    case 0:
                        valueString = lstValue;
                        break;
                    case 1:
                        valueString = $"{valueString} {lstValue}";
                        break;
                    case 2:
                        valueString = $"{lstValue} {valueString}";
                        break;
                    case 3:
                        // Used if the value already contain all information
                        break;
                }
            }

            // Trim whitespace and remove trailing newline as we sometimes see those in the properties
            valueString = valueString.Trim().Replace("\\n", "");
            return valueString;
        }

        private static double CalculatePerLevel(string parameter, int? op, int? op_param, string stat)
        {
            var val = int.Parse(parameter) / 8d;
            return val;
        }

        private static string GetValueString(double? value = null, double? value2 = null)
        {
            var valueString = "";

            if (value.HasValue)
            {
                valueString += value.Value.ToString(CultureInfo.InvariantCulture);

                if (value2.HasValue && value.Value != value2.Value)
                {
                    if (value2.Value >= 0)
                    {
                        valueString += $"-{value2.Value.ToString(CultureInfo.InvariantCulture)}";
                    }
                    else
                    {
                        valueString += $" to {value2.Value.ToString(CultureInfo.InvariantCulture)}";
                    }
                }
            }

            return valueString;
        }

        private static string UnimplementedFunction(int? value1, int? value2, string paramter, int? op, int? opParam, int function)
        {
            // Sad face :(
            return $"TODO: Unimplemented function: '{function}' value1: '{(value1.HasValue ? value1.Value.ToString() : "null")}' value2: '{(value2.HasValue ? value2.Value.ToString() : "null")}' parameter: '{paramter}' op: '{(op.HasValue ? op.Value.ToString() : "null")}' op_param: '{(opParam.HasValue ? opParam.Value.ToString() : "null")}'";
        }
    }
}
