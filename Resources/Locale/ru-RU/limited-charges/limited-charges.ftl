limited-charges-charges-remaining =
    Имеется [color=fuchsia]{ $charges }[/color] { $charges ->
        [one] заряд
        [few] заряда
        [many] зарядов
       *[other] заряда
    }.

limited-charges-max-charges = Имеет [color=green]максимум[/color] зарядов.

limited-charges-recharging =
    До нового заряда { $seconds ->
        [one] осталась [color=yellow]{ $seconds }[/color] секунда.
        [few] осталось [color=yellow]{ $seconds }[/color] секунды.
        [many] осталось [color=yellow]{ $seconds }[/color] секунд.
       *[other] осталось [color=yellow]{ $seconds }[/color] секунды.
    }

limited-charges-no-charges = Заряды закончились!
