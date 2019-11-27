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
        public int? Add { get; set; }
        public int? Multiply { get; set; }
        public int? Divide { get; set; }
        public int? Op { get; set; }
        public int? OpParam { get; set; }
        public int? DescriptionPriority { get; set; }
        public int? DescriptionFunction { get; set; }
        public string DescriptionFunctionString { get; set; }
        public int? DescriptionValue { get; set; }
        public string DescriptonStringPositive { get; set; }
        public string DescriptionStringNegative { get; set; }
        public string DescriptionString2 { get; set; }
        public int? GroupDescription { get; set; }
        public int? GroupDescriptionFunction { get; set; }
        public string GroupDescriptionFunctionString { get; set; }
        public int? GroupDescriptionValue { get; set; }
        public string GroupDescriptonStringPositive { get; set; }
        public string GroupDescriptionStringNegative { get; set; }
        public string GroupDescriptionString2 { get; set; }


        public static Dictionary<string, ItemStatCost> ItemStatCosts;

        public static void Import(string excelFolder)
        {
            ItemStatCosts = new Dictionary<string, ItemStatCost>();
            HardcodedTableStats();

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
                    Add = Utility.ToNullableInt(values[15]),
                    Multiply = Utility.ToNullableInt(values[16]),
                    Divide = Utility.ToNullableInt(values[17]),
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
                    DescriptionFunctionString = GetDescriptionFunction(Utility.ToNullableInt(values[40])),
                    GroupDescriptionFunctionString = GetDescriptionFunction(Utility.ToNullableInt(values[46]))
                };

                ItemStatCosts[itemStatCost.Stat] = itemStatCost;
            }

            FixBrokenEntries();
        }

        public override string ToString()
        {
            return Stat;
        }

        public static string GetDescriptionFunction(int? key)
        {
            if (key.HasValue)
            {
                return DescriptionFunctions[key.Value];
            }
            return null;
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
                            double val = 0;
                            if (string.IsNullOrEmpty(valueString))
                            {
                                val = int.Parse(parameter) / Math.Pow(Op.Value, OpParam.Value);
                                valueString = GetValueString(val);
                            }
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} (Based on Character Level)";
                            break;
                        case 7:
                            val = 0;
                            if (string.IsNullOrEmpty(valueString))
                            {
                                val = int.Parse(parameter) / Math.Pow(Op.Value, OpParam.Value);
                                valueString = GetValueString(val);
                            }
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"({valueString}% Per Character Level) {Math.Floor(val).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val * 99).ToString(CultureInfo.InvariantCulture)}% {lstValue} (Based on Character Level)";
                            break;
                        case 8:
                            val = 0;
                            if (string.IsNullOrEmpty(valueString))
                            {
                                val = int.Parse(parameter) / Math.Pow(Op.Value, OpParam.Value);
                                valueString = GetValueString(val);
                            }
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} (Based on Character Level)";
                            break;
                        case 9:
                            val = 0;
                            if (string.IsNullOrEmpty(valueString))
                            {
                                val = int.Parse(parameter) / Math.Pow(Op.Value, OpParam.Value);
                                valueString = GetValueString(val);
                            }
                            lstValue = DescriptonStringPositive;
                            DescriptionValue = 3;
                            valueString = $"{lstValue} {Math.Floor(val).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val * 99).ToString(CultureInfo.InvariantCulture)} ({valueString} Per Character Level)";
                            break;
                        case 10:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 11:
                            valueString = lstValue.Replace("%d", $"{((double)(Utility.ToNullableInt(parameter).Value / 100f)).ToString(CultureInfo.InvariantCulture)}");
                            DescriptionValue = 3;
                            break;
                        case 12:
                            valueString = $"+{valueString}";
                            break;
                        case 13:
                            valueString = $"+{valueString}";
                            break;
                        case 14:
                            valueString = lstValue.Replace("%d", valueString);
                            break;
                        case 15:
                            // TODO: Doesn't work when a skills has fx 13-20 range of levels. Value is 0 then.
                            valueString = lstValue.Replace("%d%", value.Value.ToString())
                                                     .Replace("%d", value2.Value.ToString())
                                                     .Replace("%s", Skill.GetSkill(parameter).Name);

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
                        case 17:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 18:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 19:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 20:
                            valueString = $"{GetValueString(value * -1, value2 * -1)}%";
                            break;
                        case 21:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 22:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 23:
                            valueString = $"{valueString}% {lstValue} {MonStat.MonStats[parameter].NameStr}";
                            DescriptionValue = 3;
                            break;
                        case 24:
                            valueString = $"Level {value2} {Skill.GetSkill(parameter).Name} ({value} Charges)";
                            break;
                        case 25:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
                        case 26:
                            // Nothing with this?
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
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
                            valueString = $"{lstValue} ({valueString})";
                            break;
                        default:
                            throw new Exception($"Property String not implemented for function '{DescriptionFunction.Value}' for stat '{Stat}'");
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

            valueString = valueString.Trim().Replace("\\n", "");
            return valueString;
        }

        private static string ReplaceValues(string description, string value = null, string string1 = null, string string2 = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                description = description.Replace("[value]", value);
            }

            if (!string.IsNullOrEmpty(string1))
            {
                description = description.Replace("[string1]", string1);
            }

            if (!string.IsNullOrEmpty(string2))
            {
                description = description.Replace("[string2]", string2);
            }


            return description;
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

        private static Dictionary<int, string> DescriptionFunctions = new Dictionary<int, string>
        {
            { 1, "+[value]"},
            { 2, "[value]%"},
            { 3, "[value]"},
            { 4, "+[value]%"},
            { 5, "[value*100/128]% [string1]"},
            { 6, "+[value] [string1] [string2]"},
            { 7, "[value]% [string1] [string2]"},
            { 8, "+[value]% [string1] [string2]"},
            { 9, "[value] [string1] [string2]"},
            { 10, "[value*100/128]% [string1] [string2]"},
            { 11, "Repairs 1 Durability In [100 / value] Seconds"},
            { 12, "+[value] [string1]"},
            { 13, "+[value] to [class] Skill Levels"},
            { 14, "+[value] to [skilltab] Skill Levels ([class] Only)"},
            { 15, "[chance]% to cast [slvl] [skill] on [event]"},
            { 16, "Level [sLvl] [skill] Aura When Equipped"},
            { 17, "[value] [string1] (Increases near [time])"},
            { 18, "[value]% [string1] (Increases near [time])"},
            { 19, "this is used by stats that use Blizzard's sprintf implementation (if you don't know what that is, it won't be of interest to you eitherway I guess), look at how prismatic is setup, the string is the format that gets passed to their sprintf spinoff."},
            { 20, "[value]% [string1]"},
            { 21, "[value] [string1]"},
            { 22, "[value]% [string1] [montype] (warning: this is bugged in vanilla and doesn't work properly, see CE forum)"},
            { 23, "[value]% [string1] [monster]"},
            { 24, "used for charges, we all know how that desc looks icon_wink.gif"},
            { 25, "not used by vanilla, present in the code but I didn't test it yet"},
            { 26, "not used by vanilla, present in the code but I didn't test it yet"},
            { 27, "+[value] to [skill] ([class] Only)"},
            { 28, "+[value] to [skill]"}
        };
    }
}
