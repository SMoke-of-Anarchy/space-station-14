### UI

# Displayed in the Character prefs window
humanoid-character-profile-summary =
    Это { $name }, возраст — { $age } { $age ->
        [one] год
        [few] года
        [many] лет
       *[other] года
    }. { $gender ->
        [male] Он
        [female] Она
        [epicene] Они
       *[neuter] Оно
    }.
