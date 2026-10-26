genpop-prisoner-id-expire = Вы отбыли наказание! Теперь вы можете выйти из тюрьмы через турникеты и забрать свои вещи.
genpop-prisoner-id-popup-not-served = Наказание ещё не отбыто!
genpop-prisoner-id-crime-default = [Редактировать]
genpop-prisoner-id-examine-wait =
    Вы отбыли { $minutes } { $minutes ->
        [one] минуту
        [few] минуты
        [many] минут
       *[other] минуты
    } { $seconds } { $seconds ->
        [one] секунду
        [few] секунды
        [many] секунд
       *[other] секунды
    } из { $sentence } { $sentence ->
        [one] минуты
        [few] минут
        [many] минут
       *[other] минуты
    } вашего приговора за "{ $crime }".
genpop-prisoner-id-examine-wait-perm = Вы отбываете бессрочное наказание за "{ $crime }".
genpop-prisoner-id-examine-served = Вы отбыли своё наказание за "{ $crime }".
genpop-locker-name-default = шкаф заключённого
genpop-locker-desc-default = Это защищённый шкафчик для персональных вещей заключённого во время его пребывания в тюрьме.
genpop-locker-name-used = шкаф заключённого ({ $name })
genpop-locker-action-clear-id = Очистить ID
genpop-locker-action-reset-sentence = Сброс времени приговора ({ NATURALFIXED($percent, 0) }% отбыто)

genpop-locker-action-end-early = Досрочное освобождение от наказания

genpop-locker-desc-used = Это защищённый шкафчик для персональных вещей заключённого во время его пребывания в тюрьме. Содержит личные вещи { $name }.

genpop-locker-ui-label-crime = [bold]Преступление:[/bold]

genpop-locker-ui-label-name = [bold]Имя осуждённого:[/bold]

genpop-locker-ui-label-sentence = [bold]Продолжительность наказания в минутах:[/bold] [color=gray](0 для бессрочного)[/color]

genpop-locket-ui-button-done = Готово
