agent-id-new =
    { CAPITALIZE($card) } { $number ->
        [0] не дала новых доступов
        [one] дала { $number } новый доступ
        [few] дала { $number } новых доступа
        [many] дала { $number } новых доступов
       *[other] дала { $number } нового доступа
    }.
agent-id-card-current-name = Имя:
agent-id-card-current-job = Должность:
agent-id-card-job-icon-label = Иконка:
agent-id-menu-title = ID карта Агента
agent-id-open-ui-verb = Настроить
agent-id-ui-footer-flavor-left = Обычное удостоверение, ничего особенного
agent-id-ui-footer-flavor-right = v2.0
agent-id-ui-input-job = Должность:
agent-id-ui-input-name = Имя:
agent-id-ui-menu-title = Данные ID-карты
agent-id-ui-tab-settings = Настройки
agent-id-ui-tab-job-icons = Значки должностей
agent-id-ui-os = Nuke#OS ™
agent-id-ui-os-flavor = Если сомневаетесь, никто не заподозрит мима.