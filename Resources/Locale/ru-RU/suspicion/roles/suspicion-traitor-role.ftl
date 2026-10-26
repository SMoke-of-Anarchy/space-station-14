# Shown when greeted with the Suspicion role
suspicion-role-greeting = Вы { $roleName }!
# Shown when greeted with the Suspicion role
suspicion-objective = Цель: { $objectiveText }
# Shown when greeted with the Suspicion role
suspicion-partners-in-crime =
    { $partnersCount ->
        [0] Вы сами по себе. Удачи!
        [zero] Вы сами по себе. Удачи!
        [1] Ваш союзник: { $partnerNames }.
        [one] Ваши союзники: { $partnerNames }.
        [few] Ваши союзники: { $partnerNames }.
        [many] Ваши союзники: { $partnerNames }.
       *[other] Ваши союзники: { $partnerNames }.
    }
