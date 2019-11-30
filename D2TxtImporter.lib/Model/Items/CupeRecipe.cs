using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class CubeRecipe
    {
        public string Description { get; set; }
        public string Item { get; set; }
        public List<string> InputList { get; set; }
        public string Output { get; set; }

        public static List<CubeRecipe> Import(string excelFolder)
        {
            var result = new List<CubeRecipe>();

            var lines = Importer.ReadCsvFile(excelFolder + "/CubeMain.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]) || values[1] == "0")
                {
                    continue;
                }

                // Params
                var paramArray = values.Skip(21).ToList().Take(25).ToList();
                var modifiers = new List<CubeMod>();
                for (int i = 0; i < paramArray.Count; i += 5)
                {
                    if (!string.IsNullOrEmpty(paramArray[i]))
                    {
                        modifiers.Add(new CubeMod
                        {
                            Mod = paramArray[i],
                            ModChance = paramArray[i + 1],
                            ModParam = paramArray[i + 2],
                            ModMin = paramArray[i + 3],
                            ModMax = paramArray[i + 4],
                        });
                    }
                }

                // Input
                var numInputs = int.Parse(values[9]);
                var inputArray = values.Skip(10).ToList();
                inputArray = inputArray.Take(numInputs).ToList();
                inputArray.RemoveAll(x => string.IsNullOrEmpty(x));

                var recipe = new CubeRecipe(inputArray.ToArray(), values[17]);

                if (recipe.Output.Contains("usetype"))
                {
                    var type = inputArray[0].Replace("\"", "").Split(',')[0];
                    if (ItemType.ItemTypes.Keys.Contains(type))
                    {
                        recipe.Output = recipe.Output.Replace("usetype", ItemType.ItemTypes[type].Name);
                    }
                    else
                    {
                        if (type == "any")
                        {
                            recipe.Output = recipe.Output.Replace("usetype", "item");
                        }
                        else
                        {
                            recipe.Output = recipe.Output.Replace("usetype", recipe.InputList[0]);
                        }
                    }
                }

                if (recipe.Output.Contains("useitem"))
                {
                    var item = inputArray[0].Replace("\"", "").Split(',')[0];
                    if (ItemType.ItemTypes.Keys.Contains(item))
                    {
                        recipe.Output = recipe.Output.Replace("useitem", ItemType.ItemTypes[item].Name);
                    }

                    if (modifiers.Count > 0 && modifiers[0].Mod == "sock")
                    {
                        recipe.Output = $"Socketed {recipe.Output}";
                    }
                }

                var descr = values[0].Replace("rune ", "r");
                var matches = Regex.Matches(descr, @"(r\d\d)");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var rune = Misc.MiscItems[match.Groups[1].Value];

                        descr = descr.Replace(rune.Code, rune.Name);
                    }
                }

                recipe.Description = descr;

                result.Add(recipe);
            }

            return result;
        }


        public override string ToString()
        {
            return Description;
        }

        public CubeRecipe(string[] inputArray, string output)
        {
            InputList = new List<string>();
            foreach (var input in inputArray)
            {
                var inputParams = input.Replace("\"", "").Split(',');
                var item = ReplaceItemName(inputParams[0]);

                var parameters = inputParams.Skip(1).ToArray();
                var parameterString = GetParameterString(parameters);

                var valueResult = parameterString.Replace("%d", item);

                InputList.Add(valueResult);
            }

            Output = GetOutput(output);
        }

        private static string GetOutput(string output)
        {
            var outputParams = output.Replace("\"", "").Split(',');
            var item = ReplaceItemName(outputParams[0]);

            var parameters = outputParams.Skip(1).ToArray();
            var parameterString = GetParameterString(parameters);

            var valueResult = parameterString.Replace("%d", item);

            return valueResult;
        }

        public static string ReplaceItemName(string item)
        {
            if (ItemType.ItemTypes.ContainsKey(item))
            {
                return ItemType.ItemTypes[item].Name;
            }
            else if (Misc.MiscItems.ContainsKey(item))
            {
                return Misc.MiscItems[item].Name;
            }
            else if (Misc.MiscItems.Values.Any(x => x.Type == item))
            {
                return Misc.MiscItems.Values.First(x => x.Type == item).Name;
            }
            else if (Misc.MiscItems.Values.Any(x => x.Type2 == item))
            {
                return Misc.MiscItems.Values.First(x => x.Type2 == item).Name;
            }
            else if (Weapon.Weapons.ContainsKey(item))
            {
                return Weapon.Weapons[item].Name;
            }
            else if (Armor.Armors.ContainsKey(item))
            {
                return Armor.Armors[item].Name;
            }

            return item;
        }

        public static string GetParameterString(string[] parameters)
        {
            var result = "%d";

            foreach (var parameter in parameters)
            {

                switch (parameter)
                {
                    case "low":
                        result = result.Replace("%d", "Low Quality %d");
                        break;
                    case "hig":
                        result = result.Replace("%d", "High Quality %d");
                        break;

                    case "nor":
                        result = result.Replace("%d", "Normal %d");
                        break;
                    case "mag":
                        // If the magic item has a sufix, don't add magic to it
                        if (parameters.Any(x => x.StartsWith("pre=") || x.StartsWith("suf=")))
                        {
                            continue;
                        }
                        result = result.Replace("%d", "Magic %d");
                        break;
                    case "rar":
                        result = result.Replace("%d", "Rare %d");
                        break;
                    case "set":
                        result = result.Replace("%d", "Set %d");
                        break;
                    case "uni":
                        result = result.Replace("%d", "Unique %d");
                        break;
                    case "orf":
                        result = result.Replace("%d", "Crafted %d");
                        break;
                    case "tmp":
                        result = result.Replace("%d", "Tempered %d");
                        break;

                    case "eth":
                        result = result.Replace("%d", "Etheral %d");
                        break;
                    case "noe":
                        result = result.Replace("%d", "Not Etheral %d");
                        break;

                    case "nos":
                        result = result.Replace("%d", "No Socket %d");
                        break;

                    case "pre": // todo
                        break;
                    case "suf": // todo
                        break;

                    case "rep":
                        result = result.Replace("%d", "%d Repair durability");
                        break;
                    case "rch":
                        result = result.Replace("%d", "%d Recharge Quantity");
                        break;

                    case "upg": // Include?
                        //result = result.Replace("%d", "%d upgraded");
                        break;
                    case "bas":
                        result = result.Replace("%d", "basic %d");
                        break;
                    case "exc":
                        result = result.Replace("%d", "exceptional %d");
                        break;
                    case "eli":
                        result = result.Replace("%d", "elite %d");
                        break;

                    case "sock":
                        result = result.Replace("%d", "Socketed %d");
                        break;

                    case "uns":
                        result = result.Replace("%d", "Destroy gems %d");
                        break;
                    case "rem":
                        result = result.Replace("%d", "Remove gems %d");
                        break;

                    case "req":
                        result = result.Replace("%d", "reroll item %d");
                        break;

                    default:
                        break;

                }

                if (parameter.StartsWith("qty="))
                {
                    var quantity = parameter.Replace("qty=", "");
                    result = result.Replace(result, $"{quantity}x {result}");
                    continue;
                }
                else if (parameter.StartsWith("sock="))
                {
                    var quantity = parameter.Replace("sock=", "");
                    result = result.Replace("%d", $"{quantity} Sockets");
                    continue;
                }
                else if (parameter.StartsWith("pre="))
                {
                    var index = int.Parse(parameter.Replace("pre=", ""));
                    result = result.Replace("%d", $"{MagicPrefix.MagicPrefixes[index].Name} %d");
                    continue;
                }
                else if (parameter.StartsWith("suf="))
                {
                    var index = int.Parse(parameter.Replace("suf=", ""));
                    result = result.Replace("%d", $"%d {MagicSuffix.MagicSuffixes[index].Name}");
                    continue;
                }
            }

            return result;
        }
    }

    public class CubeMod
    {
        public string Mod { get; set; }
        public string ModChance { get; set; }
        public string ModParam { get; set; }
        public string ModMin { get; set; }
        public string ModMax { get; set; }
    }
}
