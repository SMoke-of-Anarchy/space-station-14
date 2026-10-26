## Strings for the "grant_connect_bypass" command.

cmd-grant_connect_bypass-desc = Временно разрешить пользователю обходить стандартные проверки при подключении.
cmd-grant_connect_bypass-help = Использование: grant_connect_bypass <пользователь> [длительность в минутах]
    Временно разрешает пользователю обходить стандартные ограничения на подключение.
    Обход действует только на этом игровом сервере и истекает через 1 час (по умолчанию).
    Пользователь сможет подключиться независимо от белого списка, «Паник-бункера» и лимита игроков.
cmd-grant_connect_bypass-arg-user = <пользователь>
cmd-grant_connect_bypass-arg-duration = [длительность в минутах]
cmd-grant_connect_bypass-invalid-args = Ожидается один или два аргумента
cmd-grant_connect_bypass-unknown-user = Не удалось найти пользователя «{ $user }»
cmd-grant_connect_bypass-invalid-duration = Недопустимая длительность: «{ $duration }»
cmd-grant_connect_bypass-success = Для пользователя «{ $user }» успешно добавлено исключение
