# FTLdiskburner
cmd-ftldisk-desc = Создаёт диск координат ССТП для полёта на карту, где находится указанная сущность EntityID.

cmd-ftldisk-help = ftldisk [EntityID]

cmd-ftldisk-no-transform = У сущности { $destination } отсутствует компонент Transform!

cmd-ftldisk-no-map = У сущности { $destination } нет карты!

cmd-ftldisk-no-map-comp = Сущность { $destination } каким-то образом находится на карте { $map }, но у неё отсутствует компонент карты.

cmd-ftldisk-map-not-init = Сущность { $destination } находится на неинициализированной карте { $map }! Убедитесь, что её безопасно инициализировать, и сначала инициализируйте карту, иначе игроки застрянут на месте!

cmd-ftldisk-map-paused = Сущность { $desintation } находится на приостановленной карте { $map }! Сначала возобновите симуляцию карты, иначе игроки застрянут на месте.

cmd-ftldisk-planet = Сущность { $desintation } находится на планетарной карте { $map }, для которой потребуется точка ССТП. Возможно, она уже существует.

cmd-ftldisk-already-dest-not-enabled = Сущность { $destination } находится на карте { $map }, у которой уже есть компонент FTLDestinationComponent, но он не включён! Для безопасности включите его вручную.

cmd-ftldisk-requires-ftl-point = Для путешествия к карте { $map }, на которой находится сущность { $destination }, нужна точка ССТП! Возможно, она уже существует.

cmd-ftldisk-hint = Сетевой ID карты
