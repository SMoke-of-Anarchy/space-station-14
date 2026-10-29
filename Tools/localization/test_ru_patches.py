#!/usr/bin/env python3
"""Static/resource regressions for Russian patches, NOT a Robust runtime test.
Run from any directory: python3 Tools/localization/test_ru_patches.py
The .NET/NUnit tests must also be run on a machine with the project SDK.
"""
from pathlib import Path
import re
import unittest

ROOT = Path(__file__).resolve().parents[2]


def source(path):
    return (ROOT / path).read_text(encoding='utf-8-sig')


class RussianNameTests(unittest.TestCase):
    def test_actual_restricted_name_pattern(self):
        text = source('Content.Shared/Preferences/HumanoidCharacterProfile.cs')
        pattern = re.search(r'RestrictedNameRegex = new\(@"([^"]+)"\)', text).group(1)
        for value, expected in [
            ('Иван Петров', 'Иван Петров'), ('Ёжик ёжик', 'Ёжик ёжик'),
            ("Анна-Мария O'Brien 42", "Анна-Мария O'Brien 42"),
            ('John Smith', 'John Smith'), ('Иван_Петров<script>!', 'ИванПетровscript'),
            ('Иван\nПетров\t', 'ИванПетров'), ('山田 Иван', ' Иван'),
        ]:
            with self.subTest(value=value):
                self.assertEqual(re.sub(pattern, '', value), expected)
        self.assertIn('RestrictedNameRegex.Replace(name, string.Empty)', text)


if __name__ == '__main__':
    unittest.main(verbosity=2)
