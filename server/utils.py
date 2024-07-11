import xml.etree.ElementTree as ET
from Card import Card
from Territory import Territory


def read_objects_cards():
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()
    cards = []

    for card in root.findall('objects/object'):
        card_id = card.get('id')
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text

        card = Card(card_id, image, function, description, None)
        cards.append(card)
    return cards


def read_territories_cards():
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()
    cards = []

    for card in root.findall('.//territory'):
        card_id = card.get('id')
        name = card.find('name').text
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text
        continent = card.find('continent').text

        card = Territory(card_id, image, function, description, None, name, 0, continent)
        cards.append(card)
    return cards
