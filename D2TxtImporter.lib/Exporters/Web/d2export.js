var Uniques = {};
var AllUniques = {};

var Runewords = {};
var AllRunewords = {};

var CubeRecipes = {};
var AllCubeRecipes = {};

var Sets = {};
var AllSets = {};

function GetUniqueItemDiv(el) {
    var $item = $("<div>");
    $item.addClass("item");
    $item.addClass("row");

    // Name
    var $spanName = $("<span>");
    $spanName.text(el.Name);
    $spanName.addClass("unique-text");
    $spanName.addClass("col-sm-12");
    $item.append($spanName);

    // Type
    var $spanType = $("<span>");
    $spanType.text(el.Type);
    $spanType.addClass("unique-text");
    $spanType.addClass("col-sm-12");
    $item.append($spanType);

    // Damage Armor
    var $divDamage = $("<div>");
    $divDamage.addClass("col-sm-12");
    switch (el.Equipment.EquipmentType) {
        case 0: // Armor
            $divDamage.append($("<span>").text("Armor: "));
            $spanArmor = $("<span>");
            $spanArmor.text(el.Equipment.ArmorString);
            if (el.DamageArmorEnhanced) {
                $spanArmor.addClass("enhanced");
            }
            $divDamage.append($spanArmor);
            break;
        case 1: // Weapon
            for (var i = 0; i < el.Equipment.DamageTypes.length; i++) {
                var wep = el.Equipment.DamageTypes[i];
                $spanDamageString = $("<span>");
                $spanDamageNumbers = $("<span>");
                switch (wep.Type) {
                    case 0: // OneHanded
                        $spanDamageString.text("One-Hand Damage: ");
                        $spanDamageNumbers.text(wep.DamageString);
                        if (el.DamageArmorEnhanced) {
                            $spanDamageNumbers.addClass("enhanced");
                        }
                        break;
                    case 1: // TwoHanded
                        $spanDamageString.text("Two-Hand Damage: ");
                        $spanDamageNumbers.text(wep.DamageString);
                        if (el.DamageArmorEnhanced) {
                            $spanDamageNumbers.addClass("enhanced");
                        }
                        break;
                    case 2: // Thrown
                        $spanDamageString.text("Thrown Damage: ");
                        $spanDamageNumbers.text(wep.DamageString);
                        if (el.DamageArmorEnhanced) {
                            $spanDamageNumbers.addClass("enhanced");
                        }
                        break;
                    case 3: // Normal
                        $spanDamageString.text("Damage: ");
                        $spanDamageNumbers.text(wep.DamageString);
                        if (el.DamageArmorEnhanced) {
                            $spanDamageNumbers.addClass("enhanced");
                        }
                        break;
                }
                $divDamage.append($spanDamageString);
                $divDamage.append($spanDamageNumbers);
                $divDamage.append($("<br>"));
            };
            break;
        case 2: // Jewelery
            break;
    }
    $item.append($divDamage);

    // Required Level
    if (el.RequiredLevel > 0) {
        var $req = $("<span>");
        $req.addClass("col-sm-12");
        $req.addClass("requirement");
        $req.text("Required Level: " + el.RequiredLevel);
        $item.append($req);
    }

    // Required Strength
    if (el.Equipment.RequiredStrength > 0) {
        var $req = $("<span>");
        $req.addClass("requirement");
        $req.addClass("col-sm-12");
        $req.text("Required Strength: " + el.Equipment.RequiredStrength);
        $item.append($req);
    }

    // Required Dexterity
    if (el.Equipment.RequiredDexterity > 0) {
        var $req = $("<span>");
        $req.addClass("col-sm-12");
        $req.addClass("requirement");
        $req.text("Required Dexterity: " + el.Equipment.RequiredDexterity);
        $item.append($req);
    }

    // Required Class
    if (el.Equipment.RequiredClass !== "") {
        var $req = $("<span>");
        $req.addClass("col-sm-12");
        $req.addClass("requirement");
        $req.text(el.Equipment.RequiredClass + " Only");
        $item.append($req);
    }

    // Durability
    if (el.Equipment.Durability != 0) {
        var $req = $("<span>");
        $req.addClass("col-sm-12");
        $req.text("Durability: " + el.Equipment.Durability);
        $item.append($req);
    }

    // Add armor damage (smite damage, kick damage)
    if (el.Equipment.EquipmentType == 0) // Armor type
    {
        if (el.Equipment.DamageString) {
            var $dmgSpan = $("<span>");
            $dmgSpan.addClass("col-sm-12");
            $dmgSpan.text(el.Equipment.DamageStringPrefix + ": " + el.Equipment.DamageString);
            $item.append($dmgSpan);
        }
    }

    // Add properties
    for (var i = 0; i < el.Properties.length; i++) {
        var prop = el.Properties[i];
        $propSpan = $("<span>");
        $propSpan.addClass("col-sm-12");
        $propSpan.addClass("enhanced");
        $propSpan.text(prop.PropertyString);

        $item.append($propSpan);
    };

    return $item;
}

function RenderUniques() {
    ClearTable();

    for (var i = 0; i < Uniques.length; i++) {
        var el = Uniques[i];
        var $item = GetUniqueItemDiv(el);

        var $tr = $("<tr>");
        var $td = $("<td>");

        $td.append($item);
        $tr.append($td);
        $("#tbody").append($tr);
    };
}

function RenderRunewords() {
    ClearTable();

    for (var i = 0; i < Runewords.length; i++) {
        var el = Runewords[i];
        var $item = $("<div>");
        $item.addClass("item");

        // Name
        var $spanName = $("<span>");
        $spanName.text(el.Name);
        $spanName.addClass("unique-text");
        $item.append($spanName);
        $item.append($("<br>"));

        // Runes
        $divRunes = $("<div>");
        $divRunes.addClass("runes");
        for (var j = 0; j < el.Runes.length; j++) {
            var rune = el.Runes[j];
            var $spanRune = $("<span>").text(rune.Name.replace(" Rune", ""));
            $divRunes.append($spanRune);
        };
        $item.append($divRunes);

        // Types
        $divTypes = $("<div>");
        $divTypes.addClass("runeword-types");
        for (var j = 0; j < el.Types.length; j++) {
            var type = el.Types[j];
            var $spanType = $("<span>").text(type.Name);
            $divTypes.append($spanType);
        };
        $item.append($divTypes);

        // Required Level
        if (el.RequiredLevel > 0) {
            var $req = $("<span>");
            $req.addClass("requirement");
            $req.text("Required Level: " + el.RequiredLevel);
            $item.append($req);
            $item.append($("<br>"));
        }

        // Add properties
        for (var j = 0; j < el.Properties.length; j++) {
            var prop = el.Properties[j];
            $propSpan = $("<span>");
            $propSpan.addClass("enhanced");
            $propSpan.text(prop.PropertyString);

            $item.append($propSpan);
            $item.append($("<br>"));
        };

        var $tr = $("<tr>");
        var $td = $("<td>");

        $td.append($item);
        $tr.append($td);
        $("#tbody").append($tr);
    };
}

function RenderCubeRecipes() {
    ClearTable();

    for (var i = 0; i < CubeRecipes.length; i++) {
        var el = CubeRecipes[i];
        var $item = $("<div>");
        $item.addClass("item");

        // Name
        var $spanName = $("<span>");
        $spanName.text(el.Output);
        $spanName.addClass("unique-text");
        $item.append($spanName);
        $item.append($("<br>"));

        // Description
        $recipeSpan = $("<span>");
        $recipeSpan.text(el.CubeRecipeDescription);
        $item.append($recipeSpan);
        $item.append($("<br>"));

        var $tr = $("<tr>");
        var $td = $("<td>");

        $td.append($item);
        $tr.append($td);
        $("#tbody").append($tr);
    };
}

function RenderSets() {
    ClearTable();

    for (var i = 0; i < Sets.length; i++) {
        var set = Sets[i];

        var $item = $("<div>");
        $item.addClass("item");
        $item.addClass("row");

        // Name
        var $spanName = $("<span>");
        $spanName.text(set.Name);
        $spanName.addClass("set-text");
        $spanName.addClass("set-header");
        $spanName.addClass("col-sm-12");
        $item.append($spanName);

        $item.append($("<div>").addClass("col-sm-12").append($("<br>"))); // Extra line

        // Partial Properties
        for (var j = 0; j < set.PartialProperties.length; j++) {
            var prop = set.PartialProperties[j];

            $propSpan = $("<span>");
            $propSpan.addClass("set-text");
            $propSpan.addClass("col-sm-12");
            $propSpan.text(prop.PropertyString + " (" + Math.floor(prop.Index / 2 + 2) + " Items)");

            $item.append($propSpan);
        }

        // Full Properties
        for (var j = 0; j < set.FullProperties.length; j++) {
            var prop = set.FullProperties[j];

            $propSpan = $("<span>");
            $propSpan.addClass("set-text");
            $propSpan.addClass("col-sm-12");
            $propSpan.text(prop.PropertyString + " (Full Set)");

            $item.append($propSpan);
        }

        $item.append($("<div>").addClass("col-sm-12").append($("<br>"))); // Extra line

        // Set Items
        for (var j = 0; j < set.SetItems.length; j++) {
            var el = set.SetItems[j];

            var uniqueItemDiv = GetUniqueItemDiv(el);
            $item.append(uniqueItemDiv.children());
            for (var k = 0; k < el.SetPropertiesString.length; k++) {
                var propString = el.SetPropertiesString[k];

                var propSpan = $("<span>");
                propSpan.addClass("set-text");
                propSpan.addClass("col-sm-12");
                propSpan.text(propString);
                $item.append(propSpan);
            }
            $item.append($("<div>").addClass("col-sm-12").append($("<br>"))); // Extra line
        }

        var $tr = $("<tr>");
        var $td = $("<td>");

        $td.append($item);
        $tr.append($td);
        $("#tbody").append($tr);
    }
}

function ClearTable() {
    $("#tbody").html("");
}

var UniqueSearchItems = {};
var SetsSearchItems = {};
var RuneWordsSearchItems = {};

function GetUniqueSearch() {
    var types = [];
    for (var i = 0; i < AllUniques.length; i++) {
        var el = AllUniques[i];
        if (!ArrayContainsString(types, el.Equipment.Type.Name)) {
            types.push(el.Equipment.Type.Name);
        }
    };
    types.sort();
    types.unshift("Any");

    UniqueSearchItems = { "types": types };
}

function GetRunewordsSearch() {
    var types = [];
    for (var i = 0; i < AllRunewords.length; i++) {
        var el = AllRunewords[i];
        for (var j = 0; j < el.Types.length; j++) {
            var type = el.Types[j];
            if (!ArrayContainsString(types, type.Name)) {
                types.push(type.Name);
            }
        };
    };
    types.sort();
    types.unshift("Any");

    RuneWordsSearchItems = { "types": types };
}

function GetSetsSearch() {
    var sets = [];
    for (var i = 0; i < AllSets.length; i++) {
        el = AllSets[i];
        if (!ArrayContainsString(sets, el.Name)) {
            sets.push(el.Name);
        }
    }
    sets.sort();
    sets.unshift("Any");

    SetsSearchItems = { "sets": sets };
}

function HideSearchBars() {
    $("#search-runeword").css("display", "none");
    $("#search-unique").css("display", "none");
    $("#search-cuberecipes").css("display", "none");
    $("#search-sets").css("display", "none");
}

function GenerateSearchBarUniques() {
    HideSearchBars();
    var searchBar = $("#search-unique");
    searchBar.css("display", "flex");

    var typeDropdown = $("#search-unique-type");
    for (var i = 0; i < UniqueSearchItems.types.length; i++) {
        var type = UniqueSearchItems.types[i];
        var typeOption = $("<option>");
        typeOption.text(type);
        typeOption.val(type);
        typeDropdown.append(typeOption);
    };
}

function GenerateSearchBarRunewords() {
    HideSearchBars();
    var searchBar = $("#search-runeword");
    searchBar.css("display", "flex");

    var typeDropdown = $("#search-runeword-type");
    for (var i = 0; i < RuneWordsSearchItems.types.length; i++) {
        var type = RuneWordsSearchItems.types[i];
        var typeOption = $("<option>");
        typeOption.text(type);
        typeOption.val(type);
        typeDropdown.append(typeOption);
    };
}

function GenerateSearchBarCubeRecipes() {
    HideSearchBars();
}

function GenerateSearchBarSets() {
    HideSearchBars();
    var searchBar = $("#search-sets");
    searchBar.css("display", "flex");

    var setDropdown = $("#search-set-name");
    for (var i = 0; i < SetsSearchItems.sets.length; i++) {
        var set = SetsSearchItems.sets[i];

        var setOption = $("<option>");
        setOption.text(set);
        setOption.val(set);
        setDropdown.append(setOption);
    }
}

function GetUniques() {
    var json = "<UNIQUES_JSON>";
    Uniques = JSON.parse(json);
    AllUniques = JSON.parse(json);

    GetUniqueSearch();
}

function GetRunewords() {
    var json = "<RUNEWORDS_JSON>";
    Runewords = JSON.parse(json);
    AllRunewords = JSON.parse(json);

    GetRunewordsSearch();
}

function GetCubeRecipes() {
    var json = "<CUBE_RECIPES_JSON>";
    CubeRecipes = JSON.parse(json);
    AllCubeRecipes = JSON.parse(json);
}

function GetSets() {
    var json = "<SETS_JSON>";
    Sets = JSON.parse(json);
    AllSets = JSON.parse(json);

    GetSetsSearch();
}

GetUniques();
GetRunewords();
GetCubeRecipes();
GetSets();

function Search() {
    AddMissingArrayOptions();
    var text = $("#searchField")[0].value;
    var activeField = $(".nav-link.active")[0].id;

    var change = false;

    if (activeField === "nav-uniques") {
        var searchedUniques = [];
        var typeSearch = $("#search-unique-type").val();
        for (var i = 0; i < AllUniques.length; i++) {
            var el = AllUniques[i];
            if (typeSearch === null || typeSearch.toLowerCase() === "any" || el.Equipment.Type.Name === typeSearch) {
                if (text.length > 1) {
                    if (el.Name.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                        if (!ArrayContainsName(searchedUniques, el.Name)) {
                            searchedUniques.push(el);
                        }
                    } else if (el.Type.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                        if (!ArrayContainsName(searchedUniques, el.Name)) {
                            searchedUniques.push(el);
                        }
                    }

                    for (var j = 0; j < el.Properties.length; j++) {
                        var prop = el.Properties[j];
                        if (prop.PropertyString.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedUniques, el.Name)) {
                                searchedUniques.push(el);
                            }
                        }
                    };
                } else {
                    if (!ArrayContainsName(searchedUniques, el.Name)) {
                        searchedUniques.push(el);
                    }
                }
            }
        };

        if (!CompareItems(Uniques, searchedUniques)) {
            Uniques = searchedUniques;
            change = true;
        }
    } else if (activeField === "nav-runewords") {
        var searchedRunewords = [];
        change = true;
        var typeSearch = $("#search-runeword-type").val();
        for (var i = 0; i < AllRunewords.length; i++) {
            var el = AllRunewords[i];
            if (typeSearch === null || typeSearch.toLowerCase() === "any" || ArrayContainsName(el.Types, typeSearch)) {
                if (text.length > 1) {
                    if (el.Name.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                        if (!ArrayContainsName(searchedRunewords, el.Name)) {
                            searchedRunewords.push(el);
                        }
                    }

                    for (var j = 0; j < el.Types.length; j++) {
                        var type = el.Types[j];
                        if (type.Name.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedRunewords, el.Name)) {
                                searchedRunewords.push(el);
                            }
                        }
                    };

                    for (var j = 0; j < el.Properties.length; j++) {
                        var prop = el.Properties[j];
                        if (prop.PropertyString.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedRunewords, el.Name)) {
                                searchedRunewords.push(el);
                            }
                        }
                    };
                } else {
                    if (!ArrayContainsName(searchedRunewords, el.Name)) {
                        searchedRunewords.push(el);
                    }
                }
            }
        };

        if (!CompareItems(Runewords, searchedRunewords)) {
            Runewords = searchedRunewords;
            change = true;
        }
    } else if (activeField === "nav-cuberecipes") {
        change = true;
        CubeRecipes = [];
        for (var i = 0; i < AllCubeRecipes.length; i++) {
            var el = AllCubeRecipes[i];
            if (el.CubeRecipeDescription.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                CubeRecipes.push(el);
            }
        };
    } else if (activeField === "nav-sets") {
        var searchedSets = [];
        var nameSearch = $("#search-set-name").val();
        for (var i = 0; i < AllSets.length; i++) {
            var el = AllSets[i];
            if (nameSearch === null || nameSearch.toLowerCase() === "any" || el.Name === nameSearch) {
                if (text.length > 1) {
                    if (el.Name.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                        if (!ArrayContainsName(searchedSets, el.Name)) {
                            searchedSets.push(el);
                        }
                    }

                    for (var j = 0; j < el.PartialProperties.length; j++) {
                        var property = el.PartialProperties[j];
                        if (property.PropertyString.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedSets, el.Name)) {
                                searchedSets.push(el);
                            }
                        }
                    }

                    for (var j = 0; j < el.FullProperties.length; j++) {
                        var property = el.FullProperties[j];
                        if (property.PropertyString.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedSets, el.Name)) {
                                searchedSets.push(el);
                            }
                        }
                    }

                    for (var j = 0; j < el.SetItems.length; j++) {
                        var setItem = el.SetItems[j];
                        if (setItem.Name.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                            if (!ArrayContainsName(searchedSets, el.Name)) {
                                searchedSets.push(el);
                            }

                            if (setItem.Type.toLowerCase().indexOf(text.toLowerCase()) > -1) {
                                if (!ArrayContainsName(searchedSets, el.Name)) {
                                    searchedSets.push(el);
                                }
                            }
                        }
                    }
                } else {
                    if (!ArrayContainsName(searchedSets, el.Name)) {
                        searchedSets.push(el);
                    }
                }
            }
        }

        Sets = searchedSets;
        change = true;
    }

    if (change) {
        RenderActive();
    }
}

function ArrayContainsName(array, name) {
    for (var i = 0; i < array.length; i++) {
        if (array[i].Name === name) {
            return true;
        }
    }
    return false;
}

function ArrayContainsString(array, name) {
    for (var i = 0; i < array.length; i++) {
        if (array[i] === name) {
            return true;
        }
    }
    return false;
}

function CompareItems(arr1, arr2) {
    if (arr1.length !== arr2.length)
        return false;
    for (var i = arr1.length; i--;) {
        if (arr1[i].Name !== arr2[i].Name)
            return false;
    }
    return true;
}

function RenderActive() {
    var activeField = $(".nav-link.active")[0].id;

    if (activeField === "nav-uniques") {
        RenderUniques();
    } else if (activeField === "nav-runewords") {
        RenderRunewords();
    } else if (activeField === "nav-cuberecipes") {
        RenderCubeRecipes();
    } else if (activeField === "nav-sets") {
        RenderSets();
    }

}

function UpdateOnSearchChange() {
    $(".search-input-change").on('change paste input', function () {
        Search();
    });
}

function AddMissingArrayOptions() {
    if (!Array.prototype.some) {
        Object.defineProperty(Array.prototype, "some", {
            value: function (tester, that /*opt*/) {
                for (var i = 0, n = this.length; i < n; i++)
                    if (i in this && tester.call(that, this[i], i, this))
                        return true;
                return false;
            }
        });
    }
}

$('document').ready(function () {
    GenerateSearchBarUniques();
    RenderActive();

    $(".nav .nav-link").on("click", function () {
        $(".nav").find(".active").removeClass("active");
        $(this).addClass("active");
    });

    $("#nav-uniques").click(function () {
        $("#searchField")[0].value = "";
        GenerateSearchBarUniques();
        RenderActive();
    });

    $("#nav-runewords").click(function () {
        $("#searchField")[0].value = "";
        GenerateSearchBarRunewords();
        RenderActive();
    });

    $("#nav-cuberecipes").click(function () {
        $("#searchField")[0].value = "";
        GenerateSearchBarCubeRecipes();
        RenderActive();
    });

    $("#nav-sets").click(function () {
        $("#searchField")[0].value = "";
        GenerateSearchBarSets();
        RenderActive();
    });

    UpdateOnSearchChange();
});