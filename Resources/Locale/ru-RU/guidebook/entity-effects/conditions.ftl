entity-condition-guidebook-total-damage =
    { $max ->
        [2147483648] общий урон существа не менее { NATURALFIXED($min, 2) }
       *[other] { $min ->
            [0] общий урон существа не более { NATURALFIXED($max, 2) }
           *[other] общий урон существа от { NATURALFIXED($min, 2) } до { NATURALFIXED($max, 2) }
        }
    }

entity-condition-guidebook-type-damage =
    { $max ->
        [2147483648] урон типа {$type} не менее { NATURALFIXED($min, 2) }
       *[other] { $min ->
            [0] урон типа {$type} не более { NATURALFIXED($max, 2) }
           *[other] урон типа {$type} от { NATURALFIXED($min, 2) } до { NATURALFIXED($max, 2) }
        }
    }

entity-condition-guidebook-group-damage =
    { $max ->
        [2147483648] урон группы {$type} не менее { NATURALFIXED($min, 2) }.
       *[other] { $min ->
            [0] урон группы {$type} не более { NATURALFIXED($max, 2) }.
           *[other] урон группы {$type} от { NATURALFIXED($min, 2) } до { NATURALFIXED($max, 2) }
        }
    }

entity-condition-guidebook-total-satiation =
    { $max ->
        [2147483648] суммарное значение {$type} у цели не менее { NATURALFIXED($min, 2) }
       *[other] { $min ->
            [0] суммарное значение {$type} у цели не более { NATURALFIXED($max, 2) }
           *[other] суммарное значение {$type} у цели от { NATURALFIXED($min, 2) } до { NATURALFIXED($max, 2) }
        }
    }

entity-condition-guidebook-reagent-threshold =
    { $max ->
        [2147483648] реагента {$reagent} не менее { NATURALFIXED($min, 2) } ед.
       *[other] { $min ->
            [0] реагента {$reagent} не более { NATURALFIXED($max, 2) } ед.
           *[other] реагента {$reagent} от { NATURALFIXED($min, 2) } до { NATURALFIXED($max, 2) } ед.
        }
    }

entity-condition-guidebook-mob-state-condition =
    состояние существа: { $state }

entity-condition-guidebook-job-condition =
    должность цели: { $job }

entity-condition-guidebook-solution-temperature =
    температура раствора { $max ->
        [2147483648] не ниже { NATURALFIXED($min, 2) } K
       *[other] { $min ->
            [0] не выше { NATURALFIXED($max, 2) } K
           *[other] от { NATURALFIXED($min, 2) } K до { NATURALFIXED($max, 2) } K
        }
    }

entity-condition-guidebook-body-temperature =
    температура тела { $max ->
        [2147483648] не ниже { NATURALFIXED($min, 2) } K
       *[other] { $min ->
            [0] не выше { NATURALFIXED($max, 2) } K
           *[other] от { NATURALFIXED($min, 2) } K до { NATURALFIXED($max, 2) } K
        }
    }

entity-condition-guidebook-organ-type =
    { $shouldhave ->
        [true] у метаболизатора есть орган типа {$name}
       *[false] у метаболизатора нет органа типа {$name}
    }

entity-condition-guidebook-has-tag =
    { $invert ->
        [true] у цели нет тега {$tag}
       *[false] у цели есть тег {$tag}
    }

entity-condition-guidebook-this-reagent = этот реагент

entity-condition-guidebook-breathing =
    { $isBreathing ->
        [true] метаболизатор нормально дышит
       *[false] метаболизатор задыхается
    }

entity-condition-guidebook-internals =
    { $usingInternals ->
        [true] метаболизатор использует дыхательные аппараты
       *[false] метаболизатор дышит атмосферным воздухом
    }

entity-condition-guidebook-mouth-uncovered-condition = у существа открыт рот

entity-condition-guidebook-bloodlevel =
    { $max ->
        [2147483648] общий объём крови у цели не менее { NATURALFIXED($min, 2) }%
       *[other] { $min ->
            [0] общий объём крови у цели не более { NATURALFIXED($max, 2) }%
           *[other] общий объём крови у цели от { NATURALFIXED($min, 2) }% до { NATURALFIXED($max, 2) }%
        }
    }
