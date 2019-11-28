using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class ItemStatCost
    {
        public string Stat { get; set; }
        public int Id { get; set; }
        public int? Op { get; set; }
        public int? OpParam { get; set; }
        public int? DescriptionPriority { get; set; }
        public int? DescriptionFunction { get; set; }
        public int? DescriptionValue { get; set; }
        public string DescriptonStringPositive { get; set; }
        public string DescriptionStringNegative { get; set; }
        public string DescriptionString2 { get; set; }
        public int? GroupDescription { get; set; }
        public int? GroupDescriptionFunction { get; set; }
        public int? GroupDescriptionValue { get; set; }
        public string GroupDescriptonStringPositive { get; set; }
        public string GroupDescriptionStringNegative { get; set; }
        public string GroupDescriptionString2 { get; set; }


        public static Dictionary<string, ItemStatCost> ItemStatCosts;

        public static void Import(string excelFolder)
        {
            ItemStatCosts = new Dictionary<string, ItemStatCost>();

            var lines = Importer.ReadCsvFile(excelFolder + "/ItemStatCost.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var itemStatCost = new ItemStatCost
                {
                    Stat = values[0],
                    Id = int.Parse(values[1]),
                    Op = Utility.ToNullableInt(values[25]),
                    OpParam = Utility.ToNullableInt(values[26]),
                    DescriptionPriority = Utility.ToNullableInt(values[39]),
                    DescriptionFunction = Utility.ToNullableInt(values[40]),
                    DescriptionValue = Utility.ToNullableInt(values[41]),
                    DescriptonStringPositive = Table.GetValue(values[42]),
                    DescriptionStringNegative = Table.GetValue(values[43]),
                    DescriptionString2 = Table.GetValue(values[44]),
                    GroupDescription = Utility.ToNullableInt(values[45]),
                    GroupDescriptionFunction = Utility.ToNullableInt(values[46]),
                    GroupDescriptionValue = Utility.ToNullableInt(values[47]),
                    GroupDescriptonStringPositive = Table.GetValue(values[48]),
                    GroupDescriptionStringNegative = Table.GetValue(values[49]),
                    GroupDescriptionString2 = Table.GetValue(values[50]),
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

        public string PropertyString(int? value, int? value2, string parameter)
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
                            if (parameter == "randclassskill")
                            {
                                classReplace = "(Random Class)";
                            }
                            else
                            {
                                classReplace = CharStat.CharStats[parameter].Class;
                            }
                            lstValue = lstValue.Replace("%d", classReplace);
                            valueString = $"+{valueString}";
                            break;
                        case 14:
                            valueString = lstValue.Replace("%d", valueString);
                            break;
                        case 15:
                            // TODO: Doesn't work when a skills has fx 13-20 range of levels. Value is 0 then.
                            valueString = lstValue.Replace("%d%", value.Value.ToString())
                                                     .Replace("%d", value2.Value.ToString())
                                                     .Replace("%s", Skill.GetSkill(parameter).SkillDesc);

                            if (value2.Value == 0)
                            {
                                valueString += " - TODO: Why is this showing 0 for ranges?";
                            }
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
            var val = int.Parse(parameter) / Math.Pow(op.Value, op_param.Value);
            if (stat == "item_maxdamage_perlevel")
            {
                val *= 10;
                val = Math.Round(val * 4, MidpointRounding.ToEven) / 4; // Round to nearest quarter
            }
            else if (stat == "item_armor_perlevel")
            {
                val *= 8;
                val = Math.Round(val * 4, MidpointRounding.ToEven) / 4; // Round to nearest quarter
            }

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
