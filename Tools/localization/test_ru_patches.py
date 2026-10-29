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
import xml.etree.ElementTree as ET


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


class RussianResearchWindowTests(unittest.TestCase):
    def test_research_window_has_room_for_russian_labels(self):
        root = ET.fromstring(source('Content.Client/Research/UI/ResearchConsoleMenu.xaml'))
        self.assertEqual(root.attrib['SetSize'], '760 550')
        self.assertEqual(root.attrib['MinSize'], '625 400')
        self.assertEqual(root.attrib['Title'], "{Loc 'research-console-menu-title'}")


class RussianAccentTests(unittest.TestCase):
    base = 'Content.Shared/Speech/EntitySystems/'

    def test_all_accents_keep_shared_prediction(self):
        for name in ('BarkAccentSystem', 'FrontalLispSystem', 'LizardAccentSystem', 'MonkeyAccentSystem',
                     'MothAccentSystem', 'OwOAccentSystem', 'SlurredSystem', 'StutteringSystem'):
            code = source(self.base + name + '.cs')
            with self.subTest(system=name):
                self.assertIn('RelayAccentSystem<', code)
                self.assertIn('SharedRandomExtensions.PredictedRandom', code)
                self.assertNotIn('new Random(', code)
                self.assertNotIn('_random.Prob(', code)
                self.assertNotIn('_random.Pick(', code)

    def test_stutter_regex_has_no_accidental_unicode_ranges(self):
        code = source(self.base + 'StutteringSystem.cs')
        pattern = re.search(r'Stutter = new\("([^"]+)"', code).group(1)
        regex = re.compile(pattern, re.I)
        for letter in 'бвгджзйклмнпрстфхцчшщБВГДЖЗЙКЛМНПРСТФХЦЧШЩbcdfghjklmnpqrstvwxyz':
            self.assertIsNotNone(regex.fullmatch(letter), letter)
        for letter in 'аеёиоуыэюяАЕЁИОУЫЭЮЯъьЪЬaeiou0123 _-αé™':
            self.assertIsNone(regex.fullmatch(letter), letter)
        self.assertIn('RegexOptions.CultureInvariant', code)

    def test_cyrillic_hiss_and_buzz_patterns(self):
        code = source(self.base + 'LizardAccentSystem.cs')
        pattern = re.search(r'RegexRussianHiss = new\("([^"]+)"', code).group(1)
        replacements = dict(re.findall(r"'(.)' => '(.)'", code))
        rendered = re.sub(pattern, lambda m: replacements.get(m[0][0], m[0][0]) * 2, 'с з ш ч С З Ш Ч')
        self.assertEqual(rendered, 'сс сс шш щщ СС СС ШШ ЩЩ')
        self.assertIn('random.Next(2, 4)', code)
        code = source(self.base + 'MothAccentSystem.cs')
        pattern = re.search(r'RegexRussianBuzz = new\("([^"]+)"', code).group(1)
        self.assertEqual(re.sub(pattern, lambda m: m[0][0] * 2, 'ж з Ж З'), 'жж зз ЖЖ ЗЗ')
        self.assertIn('random.Next(2, 4)', code)

    def test_bark_monkey_owo_and_slurred_rules_present(self):
        bark = source(self.base + 'BarkAccentSystem.cs')
        self.assertIn('" Гав!", " ГАВ", " вуф-вуф"', bark)
        self.assertIn('.Replace("Л", "Р")', bark)
        monkey = source(self.base + 'MonkeyAccentSystem.cs')
        for letter in 'УКГА':
            self.assertIn(f"Append('{letter}')", monkey)
        owo = source(self.base + 'OwOAccentSystem.cs')
        for before, after in [('р', 'в'), ('Р', 'В'), ('л', 'в'), ('Л', 'В')]:
            self.assertIn(f'.Replace("{before}", "{after}")', owo)
        self.assertIn('{ "ты", "ти" }', owo)
        slurred = source(self.base + 'SlurredSystem.cs')
        for before, after in [('о', 'а'), ('к', 'кх'), ('щ', 'шч'), ('ц', 'тс')]:
            self.assertRegex(slurred, re.escape(f"'{before}' =>") + r'.*' + re.escape(f'"{after}"'))
        lisp = source(self.base + 'FrontalLispSystem.cs')
        self.assertIn('random.Prob(0.90f)', lisp)
        self.assertIn('RegexRussianLisp.Replace(message, match =>', lisp)
        self.assertIn("'с' or 'ч' =>", lisp)


class RussianRadioTests(unittest.TestCase):
    def test_radio_codes_and_shared_prefixes(self):
        rows = yaml.load(source('Resources/Prototypes/radio_channels.yml'), Loader=PrototypeLoader)
        codes = {r['id']: r['keycode'] for r in rows if 'keycode' in r}
        expected = {'Common': ';', 'CentCom': 'ц', 'Command': 'к', 'Engineering': 'и',
                    'Medical': 'м', 'Science': 'н', 'Security': 'о', 'Service': 'с',
                    'Supply': 'п', 'Syndicate': 'т', 'Binary': 'д', 'Freelance': 'ф',
                    'Xenoborg': 'ч', 'Mothership': 'я'}
        self.assertEqual(codes, expected)
        self.assertEqual(len(set(codes.values())), len(codes))
        self.assertTrue(all(len(k) == 1 for k in codes.values()))
        self.assertNotIn('р', codes.values(), 'Default-channel key must not shadow a real channel')
        code = source('Content.Shared/Chat/SharedChatSystem.cs')
        for name, value in [('LOOCPrefix', '_'), ('EmotesPrefix', '%'), ('DefaultChannelKey', 'р')]:
            self.assertIn(f"const char {name} = '{value}'", code)
        for name, value in [('RadioCommonPrefix', ';'), ('RadioChannelPrefix', ':'),
                            ('RadioChannelAltPrefix', '.'), ('EmotesAltPrefix', '*')]:
            self.assertIn(f"const char {name} = '{value}'", code)
        client = source('Content.Client/UserInterface/Systems/Chat/ChatUIController.cs')
        self.assertIn('SharedChatSystem.LOOCPrefix', client)
        self.assertIn('SharedChatSystem.EmotesPrefix', client)

    def test_radio_guide_matches_keys(self):
        text = source('Resources/ServerInfo/Guidebook/NewPlayer/Controls/Radio.xml')
        ET.fromstring(text)
        for key in 'рцкимносптдфчя':
            self.assertIn(':' + key, text)
        self.assertNotIn(':h', text)
        self.assertNotIn(':e', text)
        self.assertIn('[color=#a4885c]%[/color]', text)
        self.assertIn('[color=#a4885c]_[/color]', text)


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
