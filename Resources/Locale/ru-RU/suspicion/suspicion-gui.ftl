## SuspicionGui.xaml.cs

# Shown when clicking your Role Button in Suspicion
suspicion-ally-count-display =
    { $allyCount ->
       [0] Вы сами по себе.
       [zero] Вы сами по себе.
        [1] Ваш союзник: { $allyNames }.
        [one] Ваши союзники: { $allyNames }.
        [few] Ваши союзники: { $allyNames }.
        [many] Ваши союзники: { $allyNames }.
        *[other] Ваши союзники: { $allyNames }.
    }
