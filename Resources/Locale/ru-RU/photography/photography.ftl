# TODO: сделать эту Fluent-функцию в RT
photograph-name-text = На фотографии изображено: { PROPER($entity) ->
    *[false] { $entity } {$entity}
     [true] { $entity }
    }.

photograph-name-text-empty = Это фотография.

photograph-name-text-photograph = На этой фотографии изображена другая фотография.
