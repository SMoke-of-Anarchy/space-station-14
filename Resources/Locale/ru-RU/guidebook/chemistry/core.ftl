guidebook-reagent-effect-description =
    { $quantity ->
        [0] {""}
       *[other] Если присутствует не менее {$quantity}u {$reagent},{" "}
    }{ $chance ->
        [1] { $effect }
       *[other] с вероятностью { NATURALPERCENT($chance, 2) } срабатывает эффект: { $effect }
    }{ $conditionCount ->
        [0] .
       *[other] {" "}если { $conditions }.
    }
guidebook-reagent-name = [bold][color={ $color }]{ CAPITALIZE($name) }[/color][/bold]
guidebook-reagent-recipes-header = Рецепт
guidebook-reagent-recipes-reagent-display = [bold]{ $reagent }[/bold] \[{ $ratio }\]
guidebook-reagent-sources-header = Источники
guidebook-reagent-sources-ent-wrapper = [bold]{ $name }[/bold] \[1\]
guidebook-reagent-sources-gas-wrapper = [bold]{ $name } (газ)[/bold] \[1\]
guidebook-reagent-effects-header = Эффекты
guidebook-reagent-effects-metabolism-group-rate = [bold]{ $group }[/bold] [color=gray]({ $rate } единиц в секунду)[/color]
guidebook-reagent-plant-metabolisms-header = Метаболизм растений
guidebook-reagent-plant-metabolisms-rate = [bold]Метаболизм растений[/bold] [color=gray](1 единица каждые 3 секунды базово)[/color]
guidebook-reagent-recipes-mix-info =
    { $minTemp ->
        [0]
            { $hasMax ->
                [true] { CAPITALIZE($verb) } ниже { $maxTemp }K
               *[false] { CAPITALIZE($verb) }
            }
       *[other]
            { CAPITALIZE($verb) } { $hasMax ->
                [true] между { $minTemp }K и { $maxTemp }K
               *[false] выше { $minTemp }K
            }
    }
guidebook-reagent-physical-description = [italic]На вид вещество { $description }.[/italic]

guidebook-reagent-effects-metabolism-stage-rate = [bold]{$stage}[/bold] [color=gray]({$rate} единиц в секунду)[/color]
guidebook-reagent-effects-metabolite-item = {$reagent} со скоростью { NATURALPERCENT($rate, 2) }
guidebook-reagent-effects-metabolites = Метаболизируется в { $items }.
