-create-3rd-person =
    { $chance ->
        [1] Создаёт
       *[other] создают
    }

-cause-3rd-person =
    { $chance ->
        [1] Вызывает
       *[other] вызывают
    }

-satiate-3rd-person =
    { $chance ->
        [1] Насыщает
       *[other] насыщают
    }

entity-effect-guidebook-spawn-entity =
    { $chance ->
        [1] Создаёт
        *[other] создаёт
    } { $amount ->
        [1] сущность «{$entname}»
        [one] {$amount} сущность «{$entname}»
        [few] {$amount} сущности «{$entname}»
        [many] {$amount} сущностей «{$entname}»
       *[other] {$amount} сущности «{$entname}»
    }

entity-effect-guidebook-destroy =
    { $chance ->
        [1] Уничтожает
        *[other] уничтожает
    } объект

entity-effect-guidebook-break =
    { $chance ->
        [1] Ломает
        *[other] ломает
    } объект

entity-effect-guidebook-explosion =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } взрыв

entity-effect-guidebook-emp =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } электромагнитный импульс

entity-effect-guidebook-flash =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } ослепительную вспышку

entity-effect-guidebook-foam-area =
    { $chance ->
        [1] Создаёт
        *[other] создаёт
    } большое количество пены

entity-effect-guidebook-smoke-area =
    { $chance ->
        [1] Создаёт
        *[other] создаёт
    } большое количество дыма

entity-effect-guidebook-satiate =
    { $chance ->
        [1] Утоляет
        *[other] утоляет
    } { $relative ->
        [1] {$type} со средней скоростью
        *[other] {$type} со скоростью {NATURALFIXED($relative, 3)} от средней
    }

entity-effect-guidebook-health-change =
    { $chance ->
        [1] { $healsordeals ->
                [heals] Восстанавливает здоровье на
                [deals] Наносит урон в размере
                *[both] Изменяет здоровье на
             }
        *[other] { $healsordeals ->
                    [heals] восстанавливает здоровье на
                    [deals] наносит урон в размере
                    *[both] изменяет здоровье на
                 }
    } { $changes }

entity-effect-guidebook-even-health-change =
    { $chance ->
        [1] { $healsordeals ->
            [heals] Равномерно восстанавливает здоровье на
            [deals] Равномерно наносит урон в размере
            *[both] Равномерно изменяет здоровье на
        }
        *[other] { $healsordeals ->
            [heals] равномерно восстанавливает здоровье на
            [deals] равномерно наносит урон в размере
            *[both] равномерно изменяет здоровье на
        }
    } { $changes }

entity-effect-guidebook-status-effect-old =
    { $type ->
        [update]{ $chance ->
                    [1] Вызывает
                    *[other] вызывает
                 } {LOC($key)} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        [add]   { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } {LOC($key)} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} с накоплением
        [set]  { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } {LOC($key)} на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        *[remove]{ $chance ->
                    [1] Снимает
                    *[other] снимает
                } эффект {LOC($key)} длительностью {NATURALFIXED($time, 3)} {MANY("second", $time)}
    }

entity-effect-guidebook-status-effect =
    { $type ->
        [update]{ $chance ->
                    [1] Вызывает
                    *[other] вызывает
                 } {$key} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        [add]   { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } {$key} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} с накоплением
        [set]  { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } {$key} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        *[remove]{ $chance ->
                    [1] Снимает
                    *[other] снимает
                } эффект {$key} длительностью {NATURALFIXED($time, 3)} {MANY("second", $time)}
    } { $delay ->
        [0] немедленно
        *[other] с задержкой {NATURALFIXED($delay, 3)} с
    }

entity-effect-guidebook-status-effect-indef =
    { $type ->
        [update]{ $chance ->
                    [1] Вызывает
                    *[other] вызывает
                 } постоянный эффект {$key}
        [add]   { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } постоянный эффект {$key}
        [set]  { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } постоянный эффект {$key}
        *[remove]{ $chance ->
                    [1] Снимает
                    *[other] снимает
                } эффект {$key}
    } { $delay ->
        [0] немедленно
        *[other] с задержкой {NATURALFIXED($delay, 3)} с
    }

entity-effect-guidebook-knockdown =
    { $type ->
        [update]{ $chance ->
                    [1] Вызывает
                    *[other] вызывает
                    } {LOC($key)} минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        [add]   { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } опрокидывание минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} с накоплением
        *[set]  { $chance ->
                    [1] Вызывает
                    *[other] вызывает
                } опрокидывание минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)} без накопления
        [remove]{ $chance ->
                    [1] Снимает
                    *[other] снимает
                } опрокидывание длительностью {NATURALFIXED($time, 3)} {MANY("second", $time)}
    }

entity-effect-guidebook-set-solution-temperature-effect =
    { $chance ->
        [1] Устанавливает
        *[other] устанавливает
    } температуру раствора точно на уровне {NATURALFIXED($temperature, 2)}k

entity-effect-guidebook-adjust-solution-temperature-effect =
    { $chance ->
        [1] { $deltasign ->
                [1] Добавляет
                *[-1] Убирает
            }
        *[other]
            { $deltasign ->
                [1] добавляет
                *[-1] убирает
            }
    } тепло из раствора, пока его температура не достигнет { $deltasign ->
                [1] не выше {NATURALFIXED($maxtemp, 2)}k
                *[-1] не ниже {NATURALFIXED($mintemp, 2)}k
            }

entity-effect-guidebook-adjust-reagent-reagent =
    { $chance ->
        [1] { $deltasign ->
                [1] Добавляет
                *[-1] Удаляет
            }
        *[other]
            { $deltasign ->
                [1] добавляет
                *[-1] удаляет
            }
    } {NATURALFIXED($amount, 2)}u { $deltasign ->
        [1] реагента {$reagent} в раствор
        *[-1] реагента {$reagent} из раствора
    }

entity-effect-guidebook-adjust-reagent-group =
    { $chance ->
        [1] { $deltasign ->
                [1] Добавляет
                *[-1] Удаляет
            }
        *[other]
            { $deltasign ->
                [1] добавляет
                *[-1] удаляет
            }
    } {NATURALFIXED($amount, 2)}u { $deltasign ->
            [1] реагентов группы {$group} в раствор
            *[-1] реагентов группы {$group} из раствора
        }

entity-effect-guidebook-adjust-temperature =
    { $chance ->
        [1] { $deltasign ->
                [1] Добавляет
                *[-1] Отнимает
            }
        *[other]
            { $deltasign ->
                [1] добавляет
                *[-1] отнимает
            }
    } {POWERJOULES($amount)} тепла { $deltasign ->
            [1] телу, на которое действует эффект
            *[-1] у тела, на которое действует эффект
        }

entity-effect-guidebook-chem-cause-disease =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } заболевание { $disease }

entity-effect-guidebook-chem-cause-random-disease =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } заболевания { $diseases }

entity-effect-guidebook-jittering =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } дрожь

entity-effect-guidebook-clean-bloodstream =
    { $chance ->
        [1] Очищает
        *[other] очищает
    } кровь от других химических веществ

entity-effect-guidebook-cure-disease =
    { $chance ->
        [1] Излечивает
        *[other] излечивает
    } заболевания

entity-effect-guidebook-eye-damage =
    { $chance ->
        [1] { $deltasign ->
                [1] Наносит
                *[-1] Лечит
            }
        *[other]
            { $deltasign ->
                [1] наносит
                *[-1] лечит
            }
    } повреждение глаз

entity-effect-guidebook-vomit =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } рвоту

entity-effect-guidebook-create-gas =
    { $chance ->
        [1] Создаёт
        *[other] создаёт
    } { $moles } { $moles ->
        [one] моль
        [few] моля
        [many] молей
       *[other] моля
    } газа {$gas}

entity-effect-guidebook-drunk =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } опьянение

entity-effect-guidebook-electrocute =
    { $chance ->
        [1] { $stuns ->
            [true] Бьёт током
            *[false] Ударяет током
            }
        *[other] { $stuns ->
            [true] бьёт током
            *[false] ударяет током
            }
    } метаболизатор в течение {NATURALFIXED($time, 3)} {MANY("second", $time)}

entity-effect-guidebook-emote =
    { $chance ->
        [1] Принудительно заставляет
        *[other] принудительно заставляет
    } метаболизатор издать [bold][color=white]{$emote}[/color][/bold]

entity-effect-guidebook-extinguish-reaction =
    { $chance ->
        [1] Тушит
        *[other] тушит
    } огонь

entity-effect-guidebook-flammable-reaction =
    { $chance ->
        [1] Повышает
        *[other] повышает
    } горючесть

entity-effect-guidebook-ignite =
    { $chance ->
        [1] Поджигает
        *[other] поджигает
    } метаболизатора

entity-effect-guidebook-make-sentient =
    { $chance ->
        [1] Делает
        *[other] делает
    } метаболизатор разумным

entity-effect-guidebook-make-polymorph =
    { $chance ->
        [1] Превращает
        *[other] превращает
    } метаболизатор в {$entityname}

entity-effect-guidebook-modify-bleed-amount =
    { $chance ->
        [1] { $deltasign ->
                [1] Вызывает
                *[-1] Уменьшает
            }
        *[other] { $deltasign ->
                    [1] вызывает
                    *[-1] уменьшает
                 }
    } кровотечение

entity-effect-guidebook-modify-blood-level =
    { $chance ->
        [1] { $deltasign ->
                [1] Повышает
                *[-1] Понижает
            }
        *[other] { $deltasign ->
                    [1] повышает
                    *[-1] понижает
                 }
    } уровень крови

entity-effect-guidebook-paralyze =
    { $chance ->
        [1] Парализует
        *[other] парализует
    } метаболизатора минимум на {NATURALFIXED($time, 3)} {MANY("second", $time)}

entity-effect-guidebook-movespeed-modifier =
    { $chance ->
        [1] Изменяет
        *[other] изменяет
    } скорость передвижения на множитель {NATURALFIXED($sprintspeed, 3)}x на срок не менее {NATURALFIXED($time, 3)} с

entity-effect-guidebook-reset-narcolepsy =
    { $chance ->
        [1] Временно избавляет
        *[other] временно избавляет
    } от нарколепсии

entity-effect-guidebook-wash-cream-pie-reaction =
    { $chance ->
        [1] Смывает
        *[other] смывает
    } кремовый пирог с лица

entity-effect-guidebook-cure-zombie-infection =
    { $chance ->
        [1] Излечивает
        *[other] излечивает
    } текущую зомби-инфекцию

entity-effect-guidebook-cause-zombie-infection =
    { $chance ->
        [1] Заражает
        *[other] заражает
    } человека зомби-инфекцией

entity-effect-guidebook-innoculate-zombie-infection =
    { $chance ->
        [1] Излечивает
        *[other] излечивает
    } текущую зомби-инфекцию и обеспечивает иммунитет к будущим заражениям

entity-effect-guidebook-reduce-rotting =
    { $chance ->
        [1] Восстанавливает
        *[other] восстанавливает
    } {NATURALFIXED($time, 3)} {MANY("second", $time)} разложения

entity-effect-guidebook-area-reaction =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } реакцию дыма или пены на {NATURALFIXED($duration, 3)} {MANY("second", $duration)}

entity-effect-guidebook-add-to-solution-reaction =
    { $chance ->
        [1] Вызывает
        *[other] вызывает
    } добавление {$reagent} во внутренний контейнер раствора

entity-effect-guidebook-artifact-unlock =
    { $chance ->
        [1] Помогает
        *[other] помогает
        } открыть инопланетный артефакт.

entity-effect-guidebook-artifact-durability-restore = Восстанавливает {$restored} единиц прочности активным узлам инопланетного артефакта.

entity-effect-guidebook-plant-attribute =
    { $chance ->
        [1] Изменяет
        *[other] изменяет
    } характеристику {$attribute} на {$positive ->
    [false] [color=red]{$amount}[/color]
    *[true] [color=green]{$amount}[/color]
    }

entity-effect-guidebook-plant-cryoxadone =
    { $chance ->
        [1] Омолаживает
        *[other] омолаживает
    } растение в зависимости от его возраста и времени роста

entity-effect-guidebook-plant-phalanximine =
    { $chance ->
        [1] Восстанавливает
        *[other] восстанавливает
    } жизнеспособность растения, утратившего её из-за мутации

entity-effect-guidebook-plant-diethylamine =
    { $chance ->
        [1] Увеличивает
        *[other] увеличивает
    } продолжительность жизни растения и/или его базовое здоровье с вероятностью 10% отдельно для каждого параметра

entity-effect-guidebook-plant-robust-harvest =
    { $chance ->
        [1] Увеличивает
        *[other] увеличивает
    } потенцию растения на {$increase}, но не выше {$limit}. Когда потенция достигнет {$seedlesstreshold}, растение потеряет семена. Попытка увеличить потенцию выше {$limit} может снизить урожайность с вероятностью 10%.

entity-effect-guidebook-plant-seeds-add =
    { $chance ->
        [1] Восстанавливает
        *[other] восстанавливает
    } семена растения

entity-effect-guidebook-plant-seeds-remove =
    { $chance ->
        [1] Удаляет
        *[other] удаляет
    } семена растения

entity-effect-guidebook-plant-change-trait =
    { $chance ->
        [1] { $change ->
            [Add] добавляет растению признак {$trait}
            [Remove] удаляет у растения признак {$trait}
            [Toggle] переключает состояние признака {$trait} у растения
            *[other] включает у растения признак {$trait}
        }
        *[other] { $change ->
            [Add] добавляет растению признак {$trait}
            [Remove] удаляет у растения признак {$trait}
            [Toggle] переключает состояние признака {$trait} у растения
            *[other] включает у растения признак {$trait}
        }
    }

entity-effect-guidebook-plant-mutate-exude-gasses =
    { $chance ->
        [1] Мутирует
        *[other] мутирует
    } растение, заставляя его выделять от {$minValue} до {$maxValue} моль газа

entity-effect-guidebook-plant-mutate-consume-gasses =
    { $chance ->
        [1] Мутирует
        *[other] мутирует
    } растение, заставляя его потреблять от {$minValue} до {$maxValue} моль газа

entity-effect-guidebook-plant-mutate-chemicals =
    { $chance ->
        [1] Мутирует
        *[other] мутирует
    } растение, чтобы оно производило {$name}

entity-effect-guidebook-add-reagent-to-bloodstream =
    { $chance ->
        [1] Вводит
        *[other] вводит
    } {$quantity} реагента {$reagent} непосредственно в кровоток

entity-effect-disarm =
    { $chance ->
        [1] Разоружает
        *[other] разоружает
    } сущность

entity-effect-guidebook-make-trap-in-container =
    { $chance ->
        [1] Запирает
        *[other] запирает
    } метаболизатор внутри объекта {$entityname}
