import xml.etree.ElementTree as ET
from Card import Card
from Territory import Territory


def read_objects_cards():

# Read XML file
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()

    cards = []

    for card in root.findall('object'):
        card_id = card.get('id')
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text

        card = Card(card_id, image, function, description)
        cards.append(card)
    print(cards)
    return cards

def read_territories_cards():
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()

    cards = []

    for card in root.findall('territory'):
        card_id = card.get('id')
        name = card.get('name')
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text
        continent = card.find('continent').text

        card = Territory(card_id, image, function, description, name, 0, continent)
        cards.append(card)
    print(cards)
    return cards
