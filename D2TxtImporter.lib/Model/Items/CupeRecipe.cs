using Newtonsoft.Json;
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
        public static bool UseDescription { get; set; }
        public string Description { get; set; }
        public string Item { get; set; }
        [JsonIgnore]
        public List<string> InputList { get; set; }
        public string Output { get; set; }
        public string Input { get; set; }
        public string CubeRecipeDescription { get; set; }

        public static List<CubeRecipe> Import(string excelFolder)
        {
            var result = new List<CubeRecipe>();

            var lines = Importer.ReadTxtFileToDictionaryList(excelFolder + "/CubeMain.txt");

            foreach (var row in lines)
            {
                if (row["enabled"] == "0")
                {
                    continue;
                }

                var recipe = new CubeRecipe();

                var descr = row["description"].Replace("rune ", "r");


                if (!UseDescription)
                {
                    // Params
                    var modifiers = new List<CubeMod>();
                    for (int i = 1; i <= 5; i++)
                    {
                        if (!string.IsNullOrEmpty(row[$"mod {i}"]))
                        {
                            modifiers.Add(new CubeMod
                            {
                                Mod = row[$"mod {i}"],
                                ModChance = row[$"mod {i} chance"],
                                ModParam = row[$"mod {i} param"],
                                ModMin = row[$"mod {i} min"],
                                ModMax = row[$"mod {i} max"]
                            });
                        }
                    }

                    // Input
                    var numInputs = Utility.ToNullableInt(row["numinputs"]);
                    if (!numInputs.HasValue)
                    {
                        ExceptionHandler.LogException(new Exception($"Cube recipe '{descr}' does not have a numinputs"));
                    }

                    var inputArray = new List<string>();
                    for (int i = 1; i <= numInputs.Value; i++)
                    {
                        inputArray.Add(row[$"input {i}"]);
                    }
                    inputArray.RemoveAll(x => string.IsNullOrEmpty(x));

                    recipe = new CubeRecipe(inputArray.ToArray(), row["output"]);

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
                        else
                        {
                            recipe.Output = recipe.Output.Replace("useitem", "");
                        }

                        if (modifiers.Count > 0 && modifiers[0].Mod == "sock")
                        {
                            recipe.Output = $"Socketed {recipe.Output}";
                        }
                    }

                    recipe.Input = "";
                    foreach (var input in recipe.InputList)
                    {
                        recipe.Input += $"{input} + ";
                    }
                    recipe.Input = recipe.Input.Substring(0, recipe.Input.LastIndexOf('+'));
                }

                var matches = Regex.Matches(descr, @"(r\d\d)");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        if (Misc.MiscItems.ContainsKey(match.Groups[1].Value))
                        {
                            var rune = Misc.MiscItems[match.Groups[1].Value];
                            descr = descr.Replace(rune.Code, rune.Name);
                        }
                    }
                }

                recipe.Description = descr;

                if (UseDescription)
                {
                    recipe.CubeRecipeDescription = descr;
                }
                else
                {
                    recipe.CubeRecipeDescription = $"{recipe.Input}= {recipe.Output}";
                }

                recipe.CubeRecipeDescription = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(recipe.CubeRecipeDescription);

                result.Add(recipe);
            }

            return result;
        }


        public override string ToString()
        {
            return Description;
        }

        public CubeRecipe()
        {

        }

        public CubeRecipe(string[] inputArray, string output)
        {
            InputList = new List<string>();
            foreach (var input in inputArray)
            {
                var inputParams = input.Replace("\"", "").Split(',');
                var item = ReplaceItemName(inputParams[0]);

                if (Table.Tables.ContainsKey(item))
                {
                    item = Table.GetValue(item);
                }

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

            if (Table.Tables.ContainsKey(item))
            {
                item = Table.GetValue(item);
            }

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
            else if (Misc.MiscItems.Values.Any(x => x.Type.Code == item))
            {
                return Misc.MiscItems.Values.First(x => x.Type.Code == item).Name;
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
                    result = result.Replace(result, $"{quantity} {result}");
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
