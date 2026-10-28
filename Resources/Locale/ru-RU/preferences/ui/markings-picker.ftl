markings-used = Используемые черты

markings-unused = Неиспользуемые черты

markings-add = Добавить черту

markings-remove = Убрать черту

markings-rank-up = Вверх

markings-rank-down = Вниз

markings-search = Поиск

marking-points-remaining = Отметин осталось: { $points }

marking-used = { $marking-name }

marking-used-forced = { $marking-name } (Принудительно)

marking-slot-add = Добавить

marking-slot-remove = Удалить

marking-slot = Слот { $number }

# Categories

markings-category-Special = Специальное

markings-category-Hair = Причёска

markings-category-FacialHair = Лицевая растительность

markings-category-Head = Голова

markings-category-HeadTop = Голова (верх)

markings-category-HeadSide = Голова (бок)

markings-category-Snout = Морда

markings-category-SnoutCover = Морда (Внешний)

markings-category-UndergarmentTop = Нижнее бельё (Верх)

markings-category-UndergarmentBottom = Нижнее бельё (Низ)

markings-category-Chest = Грудь

markings-category-Arms = Руки

markings-category-Legs = Ноги

markings-category-Tail = Хвост

markings-category-Overlay = Наложение

-markings-selection = { $selectable ->
    [0] У вас не осталось доступных отметин.
    [one] Вы можете выбрать ещё { $selectable } отметину.
    [few] Вы можете выбрать ещё { $selectable } отметины.
    [many] Вы можете выбрать ещё { $selectable } отметин.
   *[other] Вы можете выбрать ещё { $selectable } отметины.
}

markings-limits = { $required ->
    [true] { $count ->
        [-1] Выберите хотя бы одну отметину.
        [0] Вы не можете выбрать ни одной отметины. Но каким-то образом должны? Это ошибка.
        [1] Выберите одну отметину.
        [one] Выберите хотя бы одну отметину и не более { $count } отметины. { -markings-selection(selectable: $selectable) }
        [few] Выберите хотя бы одну отметину и не более { $count } отметин. { -markings-selection(selectable: $selectable) }
        [many] Выберите хотя бы одну отметину и не более { $count } отметин. { -markings-selection(selectable: $selectable) }
       *[other] Выберите хотя бы одну отметину и не более { $count } отметины. { -markings-selection(selectable: $selectable) }
    }
   *[false] { $count ->
        [-1] Выберите любое количество отметин.
        [0] Вы не можете выбрать отметины.
        [1] Выберите не более одной отметины.
        [one] Выберите не более { $count } отметины. { -markings-selection(selectable: $selectable) }
        [few] Выберите не более { $count } отметин. { -markings-selection(selectable: $selectable) }
        [many] Выберите не более { $count } отметин. { -markings-selection(selectable: $selectable) }
       *[other] Выберите не более { $count } отметины. { -markings-selection(selectable: $selectable) }
    }
}

humanoid-marking-modifier-base-layers = Базовые слои

humanoid-marking-modifier-enable = Включить

humanoid-marking-modifier-prototype-id = ID прототипа:

humanoid-marking-modifier-respect-group-sex = Учитывать ограничения по группе и полу

humanoid-marking-modifier-respect-limits = Учитывать ограничения

markings-layer-Chest = Грудь

markings-layer-Eyes = Глаза

markings-layer-FacialHair = Растительность на лице

markings-layer-Hair = Волосы

markings-layer-Head = Голова

markings-layer-HeadSide = Голова (сбоку)

markings-layer-HeadTop = Голова (сверху)

markings-layer-LArm = Левая рука

markings-layer-LFoot = Левая ступня

markings-layer-LHand = Левая кисть

markings-layer-LLeg = Левая нога

markings-layer-OverEyes = Поверх глаз

markings-layer-Overlay = Наложение

markings-layer-RArm = Правая рука

markings-layer-RFoot = Правая ступня

markings-layer-RHand = Правая кисть

markings-layer-RLeg = Правая нога

markings-layer-Snout = Морда

markings-layer-SnoutCover = Морда (покрытие)

markings-layer-Special = Особое

markings-layer-Tail = Хвост

markings-layer-Tail-Moth = Крылья

markings-layer-TailOverlay = Наложение

markings-layer-UndergarmentBottom = Трусы

markings-layer-UndergarmentTop = Нижняя рубашка

markings-organ-ArmLeft = Левая рука

markings-organ-ArmRight = Правая рука

markings-organ-Eyes = Глаза

markings-organ-FootLeft = Левая ступня

markings-organ-FootRight = Правая ступня

markings-organ-HandLeft = Левая кисть

markings-organ-HandRight = Правая кисть

markings-organ-Head = Голова

markings-organ-LegLeft = Левая нога

markings-organ-LegRight = Правая нога

markings-organ-Torso = Торс

markings-reorder = Изменить порядок отметин
