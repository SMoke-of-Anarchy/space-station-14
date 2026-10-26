discord-watchlist-connection-header =
    { $players } { $players ->
        [one] игрок в списке наблюдения подключился
        [few] игрока в списке наблюдения подключились
        [many] игроков в списке наблюдения подключились
       *[other] игроков в списке наблюдения подключились
    } к { $serverName }
discord-watchlist-connection-entry =
    - { $playerName } с сообщением "{ $message }"{ $expiry ->
        [0] { "" }
       *[other] { " " }(истекает <t:{ $expiry }:R>)
    }{ $otherWatchlists ->
        [0] { "" }
        [one] { " " }и ещё { $otherWatchlists } наблюдение
        [few] { " " }и ещё { $otherWatchlists } наблюдения
        [many] { " " }и ещё { $otherWatchlists } наблюдений
       *[other] { " " }и ещё { $otherWatchlists } наблюдения
    }
