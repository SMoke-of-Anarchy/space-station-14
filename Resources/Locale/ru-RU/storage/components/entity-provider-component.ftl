comp-entity-provider-cannot-receive = { CAPITALIZE(THE($refillTarget)) } нельзя пополнить!
comp-entity-provider-cannot-transfer = { CAPITALIZE(THE($provider)) } нельзя использовать для пополнения!
comp-entity-provider-eject-all-specified-entities = Извлечь все сущности: {$entity}.
comp-entity-provider-entity-listing =
    { $amount ->
        [one] [color=yellow]{ $amount }[/color] [color=gray]{ $name }[/color]
        [few] [color=yellow]{ $amount }[/color] [color=gray]{ $name }[/color]
        [many] [color=yellow]{ $amount }[/color] [color=gray]{ $name }[/color]
       *[other] [color=yellow]{ $amount }[/color] [color=gray]{ $name }[/color]
    }
comp-entity-provider-full = { CAPITALIZE(THE($provider)) } уже заполнен!
comp-entity-provider-has-entities = Внутри находится:
comp-entity-provider-no-ejected = Нечего извлекать!
comp-entity-provider-no-stored-entities = Пусто.
comp-entity-provider-select-entity = Выбрать сущность: {$entity}.
comp-entity-provider-select-new-active = Переключить выбранный сохранённый объект.
