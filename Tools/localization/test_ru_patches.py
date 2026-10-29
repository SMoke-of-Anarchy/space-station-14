#!/usr/bin/env python3
"""Static/resource regressions for Russian patches, NOT a Robust runtime test.
Run from any directory: python3 Tools/localization/test_ru_patches.py
The .NET/NUnit tests must also be run on a machine with the project SDK.
"""
from pathlib import Path
import re
import json
import subprocess
import unittest
from functools import lru_cache
from fluent.syntax import FluentParser, ast
import yaml


class PrototypeLoader(yaml.CSafeLoader):
    pass


def robust_tag(loader, tag, node):
    if isinstance(node, yaml.MappingNode):
        return loader.construct_mapping(node)
    if isinstance(node, yaml.SequenceNode):
        return loader.construct_sequence(node)
    return loader.construct_scalar(node)


PrototypeLoader.add_multi_constructor('!type:', robust_tag)


class RobustParser(FluentParser):
    def get_literal(self, ps):
        if ps.current_char == '$':
            return self.get_inline_expression(ps)
        return super().get_literal(ps)

ROOT = Path(__file__).resolve().parents[2]


def source(path):
    return (ROOT / path).read_text(encoding='utf-8-sig')


@lru_cache(maxsize=None)
def russian_messages():
    records = {}
    for file in (ROOT / 'Resources/Locale/ru-RU').rglob('*.ftl'):
        for entry in RobustParser().parse(file.read_text(encoding='utf-8-sig')).body:
            if isinstance(entry, ast.Junk):
                raise AssertionError(f'Fluent syntax error in {file}: {entry.content}')
            if isinstance(entry, (ast.Message, ast.Term)):
                key = ('-' if isinstance(entry, ast.Term) else '') + entry.id.name
                if key in records:
                    raise AssertionError(f'Duplicate Fluent ID: {key}')
                records[key] = entry
    return records


@lru_cache(maxsize=None)
def localized_datasets():
    rows = []
    for file in (ROOT / 'Resources/Prototypes').rglob('*.yml'):
        text = file.read_text(encoding='utf-8-sig')
        if 'type: localizedDataset' not in text:
            continue
        for row in yaml.load(text, Loader=PrototypeLoader) or []:
            if row.get('type') == 'localizedDataset':
                rows.append((file, row))
    return rows


class RussianEmoteTests(unittest.TestCase):
    def test_emote_triggers_are_unambiguous(self):
        rows = yaml.load(source('Resources/Prototypes/Voice/speech_emotes.yml'), Loader=PrototypeLoader)
        seen = {}
        for row in rows:
            for trigger in row.get('chatTriggers', []):
                folded = trigger.lower()
                self.assertNotIn(folded, seen, f'{trigger}: {row["id"]} versus {seen.get(folded)}')
                seen[folded] = row['id']
        for trigger, emote in {'кричит': 'Scream', 'смеётся': 'Laugh', 'хонк': 'Honk',
                               'аплодирует': 'Clap', 'апплодирует': 'Clap',
                               'хлопнул в ладоши': 'ClapSingle', 'пинг': 'Ping',
                               'scream': 'Scream', 'laugh': 'Laugh'}.items():
            self.assertEqual(seen[trigger], emote)
        self.assertEqual(sum(bool(re.search('[А-Яа-яЁё]', k)) for k in seen), 81)

    def test_chat_shorthands_reference_existing_translations(self):
        code = source('Content.Server/Chat/Managers/ChatSanitizationManager.cs')
        entries = re.findall(r'Entry\("([^"\n]+)", "([^"\n]+)"\)', code)
        seen = set()
        for shorthand, key in entries:
            self.assertNotIn(shorthand.lower(), seen)
            seen.add(shorthand.lower())
            self.assertTrue(key in russian_messages(), f'Missing shorthand translation: {key}')
        for shorthand in ('хд', 'о-о', 'о.о', '0_о', 'о/', 'о7', 'лмао', 'рофл', 'яхз', 'кек', 'т_т', '!с', '!в', '!х', '!щ'):
            self.assertIn(shorthand, seen)
        words = [x[0] for x in entries]
        self.assertLess(words.index('))'), words.index(')'))
        self.assertLess(words.index('(('), words.index('('))


class RussianDatasetTests(unittest.TestCase):
    def test_counts_match_all_russian_entries(self):
        ids = russian_messages()
        self.assertGreaterEqual(len(localized_datasets()), 200)
        for file, row in localized_datasets():
            values = row['values']
            prefix = values['prefix']
            indices = {int(key[len(prefix):]) for key in ids
                       if key.startswith(prefix) and key[len(prefix):].isdigit()}
            with self.subTest(dataset=row['id'], file=str(file)):
                self.assertEqual(indices, set(range(1, values['count'] + 1)))


class RussianLocalizationTests(unittest.TestCase):
    def test_russian_fluent_syntax_and_uniqueness(self):
        self.assertGreater(len(russian_messages()), 45000)

    def test_english_plural_functions_are_not_used_in_russian(self):
        for file in (ROOT / 'Resources/Locale/ru-RU').rglob('*.ftl'):
            self.assertNotRegex(file.read_text(encoding='utf-8-sig'), r'\b(?:MANY|MAKEPLURAL)\s*\(', str(file))
        code = source('Content.Shared/Localizations/ContentLocalizationManager.cs')
        self.assertIn('new CultureInfo(FallbackCulture)', code)
        self.assertIn('_loc.SetFallbackCulture(cultureEn)', code)
        self.assertIn('_loc.SetCulture(culture)', code)
        self.assertIn('if (!_loc.HasCulture(loadedCulture))', code)
        self.assertIn('_loc.AddFunction(cultureEn, "MANY", FormatMany)', code)
        self.assertNotIn('_loc.AddFunction(culture, "MANY"', code)

    def test_duration_plural_categories(self):
        entry = russian_messages()['-entity-effect-duration-seconds']
        select = entry.value.elements[0].expression
        forms = {v.key.name: ''.join(e.value for e in v.value.elements) for v in select.variants}
        self.assertEqual(forms, {'one': 'секунду', 'few': 'секунды', 'many': 'секунд', 'other': 'секунды'})
        numbers = [0, 1, 2, 5, 11, 21, 22, 25, 101, 1.5]
        categories = json.loads(subprocess.check_output(['node', '-e',
            'const p=new Intl.PluralRules("ru");console.log(JSON.stringify(JSON.parse(process.argv[1]).map(n=>p.select(n))))',
            json.dumps(numbers)], text=True))
        self.assertEqual([forms[c] for c in categories],
                         ['секунд', 'секунду', 'секунды', 'секунд', 'секунд', 'секунду', 'секунды', 'секунд', 'секунду', 'секунды'])
        text = source('Resources/Locale/ru-RU/guidebook/entity-effects/effects.ftl')
        self.assertEqual(text.count('-entity-effect-duration-seconds(seconds:'), 16)
        self.assertNotIn('метаболизатор в течение', text)


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
