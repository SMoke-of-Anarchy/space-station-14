lathe-menu-title = Меню станка

lathe-menu-queue = Очередь

lathe-menu-server-list = Список серверов

lathe-menu-sync = Синхр.

lathe-menu-search-designs = Поиск проектов

lathe-menu-category-all = Всё

lathe-menu-search-filter = Фильтр

lathe-menu-amount = Кол-во:

lathe-menu-recipe-count =
    { $count ->
        [one] { $count } рецепт
        [few] { $count } рецепта
        [many] { $count } рецептов
       *[other] { $count } рецепта
    }

lathe-menu-reagent-slot-examine = Сбоку имеется отверстие для мензурки.

lathe-reagent-dispense-no-container = Жидкость выливается из { $name } на пол!

lathe-menu-result-reagent-display = { $reagent } ({ $amount } ед.)

lathe-menu-material-display = { $material } { $amount }

lathe-menu-tooltip-display = { $amount } { $material }

lathe-menu-description-display = [italic]{ $description }[/italic]

lathe-menu-material-amount = { $unit }: { NATURALFIXED($amount, 2) }

lathe-menu-material-amount-missing = { $material } ({ $unit }): { NATURALFIXED($amount, 2) } ([color=red]не хватает: { NATURALFIXED($missingAmount, 2) }[/color])

lathe-menu-no-materials-message = Материалы не загружены

lathe-menu-silo-linked-message = Хранилище связано

lathe-menu-fabricating-message = Производится...

lathe-menu-materials-title = Материалы

lathe-menu-queue-title = Очередь производства

lathe-menu-delete-fabricating-tooltip = Отменить производство текущего объекта.

lathe-menu-delete-item-tooltip = Отменить производство этой партии.

lathe-menu-move-up-tooltip = Перенести эту партию вперёд в очереди.

lathe-menu-move-down-tooltip = Перенести эту партию назад в очереди.

lathe-menu-item-single = { $index }. { $name }

lathe-menu-item-batch = { $index }. { $name } ({ $printed }/{ $total })
