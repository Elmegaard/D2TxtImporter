var Uniques = {};
var Runewords = {};
var CubeRecipes = {};

function RenderUniques() {
    Uniques.forEach(el => {
        var $tr = $("<tr>");
        var $td = $("<td>");

        // Name
        var $spanName = $("<span>");
        $spanName.text(el.Name);
        $spanName.addClass("unique-text");
        $td.append($spanName);
        $td.append($("<br>"));

        // Type
        var $spanType = $("<span>");
        $spanType.text(el.Type);
        $spanType.addClass("unique-text");
        $td.append($spanType);
        $td.append($("<br>"));

        // Damage Armor
        var $divDamage = $("<div>");
        switch (el.Equipment.EquipmentType) {
            case 0: // Armor
                $divDamage.append($("<span>").text("Armor: "));
                $spanArmor = $("<span>");
                $spanArmor.text(el.Equipment.ArmorString);
                if (el.DamageArmorEnhanced) {
                    $spanArmor.addClass("enhanced");
                }
                $divDamage.append($spanArmor);
                $divDamage.append($("<br>"));
                break;
            case 1: // Weapon
                el.Equipment.DamageTypes.forEach(wep => {
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
                });
                break;
            case 2: // Jewelery
                break;
        }
        $td.append($divDamage);

        // Required Level
        if (el.RequiredLevel > 0) {
            var $req = $("<span>");
            $req.addClass("requirement");
            $req.text("Required Level: " + el.RequiredLevel);
            $td.append($req);
            $td.append($("<br>"));
        }

        // Required Strength
        if (el.Equipment.RequiredStrength > 0) {
            var $req = $("<span>");
            $req.addClass("requirement");
            $req.text("Required Strength: " + el.Equipment.RequiredStrength);
            $td.append($req);
            $td.append($("<br>"));
        }

        // Required Dexterity
        if (el.Equipment.RequiredDexterity > 0) {
            var $req = $("<span>");
            $req.addClass("requirement");
            $req.text("Required Dexterity: " + el.Equipment.RequiredDexterity);
            $td.append($req);
            $td.append($("<br>"));
        }

        // Durability
        if (el.Equipment.Durability != 0) {
            var $req = $("<span>");
            $req.text("Durability: " + el.Equipment.Durability);
            $td.append($req);
            $td.append($("<br>"));
        }

        // Add armor damage (smite damage, kick damage)
        if (el.Equipment.EquipmentType == 0) // Armor type
        {
            if (el.Equipment.DamageString) {
                $td.append($("<span>").text(el.Equipment.DamageStringPrefix + ": " + el.Equipment.DamageString));
                $td.append($("<br>"));
            }
        }

        // Add properties
        el.Properties.forEach(prop => {
            $propSpan = $("<span>");
            $propSpan.addClass("enhanced");
            $propSpan.text(prop.PropertyString);

            $td.append($propSpan);
            $td.append($("<br>"));
        });

        $tr.append($td);
        $("#tbody-unique").append($tr);
    });
}

function GetUniques() {
    var json = "<UNIQUES_JSON>";
    Uniques = JSON.parse(json);
}

function GetRunewords() {
    var json = "<RUNEWORDS_JSON>";
    Runewords = JSON.parse(json);
}

function GetCubeRecipes() {
    var json = "<CUBE_RECIPES_JSON>";
    CubeRecipes = JSON.parse(json);
}


GetUniques();
GetRunewords();
GetCubeRecipes();

$('document').ready(function () {
    RenderUniques();
});